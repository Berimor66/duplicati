﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Duplicati.Library.Utility;
using Duplicati.Library.Main.Database;
using Duplicati.Library.Main.Volumes;
using Newtonsoft.Json;

namespace Duplicati.Library.Main
{
    public class BackendManager : IDisposable
    {
    	public const string VOLUME_HASH = "SHA256";
    
        /// <summary>
        /// Class to represent hash failures
        /// </summary>
        [Serializable]
        public class HashMismathcException : Exception
        {
            /// <summary>
            /// Default constructor, sets a generic string as the message
            /// </summary>
            public HashMismathcException() : base() { }

            /// <summary>
            /// Constructor with non-default message
            /// </summary>
            /// <param name="message">The exception message</param>
            public HashMismathcException(string message) : base(message) { }

            /// <summary>
            /// Constructor with non-default message and inner exception details
            /// </summary>
            /// <param name="message">The exception message</param>
            /// <param name="innerException">The exception that caused this exception</param>
            public HashMismathcException(string message, Exception innerException) : base(message, innerException) { }
        }
    
        private enum OperationType
        {
            Get,
            Put,
            List,
            Delete,
            CreateFolder,
            Terminate
        }

        public interface IDownloadWaitHandle
        {
            TempFile Wait();
            TempFile Wait(out string hash, out long size);
        }

        private class FileEntryItem : IDownloadWaitHandle
        {
            public OperationType Operation;
            public string RemoteFilename;
            public string LocalFilename;
            public bool Encrypted;
            public object Result;
            public string Hash;
            public long Size;
            public IndexVolumeWriter Indexfile;
            public Exception Exception;
            public bool ExceptionKillsHandler;

            private System.Threading.ManualResetEvent DoneEvent;

            public FileEntryItem(OperationType operation, string remotefilename, IndexVolumeWriter indexfile = null)
            {
                Operation = operation;
                RemoteFilename = remotefilename;
                Indexfile = indexfile;
                ExceptionKillsHandler = operation != OperationType.Get;
                Size = -1;

                DoneEvent = new System.Threading.ManualResetEvent(false);
            }

            public FileEntryItem(OperationType operation, string remotefilename, long size, string hash, IndexVolumeWriter indexfile = null)
                : this(operation, remotefilename, indexfile)
            {
                Size = size;
                Hash = hash;
            }

            public void SignalComplete()
            {
                DoneEvent.Set();
            }

            public void WaitForComplete()
            {
                DoneEvent.WaitOne();
            }

            TempFile IDownloadWaitHandle.Wait()
            {
                this.WaitForComplete();
                if (Exception != null)
                	throw Exception;
                	
                return (TempFile)this.Result;
            }

            TempFile IDownloadWaitHandle.Wait(out string hash, out long size)
            {
                this.WaitForComplete();
                
                if (Exception != null)
                	throw Exception;
                	
                hash = this.Hash;
                size = this.Size;
                
                return (TempFile)this.Result;
            }

            public void Encrypt(Library.Interface.IEncryption encryption, IBackendWriter stat)
            {
                if (encryption != null && !this.Encrypted)
                {
                    var sourcefile = this.LocalFilename + "." + encryption.FilenameExtension;
                    encryption.Encrypt(this.LocalFilename, sourcefile);
                    this.DeleteLocalFile(stat);
                    this.LocalFilename = sourcefile;
                    this.Hash = null;
                    this.Size = 0;
                    this.Encrypted = true;
                }
            }

            public static string CalculateFileHash(string filename)
            {
                using (System.IO.FileStream fs = System.IO.File.OpenRead(filename))
                    return Convert.ToBase64String(System.Security.Cryptography.HashAlgorithm.Create(VOLUME_HASH).ComputeHash(fs));
            }

            public bool UpdateHashAndSize(Options options)
            {
                if (Hash == null || Size < 0)
                {
                    Hash = CalculateFileHash(this.LocalFilename);
                    Size = new System.IO.FileInfo(this.LocalFilename).Length;
                    return true;
                }

                return false;
            }
            
            public void DeleteLocalFile(IBackendWriter stat)
            {
            	try 
            	{ 
            		System.IO.File.Delete(this.LocalFilename); 
            	}
            	catch (Exception ex) 
            	{ 
            		stat.AddError(string.Format("Failed to delete local file \"{0}\": {1}", this.LocalFilename, ex.Message), ex); 
            	}
            }
            
            public BackendActionType BackendActionType
            { 
                get
                {
                    switch (this.Operation)
                    {
                        case OperationType.Get:
                            return BackendActionType.Get;
                        case OperationType.Put:
                            return BackendActionType.Put;
                        case OperationType.Delete:
                            return BackendActionType.Delete;
                        case OperationType.List:
                            return BackendActionType.List;
                        case OperationType.CreateFolder:
                            return BackendActionType.CreateFolder;
                        default:
                            throw new Exception(string.Format("Unexpected operation type: {0}", this.Operation));
                    }
                }
            }
            
        }
        
        private class DatabaseCollector
        {
	        private object m_dbqueuelock = new object();
	        private LocalDatabase m_database;
	        private System.Threading.Thread m_callerThread;
	        private List<IDbEntry> m_dbqueue;
	        private IBackendWriter m_stats;
	        
	        private interface IDbEntry { }
	        
	        private class DbOperation : IDbEntry
	        {
	        	public string Action;
	        	public string File;
	        	public string Result;
	        }
	        
	        private class DbUpdate : IDbEntry
	        {
	        	public string Remotename;
	        	public RemoteVolumeState State;
	        	public long Size;
	        	public string Hash;
	        }
	        
	        public DatabaseCollector(LocalDatabase database, IBackendWriter stats)
	        {
	        	m_database = database;
	        	m_stats = stats;
	        	m_dbqueue = new List<IDbEntry>();	
	        	if (m_database != null)
	        		m_callerThread = System.Threading.Thread.CurrentThread;
	        }

	
	        public void LogDbOperation(string action, string file, string result)
	        {
	        	lock(m_dbqueuelock)
		    		m_dbqueue.Add(new DbOperation() { Action = action, File = file, Result = result });
	        }
	        
	        public void LogDbUpdate(string remotename, RemoteVolumeState state, long size, string hash)
	        {
	        	lock(m_dbqueuelock)
		    		m_dbqueue.Add(new DbUpdate() { Remotename = remotename, State = state, Size = size, Hash = hash });
	        }

	        public bool FlushDbMessages(bool checkThread = false)
	        {
	        	if (m_database != null && (checkThread == false || m_callerThread == System.Threading.Thread.CurrentThread))
	        		return FlushDbMessages(m_database, null);
	        	
	        	return false;
	        }
	        
	        public bool FlushDbMessages(LocalDatabase db, System.Data.IDbTransaction transaction)
	        {
	        	List<IDbEntry> entries;
	        	lock(m_dbqueuelock)
	        		if (m_dbqueue.Count == 0)
	        			return false;
	        		else
	        		{
	        			entries = m_dbqueue;
	        			m_dbqueue = new List<IDbEntry>();
	        		}
	        	
	        	//As we replace the list, we can now freely access the elements without locking
	        	foreach(var e in entries)
	        		if (e is DbOperation)
	        			db.LogRemoteOperation(((DbOperation)e).Action, ((DbOperation)e).File, ((DbOperation)e).Result, transaction);
	        		else if (e is DbUpdate)
	        			db.UpdateRemoteVolume(((DbUpdate)e).Remotename, ((DbUpdate)e).State, ((DbUpdate)e).Size, ((DbUpdate)e).Hash, transaction);
	        		else if (e != null)
	        			m_stats.AddError(string.Format("Queue had element of type: {0}, {1}", e.GetType(), e.ToString()), null);
	        			
	        	return true;
	        }
        }

        private BlockingQueue<FileEntryItem> m_queue;
        private Options m_options;
        private volatile Exception m_lastException;
        private Library.Interface.IEncryption m_encryption;
        private Library.Interface.IBackend m_backend;
        private string m_backendurl;
        private IBackendWriter m_statwriter;
        private System.Threading.Thread m_thread;
		private DatabaseCollector m_db;
                
        public string BackendUrl { get { return m_backendurl; } }

        public BackendManager(string backendurl, Options options, IBackendWriter statwriter, LocalDatabase database)
        {
            m_options = options;
            m_backendurl = backendurl;
            m_statwriter = statwriter;
            m_db = new DatabaseCollector(database, statwriter);

            m_backend = DynamicLoader.BackendLoader.GetBackend(m_backendurl, m_options.RawOptions);
            if (m_backend == null)
            	throw new Exception(string.Format("Backend not supported: {0}", m_backendurl));

            if (!m_options.NoEncryption)
            {
                m_encryption = DynamicLoader.EncryptionLoader.GetModule(m_options.EncryptionModule, m_options.Passphrase, m_options.RawOptions);
                if (m_encryption == null)
                	throw new Exception(string.Format("Encryption method not supported: ", m_options.EncryptionModule));
            }

            m_queue = new BlockingQueue<FileEntryItem>(options.SynchronousUpload ? 1 : (options.AsynchronousUploadLimit == 0 ? int.MaxValue : options.AsynchronousUploadLimit));
            m_thread = new System.Threading.Thread(this.ThreadRun);
            m_thread.Name = "Backend Async Worker";
            m_thread.IsBackground = true;
            m_thread.Start();
        }
        
        private void ThreadRun()
        {
            while (!m_queue.Completed)
            {
                var item = m_queue.Dequeue();
                if (item != null)
                {
                    int retries = 0;
                    Exception lastException = null;

                    do
                    {
                        try
                        {
                            if (m_options.NoConnectionReuse && m_backend != null)
                            {
                                m_backend.Dispose();
                                m_backend = null;
                            }

                            if (m_backend == null)
                                m_backend = DynamicLoader.BackendLoader.GetBackend(m_backendurl, m_options.RawOptions);

                            switch (item.Operation)
                            {
                                case OperationType.Put:
                                    DoPut(item);
                                    break;
                                case OperationType.Get:
                                    DoGet(item);
                                    break;
                                case OperationType.List:
                                    DoList(item);
                                    break;
                                case OperationType.Delete:
                                    DoDelete(item);
                                    break;
                                case OperationType.CreateFolder:
                                    DoCreateFolder(item);
                                    break;
                                case OperationType.Terminate:
                                    m_queue.SetCompleted();
                                    break;
                            }

                            lastException = null;
                            retries = m_options.NumberOfRetries;
                        }
                        catch (Exception ex)
                        {
                            retries++;
                            lastException = ex;
                            m_statwriter.AddRetryAttempt(string.Format("Operation {0} with file {1} attempt {2} of {3} failed with message: {4}", item.Operation, item.RemoteFilename, retries, m_options.NumberOfRetries, ex.Message), ex);
                            m_statwriter.SendEvent(item.BackendActionType, retries < m_options.NumberOfRetries ? BackendEventType.Retrying : BackendEventType.Failed, item.RemoteFilename, item.Size);

							bool recovered = false;
                            if (ex is Duplicati.Library.Interface.FolderMissingException && m_options.AutocreateFolders)
                            {
	                            try 
	                            { 
	                            	// If we successfully create the folder, we can re-use the connection
	                            	m_backend.CreateFolder(); 
	                            	recovered = true;
	                            }
	                            catch(Exception dex) 
	                            { 
	                            	m_statwriter.AddWarning(string.Format("Failed to create folder: {0}", ex.Message), dex); 
	                            }
                            }
                            
                            if (!recovered)
                            {
	                            try { m_backend.Dispose(); }
	                            catch(Exception dex) { m_statwriter.AddWarning(string.Format("Failed to dispose backend instance: {0}", ex.Message), dex); }
	
	                            m_backend = null;
	                            
	                            if (retries < m_options.NumberOfRetries && m_options.RetryDelay.Ticks != 0)
	                                System.Threading.Thread.Sleep(m_options.RetryDelay);
	                        }
                            
                        }

                    } while (retries < m_options.NumberOfRetries);

                    if (lastException != null)
                    {
                    	item.Exception = lastException;
                        if (item.Operation == OperationType.Put)
                        	item.DeleteLocalFile(m_statwriter);
                        	
                        if (item.ExceptionKillsHandler)
                        {
                        	m_lastException = lastException;

							//TODO: If there are temp files in the queue, we must delete them
	                        m_queue.SetCompleted();
                       	}
                        
                    }
                    
                    item.SignalComplete();
                }
            }

            //Make sure everything in the queue is signalled
            FileEntryItem i;
            while ((i = m_queue.Dequeue()) != null)
                i.SignalComplete();
        }

        private void DoPut(FileEntryItem item)
        {
            item.Encrypt(m_encryption, m_statwriter);
            if (item.UpdateHashAndSize(m_options))
            	m_db.LogDbUpdate(item.RemoteFilename, RemoteVolumeState.Uploading, item.Size, item.Hash);

            if (item.Indexfile != null)
            {
                item.Indexfile.FinishVolume(item.Hash, item.Size);
                item.Indexfile.Close();
                item.Indexfile = null;
            }            

            m_db.LogDbOperation("put", item.RemoteFilename, JsonConvert.SerializeObject(new { Size = item.Size, Hash = item.Hash }));
            m_statwriter.SendEvent(BackendActionType.Put, BackendEventType.Started, item.RemoteFilename, item.Size);

            if (m_backend is Library.Interface.IStreamingBackend && !m_options.DisableStreamingTransfers)
            {
                using (var fs = System.IO.File.OpenRead(item.LocalFilename))
                using (var ts = new ThrottledStream(fs, m_options.MaxDownloadPrSecond, m_options.MaxUploadPrSecond))
                using (var pgs = new Library.Utility.ProgressReportingStream(ts, item.Size))
                    ((Library.Interface.IStreamingBackend)m_backend).Put(item.RemoteFilename, pgs);
            }
            else
                m_backend.Put(item.RemoteFilename, item.LocalFilename);

			m_db.LogDbUpdate(item.RemoteFilename, RemoteVolumeState.Uploaded, item.Size, item.Hash);
			
            m_statwriter.SendEvent(BackendActionType.Put, BackendEventType.Completed, item.RemoteFilename, item.Size);

            if (m_options.ListVerifyUploads)
            {
                var f = m_backend.List().Where(n => n.Name.Equals(item.RemoteFilename, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                if (f == null)
                    throw new Exception(string.Format("List verify failed, file was not found after upload: {0}", f.Name));
                else if (f.Size != item.Size && f.Size >= 0)
                    throw new Exception(string.Format("List verify failed for file: {0}, size was {1} but expected to be {2}", f.Name, f.Size, item.Size));
            }

			item.DeleteLocalFile(m_statwriter);
        }

        private void DoGet(FileEntryItem item)
        {
            Library.Utility.TempFile tmpfile = null;
            m_statwriter.SendEvent(BackendActionType.Get, BackendEventType.Started, item.RemoteFilename, item.Size);

            try
            {
                tmpfile = new Library.Utility.TempFile();
                if (m_backend is Library.Interface.IStreamingBackend && !m_options.DisableStreamingTransfers)
                {
                    using (var fs = System.IO.File.OpenWrite(tmpfile))
                    using (var ts = new ThrottledStream(fs, m_options.MaxDownloadPrSecond, m_options.MaxUploadPrSecond))
                    using (var pgs = new Library.Utility.ProgressReportingStream(ts, item.Size))
                        ((Library.Interface.IStreamingBackend)m_backend).Get(item.RemoteFilename, pgs);
                }
                else
                    m_backend.Get(item.RemoteFilename, tmpfile);
                
                m_db.LogDbOperation("get", item.RemoteFilename, JsonConvert.SerializeObject(new { Size = new System.IO.FileInfo(tmpfile).Length, Hash = FileEntryItem.CalculateFileHash(tmpfile) }));
                m_statwriter.SendEvent(BackendActionType.Get, BackendEventType.Completed, item.RemoteFilename, new System.IO.FileInfo(tmpfile).Length);

                if (!m_options.SkipFileHashChecks)
                {
                    var nl = new System.IO.FileInfo(tmpfile).Length;
                    if (item.Size >= 0)
                    {
                        if (nl != item.Size)
                            throw new Exception(string.Format(Strings.Controller.DownloadedFileSizeError, item.RemoteFilename, nl, item.Size));
                    }
                    else
                    	item.Size = nl;

                    var nh = FileEntryItem.CalculateFileHash(tmpfile);
                    if (!string.IsNullOrEmpty(item.Hash))
                    {
                        if (nh != item.Hash)
                            throw new HashMismathcException(string.Format(Strings.Controller.HashMismatchError, tmpfile, item.Hash, nh));
                    }
                    else
                    	item.Hash = nh;
                }

                // Decrypt before returning
                if (!m_options.NoEncryption)
                {
                    try
                    {
                        using(var tmpfile2 = tmpfile)
                        { 
                        	tmpfile = new Library.Utility.TempFile();
                        	m_encryption.Decrypt(tmpfile2, tmpfile);
                        }
                    }
                    catch (Exception ex)
                    {
                        //If we fail here, make sure that we throw a crypto exception
                        if (ex is System.Security.Cryptography.CryptographicException)
                            throw;
                        else
                            throw new System.Security.Cryptography.CryptographicException(ex.Message, ex);
                    }
                }

                item.Result = tmpfile;
                tmpfile = null;
            }
            catch
            {
                if (tmpfile != null)
                    tmpfile.Dispose();

                throw;
            }
        }

        private void DoList(FileEntryItem item)
        {
            m_statwriter.SendEvent(BackendActionType.List, BackendEventType.Started, null, -1);

            var r = m_backend.List();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[");
            long count = 0;
            foreach (var e in r)
            {
                if (count != 0)
                    sb.AppendLine(",");
                count++;
                sb.Append(JsonConvert.SerializeObject(e));
            }

            sb.AppendLine();
            sb.Append("]");
            m_db.LogDbOperation("list", "", sb.ToString());
            item.Result = r;

            m_statwriter.SendEvent(BackendActionType.List, BackendEventType.Completed, null, r.Count);
        }

        private void DoDelete(FileEntryItem item)
        {
            m_statwriter.SendEvent(BackendActionType.Delete, BackendEventType.Started, item.RemoteFilename, item.Size);

            string result = null;
            try
            {
                m_backend.Delete(item.RemoteFilename);
            } 
            catch (Exception ex)
            {
                result = ex.ToString();
                throw;
            }
            finally
            {
                m_db.LogDbOperation("delete", item.RemoteFilename, result);
            }
            
            m_db.LogDbUpdate(item.RemoteFilename, RemoteVolumeState.Deleted, -1, null);
            m_statwriter.SendEvent(BackendActionType.Delete, BackendEventType.Completed, item.RemoteFilename, item.Size);
        }
        
        private void DoCreateFolder(FileEntryItem item)
        {
            m_statwriter.SendEvent(BackendActionType.CreateFolder, BackendEventType.Started, null, -1);

            string result = null;
            try
            {
                m_backend.CreateFolder();
            } 
            catch (Exception ex)
            {
                result = ex.ToString();
                throw;
            }
            finally
            {
                m_db.LogDbOperation("createfolder", item.RemoteFilename, result);
            }
            
            m_statwriter.SendEvent(BackendActionType.CreateFolder, BackendEventType.Completed, null, -1);
        }

        public void Put(VolumeWriterBase item, IndexVolumeWriter indexfile = null)
		{
			if (m_lastException != null)
				throw m_lastException;
            
			item.Close();
			m_db.LogDbUpdate(item.RemoteFilename, RemoteVolumeState.Uploading, -1, null);
			var req = new FileEntryItem(OperationType.Put, item.RemoteFilename, indexfile);
			req.LocalFilename = item.LocalFilename;
			
			if (m_queue.Enqueue(req) && m_options.SynchronousUpload)
			{
				req.WaitForComplete();
				if (req.Exception != null)
					throw req.Exception;
			}
			
			if (m_lastException != null)
				throw m_lastException;

			if (indexfile != null)
			{
				m_db.LogDbUpdate(indexfile.RemoteFilename, RemoteVolumeState.Uploading, -1, null);
				var req2 = new FileEntryItem(OperationType.Put, indexfile.RemoteFilename);
				req2.LocalFilename = indexfile.LocalFilename;

				if (m_queue.Enqueue(req2) && m_options.SynchronousUpload)
				{
					req2.WaitForComplete();
					if (req2.Exception != null)
						throw req2.Exception;
				}
				
				if (m_lastException != null)
					throw m_lastException;
            }
        }

        public Library.Utility.TempFile Get(string remotename, long size, string hash)
		{
			if (m_lastException != null)
				throw m_lastException;

			var req = new FileEntryItem(OperationType.Get, remotename, size, hash);
			if (m_queue.Enqueue(req))
			{
				req.WaitForComplete();
				if (req.Exception != null)
					throw req.Exception;
			}

			if (m_lastException != null)
				throw m_lastException;

            return (Library.Utility.TempFile)req.Result;
        }

        public IDownloadWaitHandle GetAsync(string remotename, long size, string hash)
        {
            if (m_lastException != null)
                throw m_lastException;

            var req = new FileEntryItem(OperationType.Get, remotename, size, hash);
            if (m_queue.Enqueue(req))
                return req;

			if (m_lastException != null)
				throw m_lastException;
            else
                throw new InvalidOperationException("GetAsync called after backend is shut down");
        }

        public IList<Library.Interface.IFileEntry> List()
		{
			if (m_lastException != null)
				throw m_lastException;

			var req = new FileEntryItem(OperationType.List, null);
			if (m_queue.Enqueue(req))
			{
				req.WaitForComplete();
				if (req.Exception != null)
					throw req.Exception;
			}

			if (m_lastException != null)
				throw m_lastException;

            return (IList<Library.Interface.IFileEntry>)req.Result;
        }

        public void WaitForComplete(LocalDatabase db, System.Data.IDbTransaction transation)
        {
        	m_db.FlushDbMessages(db, transation);
            if (m_lastException != null)
                throw m_lastException;

            var item = new FileEntryItem(OperationType.Terminate, null);
            if (m_queue.Enqueue(item))
                item.WaitForComplete();

        	m_db.FlushDbMessages(db, transation);

            if (m_lastException != null)
                throw m_lastException;
        }

        public void CreateFolder(string remotename)
		{
			if (m_lastException != null)
				throw m_lastException;

			var req = new FileEntryItem(OperationType.CreateFolder, remotename);
			if (m_queue.Enqueue(req))
			{
				req.WaitForComplete();
				if (req.Exception != null)
					throw req.Exception;
			}

			if (m_lastException != null)
				throw m_lastException;
        }

        public void Delete(string remotename, bool synchronous = false)
		{
			if (m_lastException != null)
				throw m_lastException;
				
			m_db.LogDbUpdate(remotename, RemoteVolumeState.Deleting, -1, null);
			var req = new FileEntryItem(OperationType.Delete, remotename);
			if (m_queue.Enqueue(req) && synchronous)
			{
				req.WaitForComplete();
				if (req.Exception != null)
					throw req.Exception;
			}

			if (m_lastException != null)
				throw m_lastException;
		}
        
        public bool FlushDbMessages(LocalDatabase database, System.Data.IDbTransaction transaction)
        {
        	return m_db.FlushDbMessages(database, transaction);
        }
        
        public bool FlushDbMessages()
        {
        	return m_db.FlushDbMessages(false);
        }

        public void Dispose()
        {
            if (m_queue != null && !m_queue.Completed)
                m_queue.SetCompleted();

			//TODO: We cannot null this, because it will be recreated
			//Should we wait for queue completion or abort immediately?
            if (m_backend != null)
            {
                m_backend.Dispose();
                m_backend = null;
            }
            
            if (m_thread != null)
            {
                if (!m_thread.Join(TimeSpan.FromSeconds(10)))
                {
                    m_thread.Abort();
                    m_thread.Join(TimeSpan.FromSeconds(10));
                }

                m_thread = null;
            }
            
        	try { m_db.FlushDbMessages(true); }
        	catch (Exception ex) { m_statwriter.AddError(string.Format("Backend Shutdown error: {0}", ex.Message), ex); }
        }
    }
}
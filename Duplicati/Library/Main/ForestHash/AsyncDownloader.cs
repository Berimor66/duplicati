﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Duplicati.Library.Main.ForestHash.Database;

namespace Duplicati.Library.Main.ForestHash
{
    public class AsyncDownloader : IEnumerable<KeyValuePair<IRemoteVolume, Utility.TempFile>>
    {
        private class AsyncDownloaderEnumerator : IEnumerator<KeyValuePair<IRemoteVolume, Utility.TempFile>>
        {
        	private class RemoteVolume : IRemoteVolume
        	{
        		public string Name { get; set; }
        		public string Hash { get; set; }
        		public long Size { get; set; }
        		
        		public RemoteVolume(string name, string hash, long size)
        		{
        			this.Name = name;
        			this.Hash = hash;
        			this.Size = size;
        		}
        	}
        
            private IList<IRemoteVolume> m_volumes;
            private FhBackend.IDownloadWaitHandle m_handle;
            private FhBackend m_backend;
            private int m_index;
            private KeyValuePair<IRemoteVolume, Utility.TempFile>? m_current;

            public AsyncDownloaderEnumerator(IList<IRemoteVolume> volumes, FhBackend backend)
            {
                m_volumes = volumes;
                m_backend = backend;
                m_index = 0;
            }

            public KeyValuePair<IRemoteVolume, Utility.TempFile> Current
            {
                get { return m_current.Value; }
            }

            public void Dispose()
            {
                if (m_current != null)
                {
                    m_current.Value.Value.Dispose();
                    m_current = null;
                }
            }

            object System.Collections.IEnumerator.Current
            {
                get { return this.Current; }
            }

            public bool MoveNext()
            {
                if (m_current != null)
                {
                    m_current.Value.Value.Dispose();
                    m_current = null;
                }

                if (m_index >= m_volumes.Count)
                    return false;

                if (m_handle == null)
                	m_handle = m_backend.GetAsync(m_volumes[m_index].Name, m_volumes[m_index].Size, m_volumes[m_index].Hash);
                
                string hash;
                long size;
                var file = m_handle.Wait(out hash, out size);
                
                m_current = new KeyValuePair<IRemoteVolume, Utility.TempFile>(new RemoteVolume(m_volumes[m_index].Name, hash, size), file);
                m_handle = null;

                m_index++;
                if (m_index < m_volumes.Count)
                    m_handle = m_backend.GetAsync(m_volumes[m_index].Name, m_volumes[m_index].Size, m_volumes[m_index].Hash);

                return true;
            }

            public void Reset()
            {
                throw new NotSupportedException("Cannot reset " + this.GetType().FullName);
            }
        }

        private IList<IRemoteVolume> m_volumes;
        private FhBackend m_backend;

        public AsyncDownloader(IList<IRemoteVolume> volumes, FhBackend backend)
        {
            m_volumes = volumes;
            m_backend = backend;
        }

        public IEnumerator<KeyValuePair<IRemoteVolume, Utility.TempFile>> GetEnumerator()
        {
            return new AsyncDownloaderEnumerator(m_volumes, m_backend);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
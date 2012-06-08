using System;
using System.IO;
using SharpCompress.Common;

namespace Duplicati.Library.Compression
{
    sealed class CompressionEntity : IDisposable
    {
        #region Events

        public delegate MemoryStream CompressionEntityHandler(CompressionEntity sender);
        public event CompressionEntityHandler RequiresStream;

        public delegate void DisposingHandler(CompressionEntity sender);
        public event DisposingHandler StreamDisposing;
        #endregion

        #region Properties

        private StreamWrapper stream { get; set; }
        public StreamWrapper CompressionStream
        {
            get
            {
                //Lazy loading style. Let the host setup our stream
                //This allows us to pick up a stream only when we need it
                if (stream == null && RequiresStream != null)
                {
                    stream = new StreamWrapper(RequiresStream(this));
                    stream.Disposing += StreamIsDisposing;

                    ResetStream();
                }

                return stream;
            }
            set
            {
                stream = value;
                ResetStream();
            }
        }

        public long UnFlushedSize
        {
            get { return stream == null ? 0 : stream.GetSize(); }
        }

        //TODO: Do we need this now the total size calculation is quite accurate?
        private long BaseSize()
        {
            return 46 + 24 + FilePath.Length;
        }
        #endregion

        public string FilePath { get; private set; }
        public DateTime LastModified { get; private set; }

        public CompressionEntity(IEntry entry)
        {
            FilePath = entry.FilePath;
            SetLastModified(entry.LastModifiedTime);
        }

        public CompressionEntity(string filePath, DateTime? lastModified)
        {
            FilePath = filePath;
            SetLastModified(lastModified);
        }

        private void SetLastModified(DateTime? lastModified)
        {
            LastModified = lastModified.HasValue ? lastModified.Value : DateTime.MinValue;
        }

        private void StreamIsDisposing(StreamWrapper sender)
        {
            if (StreamDisposing != null)
                StreamDisposing(this);
        }

        public void ResetStream()
        {
            if (stream.Position > 0 && stream.CanSeek)
                stream.Position = 0;
        }

        public void Dispose()
        {
            if (stream != null)
                stream.Dispose();
        }
    }
}
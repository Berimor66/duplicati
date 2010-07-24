#region Disclaimer / License
// Copyright (C) 2010, Kenneth Skovhede
// http://www.hexad.dk, opensource@hexad.dk
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace Duplicati.Library.Main
{
    //This file contains various classes that represents the 
    // various types of files that exist on a backend.
    //
    //The classes are primarily created from the FilenameStrategy class

    /// <summary>
    /// Sort helper to keep the entries sorted by datetime and volumenumber
    /// </summary>
    internal class Sorter 
        : IComparer<BackupEntryBase>, 
            IComparer<ManifestEntry>, 
            IComparer<PayloadEntryBase>, 
            IComparer<SignatureEntry>, 
            IComparer<ContentEntry>
    {
        #region IComparer<BackupEntry> Members

        public int Compare(BackupEntryBase x, BackupEntryBase y)
        {
            if (x.Time.Equals(y.Time))
            {
                if (x is PayloadEntryBase && y is PayloadEntryBase)
                    return ((PayloadEntryBase)x).Volumenumber.CompareTo(((PayloadEntryBase)y).Volumenumber);
                else
                    return 0;
            }
            else
                return x.Time.CompareTo(y.Time);
        }

        #endregion

        public int Compare(ContentEntry x, ContentEntry y) { return Compare((BackupEntryBase)x, (BackupEntryBase)y); }
        public int Compare(SignatureEntry x, SignatureEntry y) { return Compare((BackupEntryBase)x, (BackupEntryBase)y); }
        public int Compare(PayloadEntryBase x, PayloadEntryBase y) { return Compare((BackupEntryBase)x, (BackupEntryBase)y); }
        public int Compare(ManifestEntry x, ManifestEntry y) { return Compare((BackupEntryBase)x, (BackupEntryBase)y); }
    }


    /// <summary>
    /// An abstract base class for all backup entries
    /// </summary>
    public abstract class BackupEntryBase
    {
        protected string m_filename;
        protected Duplicati.Library.Interface.IFileEntry m_fileentry;
        protected DateTime m_time;
        protected string m_timeString;
        protected string m_encryption;
        protected bool m_isFull;

        /// <summary>
        /// Gets the filename that represents this entry
        /// </summary>
        public string Filename { get { return m_filename; } }
        /// <summary>
        /// Gets the Fileentry that was parsed into this object
        /// </summary>
        public Duplicati.Library.Interface.IFileEntry Fileentry { get { return m_fileentry; } }
        /// <summary>
        /// Gets the backup time that this entry represents
        /// </summary>
        public DateTime Time { get { return m_time; } }
        /// <summary>
        /// Gets the string that was used to parse the date
        /// </summary>
        public string TimeString { get { return m_timeString; } }
        /// <summary>
        /// Gets the encryption mode used, or null if no encryption was applied
        /// </summary>
        public string EncryptionMode { get { return m_encryption; } }
        /// <summary>
        /// Gets a value indicating if this is a full backup entry
        /// </summary>
        public bool IsFull { get { return m_isFull; } }

        protected BackupEntryBase(string filename, Duplicati.Library.Interface.IFileEntry entry, DateTime time, bool isFull, string timeString, string encryption)
        {
            m_filename = filename;
            m_fileentry = entry;
            m_time = time;
            m_isFull = isFull;
            m_timeString = timeString;
            m_encryption = encryption;
        }
    }

    /// <summary>
    /// A class that represents a backup set
    /// </summary>
    public class ManifestEntry : BackupEntryBase
    {
        protected List<KeyValuePair<SignatureEntry, ContentEntry>> m_volumes;
        protected List<ManifestEntry> m_incrementals;
        protected ManifestEntry m_alternate;
        protected bool m_isPrimary;

        /// <summary>
        /// Gets a list of volumes that make up this backup
        /// </summary>
        public List<KeyValuePair<SignatureEntry, ContentEntry>> Volumes { get { return m_volumes; } }
        /// <summary>
        /// Gets a list of incrementals that belong to this entry
        /// </summary>
        public List<ManifestEntry> Incrementals { get { return m_incrementals; } }
        /// <summary>
        /// Gets a value indicating if this manifest is the primary one
        /// </summary>
        public bool IsPrimary { get { return m_isPrimary; } }
        /// <summary>
        /// Gets or sets the alternate manifest file, if avalible
        /// </summary>
        public ManifestEntry Alternate { get { return m_alternate; } set { m_alternate = value; } }

        public ManifestEntry(string filename, Duplicati.Library.Interface.IFileEntry entry, DateTime time, bool isFull, string timeString, string encryption, bool primary)
            : base(filename, entry, time, isFull, timeString, encryption)
        {
            m_volumes = new List<KeyValuePair<SignatureEntry, ContentEntry>>();
            m_incrementals = new List<ManifestEntry>();
            m_isPrimary = primary;
        }

        public ManifestEntry(DateTime time, bool full, bool primary)
            : base(null, null, time, full, null, null)
        {
            m_isPrimary = primary;
        }
    }

    public abstract class PayloadEntryBase : BackupEntryBase
    {
        protected string m_compression;
        protected int m_volumenumber;

        /// <summary>
        /// Gets the volumenumber of the entry
        /// </summary>
        public int Volumenumber { get { return m_volumenumber; } }
        /// <summary>
        /// Gets the compression type applied to the file
        /// </summary>
        public string Compression { get { return m_compression; } }

        protected PayloadEntryBase(string filename, Duplicati.Library.Interface.IFileEntry entry, DateTime time, bool isFull, string timeString, string encryption, string compression, int volumenumber)
            : base(filename, entry, time, isFull, timeString, encryption)
        {
            m_compression = compression;
            m_volumenumber = volumenumber;
        }
    }

    /// <summary>
    /// A class that represents a signature entry
    /// </summary>
    public class SignatureEntry : PayloadEntryBase
    {
        public SignatureEntry(string filename, Duplicati.Library.Interface.IFileEntry entry, DateTime time, bool isFull, string timeString, string encryption, string compression, int volumenumber)
            : base(filename, entry, time, isFull, timeString, encryption, compression, volumenumber)
        {
        }

        public SignatureEntry(DateTime time, bool isFull, int volumenumber)
            : base(null, null, time, isFull, null, null, null, volumenumber)
        {
        }
    }

    /// <summary>
    /// A class that represents a content entry
    /// </summary>
    public class ContentEntry : PayloadEntryBase
    {
        public ContentEntry(string filename, Duplicati.Library.Interface.IFileEntry entry, DateTime time, bool isFull, string timeString, string encryption, string compression, int volumenumber)
            : base(filename, entry, time, isFull, timeString, encryption, compression, volumenumber)
        {
        }

        public ContentEntry(DateTime time, bool isFull, int volumenumber)
            : base(null, null, time, isFull, null, null, null, volumenumber)
        {
        }
    }
}

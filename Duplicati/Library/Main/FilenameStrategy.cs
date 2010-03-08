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
using System.Text.RegularExpressions;

namespace Duplicati.Library.Main
{
    /// <summary>
    /// This class is responsible for naming remote files,
    /// and parsing remote filenames
    /// </summary>
    internal class FilenameStrategy
    {
        /// <summary>
        /// Describes the possible entry types
        /// </summary>
        public enum BackupEntryType
        {
            Manifest,
            Signature,
            Content
        }

        private bool m_useShortFilenames;
        private string m_timeSeperator;
        private string m_prefix;

        private Regex m_filenameRegExp;
        private Regex m_shortRegExp;

        public FilenameStrategy(string prefix, string timeSeperator, bool useShortNames)
        {
            m_prefix = prefix;
            m_timeSeperator = timeSeperator;
            m_useShortFilenames = useShortNames;

            m_shortRegExp = new Regex(@"(?<prefix>" + Regex.Escape(m_prefix) + @")\-(?<type>(C|S|M))(?<inc>(F|I))(?<time>([A-F]|[a-f]|[0-9])+)\.(?<volumegroup>vol(?<volumenumber>\d+)\.)?(?<extension>.+)");
            m_filenameRegExp = new Regex(@"(?<prefix>" + Regex.Escape(m_prefix) + @")\-(?<inc>(full|inc))\-(?<type>(content|signature|manifest))\.(?<time>\d{4}\-\d{2}\-\d{2}.\d{2}" + Regex.Escape(m_timeSeperator) + @"\d{2}" + Regex.Escape(m_timeSeperator) + @"\d{2}(?<timezone>([\+\-]\d{2}" + Regex.Escape(m_timeSeperator) + @"\d{2})|Z)?)\.(?<volumegroup>vol(?<volumenumber>\d+)\.)?(?<extension>.+)");
        }

        public FilenameStrategy(Options options)
            : this(options.BackupPrefix, options.TimeSeperatorChar, options.UseShortFilenames)
        {
        }

        public string GenerateFilename(BackupEntryBase entry)
        {
            if (entry is PayloadEntryBase)
                return GenerateFilename(entry is SignatureEntry ? BackupEntryType.Signature : BackupEntryType.Content, entry.IsFull, entry.Time, ((PayloadEntryBase)entry).Volumenumber);
            else
                return GenerateFilename(BackupEntryType.Manifest, entry.IsFull, entry.Time);
        }

        public string GenerateFilename(BackupEntryType type, bool full, DateTime time, int volume)
        {
            return GenerateFilename(type, full, time) + ".vol" + volume.ToString();
        }

        public string GenerateFilename(BackupEntryType type, bool full, DateTime time)
        {
            return GenerateFilename(type, full, m_useShortFilenames, time);
        }

        public string GenerateFilename(BackupEntryType type, bool full, bool shortName, DateTime time)
        {
            string t;
            if (type == BackupEntryType.Manifest)
                t = shortName ? "M" : "manifest";
            else if (type == BackupEntryType.Content)
                t = shortName ? "C" : "content";
            else if (type == BackupEntryType.Signature)
                t = shortName ? "S" : "signature";
            else
                throw new Exception(string.Format(Strings.FilenameStrategy.InvalidEntryTypeError, type));

            if (!shortName)
            {
                string datetime = time.ToString("yyyy-MM-ddTHH:mm:ssK").Replace(":", m_timeSeperator);
                return m_prefix + "-" + (full ? "full" : "inc") + "-" + t + "." + datetime;
            }
            else
            {
                return m_prefix + "-" + t + (full ? "F" : "I") + (time.ToUniversalTime().Ticks / TimeSpan.TicksPerSecond).ToString("X");
            }
        }

        public BackupEntryBase DecodeFilename(Duplicati.Library.Backend.FileEntry fe)
        {
            Match m = m_filenameRegExp.Match(fe.Name);
            if (!m.Success)
                m = m_shortRegExp.Match(fe.Name);
            if (!m.Success)
                return null;
            if (m.Value != fe.Name)
                return null; //Accept only full matches

            BackupEntryType type;
            if (m.Groups["type"].Value == "manifest" || m.Groups["type"].Value == "M")
                type = BackupEntryType.Manifest;
            else if (m.Groups["type"].Value == "content" || m.Groups["type"].Value == "C")
                type = BackupEntryType.Content;
            else if (m.Groups["type"].Value == "signature" || m.Groups["type"].Value == "S")
                type = BackupEntryType.Signature;
            else
                return null;

            bool isFull = m.Groups["inc"].Value == "full" || m.Groups["inc"].Value == "F";
            bool isShortName = m.Groups["inc"].Value.Length == 1;
            string timeString = m.Groups["time"].Value;
            DateTime time;
            if (isShortName)
                time = new DateTime(long.Parse(timeString, System.Globalization.NumberStyles.HexNumber) * TimeSpan.TicksPerSecond, DateTimeKind.Utc).ToLocalTime();
            else
                time = DateTime.Parse(timeString.Replace(m_timeSeperator, ":"));

            string extension = m.Groups["extension"].Value;

            int volNumber = -1;
            if (m.Groups["volumenumber"].Success)
                volNumber = int.Parse(m.Groups["volumenumber"].Value);

            string compression = extension;
            string encryption = null;

            int dotIndex = compression.IndexOf(".");
            if (dotIndex > 0)
            {
                encryption = compression.Substring(dotIndex + 1);
                compression = compression.Substring(0, dotIndex);
            }

            if (type == BackupEntryType.Manifest)
                return new ManifestEntry(fe.Name, fe, time, isFull, timeString, encryption);
            else if (type == BackupEntryType.Signature)
                return new SignatureEntry(fe.Name, fe, time, isFull, timeString, encryption, compression, volNumber);
            else if (type == BackupEntryType.Content)
                return new ContentEntry(fe.Name, fe, time, isFull, timeString, encryption, compression, volNumber);
            else
                return null;
        }

        public bool UseShortNames { get { return m_useShortFilenames; } }
        public string Prefix { get { return m_prefix; } }
    }
}

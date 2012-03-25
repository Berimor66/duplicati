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
using Duplicati.Library.Interface;
using System.IO;
using System.Security.Cryptography;

namespace Duplicati.Library.Encryption
{
    /// <summary>
    /// A dummy class for the 7-Zip compression. Password is handled within the compression module
    /// </summary>
    public class SevenZipEncryption : EncryptionBase, IEncryptionGUI, IGUIMiniControl
    {
        /// <summary>
        /// Default constructor, used to read file extension and supported commands
        /// </summary>
        public SevenZipEncryption()
        {
        }

        /// <summary>
        /// A dummy class for the 7-Zip compression. Password is handled within the compression module
        /// </summary>
        public SevenZipEncryption(string passphrase, Dictionary<string, string> options)
        {
            if (!(options.ContainsKey("compression-module") && options["compression-module"].ToLower() == FilenameExtension.ToLower()))
                throw new Exception(Strings.SevenZipEncryption.SetupError);
        }



        #region IEncryption Members

        public override string FilenameExtension { get { return "7z"; } }
        public override string Description { get { return Strings.SevenZipEncryption.Description; } }
        public override string DisplayName { get { return Strings.SevenZipEncryption.DisplayName; } }
        protected override void Dispose(bool disposing) { }

        public override long SizeOverhead(long filesize)
        {
            return base.SizeOverhead(1);
        }

        public override Stream Encrypt(Stream input)
        {
            return input;
        }

        public override Stream Decrypt(Stream input)
        {
            return input;
        }

        public override IList<ICommandLineArgument> SupportedCommands
        {
            get
            {
                return new List<ICommandLineArgument>();
            }
        }

        #endregion

        #region IGUIControl Members

        public string PageTitle
        {
            get { return this.DisplayName; }
        }

        public string PageDescription
        {
            get { return this.Description; }
        }

        public System.Windows.Forms.Control GetControl(IDictionary<string, string> applicationSettings, IDictionary<string, string> options)
        {
            return new System.Windows.Forms.Control();
        }

        public void Leave(System.Windows.Forms.Control control)
        {
        }

        public bool Validate(System.Windows.Forms.Control control)
        {
            return true;
        }

        public string GetConfiguration(IDictionary<string, string> applicationSettings, IDictionary<string, string> guiOptions, IDictionary<string, string> commandlineOptions)
        {
            return null;
        }

        #endregion
    }
}

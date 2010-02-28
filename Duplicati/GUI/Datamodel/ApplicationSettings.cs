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
using System.Data.LightDatamodel;

namespace Duplicati.Datamodel
{
    public class ApplicationSettings
    {
        private SettingsHelper<ApplicationSetting, string, string> m_appset;

        private const string RECENT_DURATION = "Recent duration";
        private const string PGP_PATH = "PGP path";
        private const string SFTP_PATH = "SFTP Path";
        private const string TEMP_PATH = "Temp Path";

        private const string USE_COMMON_PASSWORD = "Use common password";
        private const string COMMON_PASSWORD_USE_GPG = "Use PGP with common password";
        private const string COMMON_PASSWORD = "Common password";

        private const string SIGNATURE_CACHE_PATH = "Signature Cache Path";
        private const string SIGNATURE_CACHE_ENABLED = "Signature Cache Enabled";

        private const string DISPLAY_LANGUAGE = "Display Language";

        //This is only used on windows, on linux, it is assumed that the programs are in the path env.
        public const string PROGRAM_FILES = "%PROGRAMFILES%";

        public const string STARTUP_DELAY_DURATION = "Startup delay duration";
        public const string THREAD_PRIORITY_OVERRIDE = "Thread priority override";
        public const string UPLOAD_SPEED_LIMIT = "Upload speed limit";
        public const string DOWNLOAD_SPEED_LIMIT = "Download speed limit";

        public ApplicationSettings(IDataFetcher dataparent)
        {
            m_appset = new SettingsHelper<ApplicationSetting, string, string>(dataparent, new List<ApplicationSetting>(dataparent.GetObjects<ApplicationSetting>()), "Name", "Value");
        }

        /// <summary>
        /// Creates a detached copy of all the application settings
        /// </summary>
        /// <returns>A dictionary with application settings</returns>
        public Dictionary<string, string> CreateDetachedCopy()
        {
            //We need to read/write all values to make sure they have the propper defaults
            foreach (System.Reflection.PropertyInfo pi in this.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.DeclaredOnly))
                if (pi.CanRead && pi.CanWrite)
                    pi.SetValue(this, pi.GetValue(this, null), null);

            //After that, simply copy the values over
            Dictionary<string, string> res = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> k in m_appset)
                res[k.Key] = k.Value;
            return res;
        }

        /// <summary>
        /// Gets or sets the amount of time a log entry will be visible in the list of recent backups
        /// </summary>
        public string RecentBackupDuration
        {
            get { return string.IsNullOrEmpty(m_appset[RECENT_DURATION]) ? "2W" : m_appset[RECENT_DURATION]; }
            set { m_appset[RECENT_DURATION] = value; }
        }

        /// <summary>
        /// Gets or sets the path to PGP. May contain environment variables
        /// </summary>
        public string GPGPath
        {
            get 
            { 
                if (string.IsNullOrEmpty(m_appset[PGP_PATH]))
                {
                    if (Library.Core.Utility.IsClientLinux)
                        return "gpg";
                    else
                        return System.IO.Path.Combine(PROGRAM_FILES, "GNU\\GnuPG\\gpg.exe");
                }

                return m_appset[PGP_PATH];
            }
            set { m_appset[PGP_PATH] = value; }
        }

        /// <summary>
        /// Gets or sets the path to SFtp. May contain environment variables
        /// </summary>
        public string SFtpPath
        {
            get
            {
                if (string.IsNullOrEmpty(m_appset[SFTP_PATH]))
                {
                    if (Library.Core.Utility.IsClientLinux)
                        return "sftp";
                    else
                        return System.IO.Path.Combine(PROGRAM_FILES, "putty\\psftp.exe");
                }
                return m_appset[SFTP_PATH];
            }
            set { m_appset[SFTP_PATH] = value; }
        }

        /// <summary>
        /// Gets or sets the path to store temporary files. May contain environment variables
        /// </summary>
        public string TempPath
        {
            get
            {
                if (string.IsNullOrEmpty(m_appset[TEMP_PATH]))
                {
#if DEBUG
                    return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "TEMP");
#else
                    return System.IO.Path.GetTempPath();
#endif
                }
                else
                    return m_appset[TEMP_PATH];
            }
            set { m_appset[TEMP_PATH] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating if Duplicati should apply a common password to backups
        /// </summary>
        public bool UseCommonPassword
        {
            get
            {
                bool res;
                if (bool.TryParse(m_appset[USE_COMMON_PASSWORD], out res))
                    return res;
                else
                    return false;
            }
            set { m_appset[USE_COMMON_PASSWORD] = value.ToString(); }
        }

        /// <summary>
        /// Gets or sets a value indicating if GPG should be used to encrypt passwords
        /// </summary>
        public bool CommonPasswordUseGPG
        {
            get
            {
                bool res;
                if (bool.TryParse(m_appset[COMMON_PASSWORD_USE_GPG], out res))
                    return res;
                else
                    return false;
            }
            set { m_appset[COMMON_PASSWORD_USE_GPG] = value.ToString(); }
        }

        /// <summary>
        /// Gets or sets the common password applied to encrypt files.
        /// </summary>
        public string CommonPassword
        {
            get
            {
                if (string.IsNullOrEmpty(m_appset[COMMON_PASSWORD]))
                    return "";
                else
                    return m_appset[COMMON_PASSWORD];
            }
            set { m_appset[COMMON_PASSWORD] = value; }
        }

        /// <summary>
        /// Gets or sets the path to store signature cache files. May contain environment variables
        /// </summary>
        public string SignatureCachePath
        {
            get
            {
                if (string.IsNullOrEmpty(m_appset[SIGNATURE_CACHE_PATH]))
                {
#if DEBUG
                    return System.IO.Path.Combine(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "TEMP"), "Duplicati Signature Cache");
#else
                    return System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Duplicati Signature Cache");
#endif
                }
                else
                    return m_appset[SIGNATURE_CACHE_PATH];
            }
            set { m_appset[SIGNATURE_CACHE_PATH] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating if the signature cache should be used.
        /// </summary>
        public bool SignatureCacheEnabled
        {
            get
            {
                bool res;
                if (bool.TryParse(m_appset[SIGNATURE_CACHE_ENABLED], out res))
                    return res;
                else
                    return true;
            }
            set { m_appset[SIGNATURE_CACHE_ENABLED] = value.ToString(); }
        }

        /// <summary>
        /// Gets or sets the language used in the user interface
        /// </summary>
        public string DisplayLanguage
        {
            get
            {
                if (string.IsNullOrEmpty(m_appset[DISPLAY_LANGUAGE]))
                    return "";
                else
                    return m_appset[DISPLAY_LANGUAGE];
            }
            set { m_appset[DISPLAY_LANGUAGE] = value; }
        }

        /// <summary>
        /// Gets or sets the amount of time Duplicati will be paused after startup
        /// </summary>
        public string StartupDelayDuration
        {
            get { return string.IsNullOrEmpty(m_appset[STARTUP_DELAY_DURATION]) ? "5m" : m_appset[STARTUP_DELAY_DURATION]; }
            set { m_appset[STARTUP_DELAY_DURATION] = value; }
        }

        /// <summary>
        /// Gets or sets the thread priority used for the backup thread
        /// </summary>
        public System.Threading.ThreadPriority? ThreadPriorityOverride
        {
            get
            {
                if (string.IsNullOrEmpty(m_appset[THREAD_PRIORITY_OVERRIDE]))
                    return null;
                else
                    return Library.Core.Utility.ParsePriority(m_appset[THREAD_PRIORITY_OVERRIDE]);
            }
            set 
            {
                if (value == null)
                    m_appset[THREAD_PRIORITY_OVERRIDE] = "";
                else
                    m_appset[THREAD_PRIORITY_OVERRIDE] = value.Value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the upper limit for download speeds
        /// </summary>
        public string DownloadSpeedLimit
        {
            get { return m_appset[DOWNLOAD_SPEED_LIMIT]; }
            set { m_appset[DOWNLOAD_SPEED_LIMIT] = value == null ? "" : value; }
        }

        /// <summary>
        /// Gets or sets the upper limit for upload speeds
        /// </summary>
        public string UploadSpeedLimit
        {
            get { return m_appset[UPLOAD_SPEED_LIMIT]; }
            set { m_appset[UPLOAD_SPEED_LIMIT] = value == null ? "" : value; }
        }

    }
}

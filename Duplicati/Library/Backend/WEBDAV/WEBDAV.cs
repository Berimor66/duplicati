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

namespace Duplicati.Library.Backend
{
    public class WEBDAV : IBackend, IStreamingBackend, IBackendGUI
    {
        private System.Net.NetworkCredential m_userInfo;
        private string m_url;
        private string m_path;
        private string m_rawurl;
        Dictionary<string, string> m_options;
        private bool m_useIntegratedAuthentication = false;
        private bool m_forceDigestAuthentication = false;
        private bool m_acceptAllCertificates = false;
        private string m_acceptCertificateHash = null;
        private bool m_useSSL = false;

        /// <summary>
        /// A list of files seen in the last List operation.
        /// It is used to detect a problem with IIS where a file is list,
        /// but IIS responds 404 because the file mapping is incorrect.
        /// </summary>
        private List<string> m_filenamelist = null;

        private static readonly byte[] PROPFIND = System.Text.Encoding.UTF8.GetBytes("<?xml version=\"1.0\"?><D:propfind xmlns:D=\"DAV:\"><D:allprop/></D:propfind>");

        public WEBDAV()
        {
        }

        public WEBDAV(string url, Dictionary<string, string> options)
        {
            Uri u = new Uri(url);

            if (!string.IsNullOrEmpty(u.UserInfo))
            {
                m_userInfo = new System.Net.NetworkCredential();
                if (u.UserInfo.IndexOf(":") >= 0)
                {
                    m_userInfo.UserName = u.UserInfo.Substring(0, u.UserInfo.IndexOf(":"));
                    m_userInfo.Password = u.UserInfo.Substring(u.UserInfo.IndexOf(":") + 1);
                }
                else
                {
                    m_userInfo.UserName = u.UserInfo;
                    if (options.ContainsKey("ftp-password"))
                        m_userInfo.Password = options["ftp-password"];
                }
            }
            else
            {
                if (options.ContainsKey("ftp-username"))
                {
                    m_userInfo = new System.Net.NetworkCredential();
                    m_userInfo.UserName = options["ftp-username"];
                    if (options.ContainsKey("ftp-password"))
                        m_userInfo.Password = options["ftp-password"];
                }
            }

            m_options = options;
            m_useIntegratedAuthentication = m_options.ContainsKey("integrated-authentication");
            m_forceDigestAuthentication = m_options.ContainsKey("force-digest-authentication");
            m_useSSL = m_options.ContainsKey("use-ssl");
            m_acceptAllCertificates = m_options.ContainsKey("accept-any-ssl-certificate");
            if (m_options.ContainsKey("accept-specified-ssl-hash"))
                m_acceptCertificateHash = m_options["accept-specified-ssl-hash"];

            m_url = (m_useSSL ? "https" : "http") + url.Substring(u.Scheme.Length);
            if (!m_url.EndsWith("/"))
                m_url += "/";

            m_path = new Uri(m_url).PathAndQuery;
            if (m_path.IndexOf("?") > 0)
                m_path = m_path.Substring(0, m_path.IndexOf("?"));
            
            m_path = System.Web.HttpUtility.UrlDecode(m_path);
            m_rawurl = (m_useSSL ? "https://" : "http://") + u.Host + m_path;
        }

        #region IBackend Members

        public string DisplayName
        {
            get { return Strings.WEBDAV.DisplayName; }
        }

        public string ProtocolKey
        {
            get { return "webdav"; }
        }

        public List<FileEntry> List()
        {
            using (ActivateCertificateValidator())
            {
                System.Net.HttpWebRequest req = CreateRequest("");

                req.Method = "PROPFIND";
                req.Headers.Add("Depth", "1");
                req.ContentType = "text/xml";
                req.ContentLength = PROPFIND.Length;

                using (System.IO.Stream s = req.GetRequestStream())
                    s.Write(PROPFIND, 0, PROPFIND.Length);

                try
                {
                    System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                    using (System.Net.HttpWebResponse resp = (System.Net.HttpWebResponse)req.GetResponse())
                    {
                        int code = (int)resp.StatusCode;
                        if (code < 200 || code >= 300) //For some reason Mono does not throw this automatically
                            throw new System.Net.WebException(resp.StatusDescription, null, System.Net.WebExceptionStatus.ProtocolError, resp);

                        doc.Load(resp.GetResponseStream());
                    }

                    System.Xml.XmlNamespaceManager nm = new System.Xml.XmlNamespaceManager(doc.NameTable);
                    nm.AddNamespace("D", "DAV:");

                    List<FileEntry> files = new List<FileEntry>();
                    m_filenamelist = new List<string>();

                    foreach (System.Xml.XmlNode n in doc.SelectNodes("D:multistatus/D:response/D:href", nm))
                    {
                        string name = System.Web.HttpUtility.UrlDecode(n.InnerText);

                        string cmp_path;

                        if (name.StartsWith(m_url))
                            cmp_path = m_url;
                        else if (name.StartsWith(m_rawurl))
                            cmp_path = m_rawurl;
                        else if (name.StartsWith(m_path))
                            cmp_path = m_path;
                        else
                            continue;

                        if (name.Length <= cmp_path.Length)
                            continue;

                        name = name.Substring(cmp_path.Length);

                        long size = 0;
                        DateTime lastAccess = new DateTime();
                        DateTime lastModified = new DateTime();
                        bool isCollection = false;

                        System.Xml.XmlNode stat = n.ParentNode.SelectSingleNode("D:propstat/D:prop", nm);
                        if (stat != null)
                        {
                            System.Xml.XmlNode s = stat.SelectSingleNode("D:getcontentlength", nm);
                            if (s != null)
                                size = long.Parse(s.InnerText);
                            s = stat.SelectSingleNode("D:getlastmodified", nm);
                            if (s != null)
                                try
                                {
                                    //Not important if this succeeds
                                    lastAccess = lastModified = DateTime.Parse(s.InnerText, System.Globalization.CultureInfo.InvariantCulture);
                                }
                                catch { }

                            s = stat.SelectSingleNode("D:iscollection", nm);
                            if (s != null)
                                isCollection = s.InnerText.Trim() == "1";
                            else
                                isCollection = (stat.SelectSingleNode("D:resourcetype/D:collection", nm) != null);
                        }

                        FileEntry fe = new FileEntry(name, size, lastAccess, lastModified);
                        fe.IsFolder = isCollection;
                        files.Add(fe);
                        m_filenamelist.Add(name);
                    }

                    return files;
                }
                catch (System.Net.WebException wex)
                {
                    if (wex.Response as System.Net.HttpWebResponse != null && (wex.Response as System.Net.HttpWebResponse).StatusCode == System.Net.HttpStatusCode.NotFound)
                        throw new Backend.FolderMissingException(string.Format(Strings.WEBDAV.MissingFolderError, m_path, wex.Message), wex);
                    else
                        throw;
                }
            }
        }

        public void CreateFolder()
        {
            using (ActivateCertificateValidator())
            {
                System.Net.HttpWebRequest req = CreateRequest("");
                req.Method = System.Net.WebRequestMethods.Http.MkCol;
                req.KeepAlive = false;
                using (req.GetResponse())
                { }
            }
        }

        public void Put(string remotename, string filename)
        {
            using (System.IO.FileStream fs = System.IO.File.OpenRead(filename))
                Put(remotename, fs);
        }

        public void Get(string remotename, string filename)
        {
            using (System.IO.FileStream fs = System.IO.File.Create(filename))
                Get(remotename, fs);
        }

        public void Delete(string remotename)
        {
            using (ActivateCertificateValidator())
            {
                System.Net.HttpWebRequest req = CreateRequest(remotename);
                req.Method = "DELETE";
                using (req.GetResponse())
                { }
            }
        }

        public IList<ICommandLineArgument> SupportedCommands
        {
            get 
            {
                return new List<ICommandLineArgument>(new ICommandLineArgument[] {
                    new CommandLineArgument("ftp-password", CommandLineArgument.ArgumentType.String, Strings.WEBDAV.DescriptionFTPPasswordShort, Strings.WEBDAV.DescriptionFTPPasswordLong),
                    new CommandLineArgument("ftp-username", CommandLineArgument.ArgumentType.String, Strings.WEBDAV.DescriptionFTPUsernameShort, Strings.WEBDAV.DescriptionFTPUsernameLong),
                    new CommandLineArgument("integrated-authentication", CommandLineArgument.ArgumentType.Boolean, Strings.WEBDAV.DescriptionIntegratedAuthenticationShort, Strings.WEBDAV.DescriptionIntegratedAuthenticationLong),
                    new CommandLineArgument("force-digest-authentication", CommandLineArgument.ArgumentType.Boolean, Strings.WEBDAV.DescriptionForceDigestShort, Strings.WEBDAV.DescriptionForceDigestLong),
                    new CommandLineArgument("use-ssl", CommandLineArgument.ArgumentType.Boolean, Strings.WEBDAV.DescriptionUseSSLShort, Strings.WEBDAV.DescriptionUseSSLLong),
                    new CommandLineArgument("accept-specified-ssl-hash", CommandLineArgument.ArgumentType.String, Strings.WEBDAV.DescriptionAcceptHashShort, Strings.WEBDAV.DescriptionAcceptHashLong),
                    new CommandLineArgument("accept-any-ssl-certificate", CommandLineArgument.ArgumentType.Boolean, Strings.WEBDAV.DescriptionAcceptAnyCertificateShort, Strings.WEBDAV.DescriptionAcceptAnyCertificateLong),
                });
            }
        }

        public string Description
        {
            get { return Strings.WEBDAV.Description; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion

        private IDisposable ActivateCertificateValidator()
        {
            return m_useSSL ? 
                new Core.SslCertificateValidator(m_acceptAllCertificates, m_acceptCertificateHash) : 
                null;
        }

        private System.Net.HttpWebRequest CreateRequest(string remotename)
        {
            System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(m_url + System.Web.HttpUtility.UrlEncode(remotename));
            if (m_useIntegratedAuthentication)
                req.UseDefaultCredentials = true;
            else if (m_forceDigestAuthentication)
            {
                System.Net.CredentialCache cred = new System.Net.CredentialCache();
                cred.Add(new Uri(m_url), "Digest", m_userInfo);
                req.Credentials = cred;
            }
            else
			{
                req.Credentials = m_userInfo;
                if (Library.Core.Utility.IsClientLinux)
					req.PreAuthenticate = true; //We need this under Mono for some reason
			}

            req.KeepAlive = false;
            req.UserAgent = "Duplicati WEBDAV Client";

            return req;
        }
        

        #region IStreamingBackend Members

        public bool SupportsStreaming
        {
            get { return true; }
        }

        public void Put(string remotename, System.IO.Stream stream)
        {
            using (ActivateCertificateValidator())
            {
                System.Net.HttpWebRequest req = CreateRequest(remotename);
                req.Method = System.Net.WebRequestMethods.Http.Put;
                req.ContentType = "application/binary";

                try { req.ContentLength = stream.Length; }
                catch { }

                using (System.IO.Stream s = req.GetRequestStream())
                    Core.Utility.CopyStream(stream, s);

                using (System.Net.HttpWebResponse resp = (System.Net.HttpWebResponse)req.GetResponse())
                {
                    int code = (int)resp.StatusCode;
                    if (code < 200 || code >= 300) //For some reason Mono does not throw this automatically
                        throw new System.Net.WebException(resp.StatusDescription, null, System.Net.WebExceptionStatus.ProtocolError, resp);
                }
            }
        }

        public void Get(string remotename, System.IO.Stream stream)
        {
            using (ActivateCertificateValidator())
            {
                System.Net.HttpWebRequest req = CreateRequest(remotename);
                req.Method = System.Net.WebRequestMethods.Http.Get;

                try
                {
                    using (System.Net.HttpWebResponse resp = (System.Net.HttpWebResponse)req.GetResponse())
                    {
                        int code = (int)resp.StatusCode;
                        if (code < 200 || code >= 300) //For some reason Mono does not throw this automatically
                            throw new System.Net.WebException(resp.StatusDescription, null, System.Net.WebExceptionStatus.ProtocolError, resp);

                        using (System.IO.Stream s = resp.GetResponseStream())
                            Core.Utility.CopyStream(s, stream);
                    }
                }
                catch (System.Net.WebException wex)
                {
                    if (
                        wex.Response as System.Net.HttpWebResponse != null
                        &&
                        (wex.Response as System.Net.HttpWebResponse).StatusCode == System.Net.HttpStatusCode.NotFound
                        &&
                        m_filenamelist != null
                        &&
                        m_filenamelist.Contains(remotename)
                    )
                        throw new Exception(string.Format(Strings.WEBDAV.SeenThenNotFoundError, m_path, remotename, System.IO.Path.GetExtension(remotename), wex.Message), wex);
                    else
                        throw;
                }
            }
        }

        #endregion

        #region IBackendGUI Members

        public string PageTitle
        {
            get { return WebDAVUI.PageTitle; }
        }

        public string PageDescription
        {
            get { return WebDAVUI.PageDescription; }
        }

        public System.Windows.Forms.Control GetControl(IDictionary<string, string> applicationSettings, IDictionary<string, string> options)
        {
            return new WebDAVUI(options);
        }

        public void Leave(System.Windows.Forms.Control control)
        {
            ((WebDAVUI)control).Save(false);
        }

        public bool Validate(System.Windows.Forms.Control control)
        {
            return ((WebDAVUI)control).Save(true);
        }

        public string GetConfiguration(IDictionary<string, string> applicationSettings, IDictionary<string, string> guiOptions, IDictionary<string, string> commandlineOptions)
        {
            return WebDAVUI.GetConfiguration(guiOptions, commandlineOptions);
        }

        #endregion
    }
}

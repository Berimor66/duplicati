#region Disclaimer / License
// Copyright (C) 2009, Kenneth Skovhede
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

namespace Duplicati.Library.Backend
{
    public class S3 : IStreamingBackend
    {
        private string m_awsID;
        private string m_awsKey;
        private string m_url;
        private string m_host;
        private string m_bucket;
        private string m_prefix;
        private Affirma.ThreeSharp.CallingFormat m_format;
        private bool m_euBuckets;

        Dictionary<string, string> m_options;

        public S3()
        {
        }

        public S3(string url, Dictionary<string, string> options)
        {
            Uri u = new Uri(url);

            if (!string.IsNullOrEmpty(u.UserInfo))
            {
                if (u.UserInfo.IndexOf(":") >= 0)
                {
                    m_awsID = u.UserInfo.Substring(0, u.UserInfo.IndexOf(":"));
                    m_awsKey = u.UserInfo.Substring(u.UserInfo.IndexOf(":") + 1);
                }
                else
                {
                    m_awsID = u.UserInfo;
                    if (options.ContainsKey("aws_secret_access_key"))
                        m_awsKey = options["aws_secret_access_key"];
                    else if (options.ContainsKey("ftp-password"))
                        m_awsKey = options["ftp-password"];
                }
            }
            else
            {
                if (options.ContainsKey("ftp-username"))
                    m_awsID = options["ftp-username"];
                if (options.ContainsKey("ftp-password"))
                    m_awsKey = options["ftp-password"];

                if (options.ContainsKey("aws_access_key_id"))
                    m_awsID = options["aws_access_key_id"];
                if (options.ContainsKey("aws_secret_access_key"))
                    m_awsKey = options["aws_secret_access_key"];
            }

            if (string.IsNullOrEmpty(m_awsID))
                throw new Exception(Strings.S3Backend.NoAMZUserIDError);
            if (string.IsNullOrEmpty(m_awsKey))
                throw new Exception(Strings.S3Backend.NoAMZKeyError);


            m_prefix = "";

            m_host = u.Host;
            m_format = options.ContainsKey("s3-use-new-style") ? Affirma.ThreeSharp.CallingFormat.SUBDOMAIN : Affirma.ThreeSharp.CallingFormat.REGULAR;
            m_euBuckets = options.ContainsKey("s3-european-buckets");

            if (m_host.ToLower() == "s3.amazonaws.com")
            {
                m_bucket = u.PathAndQuery;

                if (m_bucket.Contains("/"))
                {
                    m_prefix = m_bucket.Substring(m_bucket.IndexOf("/") + 1);
                    m_bucket = m_bucket.Substring(0, m_bucket.IndexOf("/"));
                }

            }
            else 
            {
                //Vanity style, do a CNAME lookup
                if (!m_host.ToLower().EndsWith(".s3.amazonaws.com"))
                {
                    System.Net.IPHostEntry ent = System.Net.Dns.GetHostEntry(m_host);
                    List<string> entries = new List<string>();
                    entries.AddRange(ent.Aliases);

                    foreach (string s in entries)
                        if (s.ToLower().EndsWith(".s3.amazonaws.com"))
                        {
                            m_host = s;
                            break;
                        }
                }

                if (m_host.ToLower().EndsWith(".s3.amazonaws.com"))
                {
                    m_format = Affirma.ThreeSharp.CallingFormat.SUBDOMAIN;
                    m_bucket = m_host.Substring(0, m_host.Length - ".s3.amazonaws.com".Length);
                    m_host = "s3.amazonaws.com";
                    m_prefix = u.PathAndQuery;

                    if (m_prefix.StartsWith("/"))
                        m_prefix = m_prefix.Substring(1);
                }
                else
                    throw new Exception(string.Format(Strings.S3Backend.UnableToDecodeBucketnameError, m_url));
            }


            m_options = options;
            m_url = url;
            m_prefix = m_prefix.Trim();
            if (m_prefix.Length != 0 && !m_prefix.EndsWith("/"))
                m_prefix += "/";
        }

        #region IBackendInterface Members

        public string DisplayName
        {
            get { return Strings.S3Backend.DisplayName; }
        }

        public string ProtocolKey
        {
            get { return "s3"; }
        }

        public bool SupportsStreaming
        {
            get { return true; }
        }

        public List<FileEntry> List()
        {

            try
            {
                S3Wrapper con = CreateRequest();

                List<FileEntry> lst = con.ListBucket(m_bucket, m_prefix);
                for (int i = 0; i < lst.Count; i++)
                {
                    lst[i].Name = lst[i].Name.Substring(m_prefix.Length);
                    
                    //Fix for a bug in Duplicati 1.0 beta 3 and earlier, where filenames are incorrectly prefixed with a slash
                    if (lst[i].Name.StartsWith("/") && !m_prefix.StartsWith("/"))
                        lst[i].Name = lst[i].Name.Substring(1);
                }
                return lst;
            }
            catch (System.Net.WebException wex)
            {
                //Catch "non-existing" buckets
                if (wex.Status == System.Net.WebExceptionStatus.ProtocolError)
                    if (wex.Response is System.Net.HttpWebResponse && ((System.Net.HttpWebResponse)wex.Response).StatusCode == System.Net.HttpStatusCode.NotFound)
                        return new List<FileEntry>();

                throw;
            }
        }

        public void Put(string remotename, string localname)
        {
            using (System.IO.FileStream fs = System.IO.File.Open(localname, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
                Put(remotename, fs);
        }

        public void Put(string remotename, System.IO.Stream input)
        {
            S3Wrapper con = CreateRequest();
            try
            {
                con.AddFileStream(m_bucket, GetFullKey(remotename), input);
            }
            catch (System.Net.WebException wex)
            {
                if (wex.Status == System.Net.WebExceptionStatus.ProtocolError)
                {
                    if (wex.Response is System.Net.HttpWebResponse && ((System.Net.HttpWebResponse)wex.Response).StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        //Perhaps the bucket needs to be created?
                        try
                        {
                            con.AddBucket(m_bucket);
                            con.AddFileStream(m_bucket, m_prefix + remotename, input);
                            return;
                        }
                        catch
                        {
                        }

                        throw;
                    }
                    else
                        throw;
                }
            }
        }

        public void Get(string remotename, string localname)
        {
            using (System.IO.FileStream fs = System.IO.File.Open(localname, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None))
                Get(remotename, fs);
        }

        public void Get(string remotename, System.IO.Stream output)
        {
            S3Wrapper con = CreateRequest();
            try
            {
                con.GetFileStream(m_bucket, GetFullKey(remotename), output);
            }
            catch
            {
                //This is a fix for the S3 backend prior to beta 3, where the filenames had a slash suffix
                bool fallbackFix = false;
                try
                {
                    if (!remotename.StartsWith("/"))
                        con.GetFileStream(m_bucket, GetFullKey("/" + remotename), output);
                    fallbackFix = true;
                }
                catch
                {
                }

                if (!fallbackFix)
                    throw;
            }
        }

        public void Delete(string remotename)
        {
            S3Wrapper con = CreateRequest();
            con.DeleteObject(m_bucket, GetFullKey(remotename));
        }

        public IList<ICommandLineArgument> SupportedCommands
        {
            get
            {
                return new List<ICommandLineArgument>(new ICommandLineArgument[] {
                    new CommandLineArgument("aws_secret_access_key", CommandLineArgument.ArgumentType.Path, Strings.S3Backend.AMZKeyDescriptionShort, Strings.S3Backend.AMZKeyDescriptionLong, null, new string[] {"ftp-password"}, null),
                    new CommandLineArgument("aws_access_key_id", CommandLineArgument.ArgumentType.Path, Strings.S3Backend.AMZUserIDDescriptionShort, Strings.S3Backend.AMZUserIDDescriptionLong, null, new string[] {"ftp-username"}, null),
                    new CommandLineArgument("s3-use-new-style", CommandLineArgument.ArgumentType.Boolean, Strings.S3Backend.S3NewStyleDescriptionShort, Strings.S3Backend.S3NewStyleDescriptionLong, "true"),
                    new CommandLineArgument("s3-european-buckets", CommandLineArgument.ArgumentType.Boolean, Strings.S3Backend.S3EurobucketDescriptionShort, Strings.S3Backend.S3EurobucketDescriptionLong, "false"),
                    new CommandLineArgument("ftp-password", CommandLineArgument.ArgumentType.String, Strings.S3Backend.FTPPasswordDescriptionShort, Strings.S3Backend.FTPPasswordDescriptionLong),
                    new CommandLineArgument("ftp-username", CommandLineArgument.ArgumentType.String, Strings.S3Backend.DescriptionFTPUsernameShort, Strings.S3Backend.DescriptionFTPUsernameLong)
                });

            }
        }

        public string Description
        {
            get
            {
                return Strings.S3Backend.Description;
            }
        }
        
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (m_options != null)
                m_options = null;
            if (m_awsID != null)
                m_awsID = null;
            if (m_awsKey != null)
                m_awsKey = null;
        }

        #endregion

        private S3Wrapper CreateRequest()
        {
            return new S3Wrapper(m_awsID, m_awsKey, m_format, m_euBuckets);
        }

        private string GetFullKey(string name)
        {
            //Url encode special chars, but keep slashes
            return System.Web.HttpUtility.UrlEncode(m_prefix + name).Replace("%2f", "/");
        }

    }
}

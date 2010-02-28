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
    public class CommunicationStatistics
    {
        private long m_numberOfBytesUploaded;
        private long m_numberOfRemoteCalls;
        private long m_numberOfBytesDownloaded;

        private long m_numberOfErrors;
        private StringBuilder m_errorMessages = new StringBuilder();

        public long NumberOfBytesUploaded
        {
            get { return m_numberOfBytesUploaded; }
            set { m_numberOfBytesUploaded = value; }
        }

        public long NumberOfBytesDownloaded
        {
            get { return m_numberOfBytesDownloaded; }
            set { m_numberOfBytesDownloaded = value; }
        }

        public long NumberOfRemoteCalls
        {
            get { return m_numberOfRemoteCalls; }
            set { m_numberOfRemoteCalls = value; }
        }

        public void LogError(string errorMessage)
        {
            m_numberOfErrors++;
            m_errorMessages.AppendLine(errorMessage);
        }

        public override string ToString()
        {
            //TODO: Figure out how to translate this without breaking the output parser
            StringBuilder sb = new StringBuilder();
            sb.Append("BytesUploaded   : " + this.NumberOfBytesUploaded.ToString(System.Globalization.CultureInfo.InvariantCulture) + "\r\n");
            sb.Append("BytesDownloaded : " + this.NumberOfBytesDownloaded.ToString(System.Globalization.CultureInfo.InvariantCulture) + "\r\n");
            sb.Append("RemoteCalls     : " + this.NumberOfRemoteCalls.ToString(System.Globalization.CultureInfo.InvariantCulture) + "\r\n");

            if (m_numberOfErrors > 0)
            {
                sb.Append("NumberOfErrors  : " + m_numberOfErrors .ToString(System.Globalization.CultureInfo.InvariantCulture) + "\r\n");
                sb.Append("****************\r\n");
                sb.Append(m_errorMessages.ToString());
                sb.Append("****************\r\n");
            }

            return sb.ToString();
        }
    }
}

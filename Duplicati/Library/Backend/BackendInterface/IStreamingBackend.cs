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

namespace Duplicati.Library.Backend
{
    /// <summary>
    /// An interface a backend may implement if it supports streaming operations.
    /// </summary>
    public interface IStreamingBackend : IBackend
    {
        /// <summary>
        /// A value that indicates if the backend supports streaming.
        /// Backends that implement the IStreamingBackend interface should return true.
        /// </summary>
        bool SupportsStreaming { get; }

        /// <summary>
        /// Puts the content of the file to the url passed
        /// </summary>
        /// <param name="remotename">The remote filename, relative to the URL</param>
        /// <param name="stream">The stream to read from</param>
        void Put(string remotename, System.IO.Stream stream);

        /// <summary>
        /// Downloads a file with the remote data
        /// </summary>
        /// <param name="remotename">The remote filename, relative to the URL</param>
        /// <param name="stream">The stream to write data to</param>
        void Get(string remotename, System.IO.Stream stream);

    }
}

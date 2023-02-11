//
// Source.cs
//
// Author: Kees van Spelde <sicos2002@hotmail.com>
//
// Copyright (c) 2014-2023 Magic-Sessions. (www.magic-sessions.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NON INFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using System;
using VCardReader.Collections;

namespace VCardReader
{
    /// <summary>
    ///     A source of directory information for a vCard.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A source identifies a directory that contains or provided information for the vCard.
    ///         A source consists of a URI and a context. The URI is generally a URL; the context identifies
    ///         the protocol and type of URI. For example, a vCard associated with an LDAP directory entry
    ///         will have an ldap:// URL and a context of "LDAP".
    ///     </para>
    /// </remarks>
    /// <seealso cref="SourceCollection" />
    public class Source
    {
        #region Fields
        private string _context;
        #endregion

        #region Context
        /// <summary>
        ///     The context of the source URI.
        /// </summary>
        /// <remarks>
        ///     The context identifies how the URI should be interpreted.  Example is "LDAP", which indicates
        ///     the URI is an LDAP reference.
        /// </remarks>
        public string Context
        {
            get { return _context ?? string.Empty; }
            set { _context = value; }
        }
        #endregion

        #region Uri
        /// <summary>
        ///     The URI of the source.
        /// </summary>
        public Uri Uri { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        ///     Initializes a new instance of the vCardSource class.
        /// </summary>
        public Source()
        {
            _context = string.Empty;
        }

        /// <summary>
        ///     Initializes a new source with the specified URI.
        /// </summary>
        /// <param name="uri">
        ///     The URI of the directory entry.
        /// </param>
        public Source(Uri uri)
        {
            Uri = uri;
        }

        /// <summary>
        ///     Initializes a new source with the specified context and URI.
        /// </summary>
        /// <param name="uri">
        ///     The URI of the source of the vCard data.
        /// </param>
        /// <param name="context">
        ///     The context of the source.
        /// </param>
        public Source(Uri uri, string context)
        {
            _context = context;
            Uri = uri;
        }
        #endregion
    }
}
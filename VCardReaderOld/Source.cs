using System;
using VCardReader.Collections;

/*
   Copyright 2014-2016 Kees van Spelde

   Licensed under The Code Project Open License (CPOL) 1.02;
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

     http://www.codeproject.com/info/cpol10.aspx

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

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
//
// Website.cs
//
// Author: Kees van Spelde <sicos2002@hotmail.com>
//
// Copyright (c) 2014-2021 Magic-Sessions. (www.magic-sessions.com)
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
    #region Public enum WebsiteTypes
    /// <summary>
    ///     The type or classification of a web site.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The Microsoft Outlook contact editor has a field for entering a web site. The default classification of
    ///         this web site is work-related. A personal web site can be viewed or entered through the All Fields tab.
    ///     </para>
    /// </remarks>
    [Flags]
    public enum WebsiteTypes
    {
        /// <summary>
        ///     No web site designation.
        /// </summary>
        Default = 0,

        /// <summary>
        ///     A personal home page.
        /// </summary>
        Personal = 1,

        /// <summary>
        ///     A work-related web site.
        /// </summary>
        Work = 2
    }
    #endregion
    
    /// <summary>
    ///     A web site defined in a vCard.
    /// </summary>
    /// <seealso cref="WebsiteCollection" />
    /// <seealso cref="WebsiteTypes" />
    public class Website
    {
        #region Fields
        private string _url;
        #endregion

        #region IsPersonalSite
        /// <summary>
        ///     Indicates a personal home page.
        /// </summary>
        public bool IsPersonalSite
        {
            get
            {
                return (WebsiteType & WebsiteTypes.Personal) ==
                       WebsiteTypes.Personal;
            }
            set
            {
                if (value)
                    WebsiteType |= WebsiteTypes.Personal;
                else
                    WebsiteType &= ~WebsiteTypes.Personal;
            }
        }
        #endregion

        #region IsWorkSite
        /// <summary>
        ///     Indicates a work-related web site.
        /// </summary>
        public bool IsWorkSite
        {
            get
            {
                return (WebsiteType & WebsiteTypes.Work) ==
                       WebsiteTypes.Work;
            }
            set
            {
                if (value)
                    WebsiteType |= WebsiteTypes.Work;
                else
                    WebsiteType &= ~WebsiteTypes.Work;
            }
        }
        #endregion

        #region Url
        /// <summary>
        ///     The URL of the web site.
        /// </summary>
        /// <remarks>
        ///     The format of the URL is not validated.
        /// </remarks>
        public string Url
        {
            get { return _url; }
            set { _url = value ?? string.Empty; }
        }
        #endregion

        #region WebsiteType
        /// <summary>
        ///     The type of web site (e.g. home page, work, etc).
        /// </summary>
        public WebsiteTypes WebsiteType { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        ///     Creates a vCardWebSite object.
        /// </summary>
        public Website()
        {
            _url = string.Empty;
        }

        /// <summary>
        ///     Creates a new vCardWebSite object with the specified URL.
        /// </summary>
        /// <param name="url">
        ///     The URL of the web site.
        /// </param>
        public Website(string url)
        {
            _url = url ?? string.Empty;
        }

        /// <summary>
        ///     Creates a new vCardWebSite with the
        ///     specified URL and classification.
        /// </summary>
        /// <param name="url">
        ///     The URL of the web site.
        /// </param>
        /// <param name="websiteType">
        ///     The classification of the web site.
        /// </param>
        public Website(string url, WebsiteTypes websiteType)
        {
            _url = url ?? string.Empty;
            WebsiteType = websiteType;
        }
        #endregion

        #region ToString
        /// <summary>
        ///     Returns the string representation (URL) of the web site.
        /// </summary>
        /// <returns>
        ///     The URL of the web site.
        /// </returns>
        public override string ToString()
        {
            return _url;
        }
        #endregion
    }
}
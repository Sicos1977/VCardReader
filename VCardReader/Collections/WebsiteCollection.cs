//
// WebsiteCollection.cs
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

using System.Collections.ObjectModel;

namespace VCardReader.Collections
{
    /// <summary>
    ///     A collection of <see cref="Website" /> objects.
    /// </summary>
    /// <seealso cref="Website" />
    /// <seealso cref="WebsiteTypes" />
    public class WebsiteCollection : Collection<Website>
    {
        #region GetFirstChoice
        /// <summary>
        ///     Returns the first web site of the specified type.  If
        ///     the collection does not contain a website of the specified
        ///     type, but does contain a default (uncategorized) website,
        ///     then that website will be returned.
        /// </summary>
        /// <param name="siteType"></param>
        /// <returns></returns>
        // ReSharper disable once UnusedMember.Global
        public Website GetFirstChoice(WebsiteTypes siteType)
        {
            Website alternate = null;

            foreach (var webSite in this)
            {
                if ((webSite.WebsiteType & siteType) == siteType)
                    return webSite;

                if ((alternate == null) &&
                    (webSite.WebsiteType == WebsiteTypes.Default))
                    alternate = webSite;
            }
            return alternate;
        }
        #endregion
    }
}
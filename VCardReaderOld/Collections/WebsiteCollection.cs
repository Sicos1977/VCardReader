using System.Collections.ObjectModel;

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
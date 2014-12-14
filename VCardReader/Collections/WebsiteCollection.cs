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
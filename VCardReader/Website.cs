using VCardReader.Collections;

namespace VCardReader
{
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
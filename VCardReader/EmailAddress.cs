using System;
using VCardReader.Collections;

namespace VCardReader
{
    #region Public enum EmailAddressType
    /// <summary>
    ///     Identifies the type of email address in a vCard.
    /// </summary>
    /// <seealso cref="EmailAddress" />
    [Flags]
    public enum EmailAddressType
    {
        /// <summary>
        ///     An Internet (SMTP) mail (default) address.
        /// </summary>
        Internet = 0,

        /// <summary>
        ///     An America On-Line email address.
        /// </summary>
        AOl,

        /// <summary>
        ///     An AppleLink email address.
        /// </summary>
        AppleLink,

        /// <summary>
        ///     An AT&amp;T Mail email address
        /// </summary>
        AttMail,

        /// <summary>
        ///     A CompuServe Information Service (CIS) email address.
        /// </summary>
        CompuServe,

        /// <summary>
        ///     An eWorld email address.
        /// </summary>
        /// <remarks>
        ///     eWorld was an online service by Apple Computer in the mid 1990s.
        ///     It was officially shut down on March 31, 1996.
        /// </remarks>
        EWorld,

        // ReSharper disable InconsistentNaming
        /// <summary>
        ///     An IBM Mail email address.
        /// </summary>
        IBMMail,

        /// <summary>
        ///     An MCI Mail email address.
        /// </summary>
        MCIMail,
        // ReSharper restore InconsistentNaming

        /// <summary>
        ///     A PowerShare email address.
        /// </summary>
        PowerShare,

        /// <summary>
        ///     A Prodigy Information Service email address.
        /// </summary>
        Prodigy,

        /// <summary>
        ///     A telex email address.
        /// </summary>
        Telex,

        /// <summary>
        ///     An X.400 service email address.
        /// </summary>
        X400
    }
    #endregion

    /// <summary>
    ///     An email address in a <see cref="VCard" />.
    /// </summary>
    /// <remarks>
    ///     Most vCard email addresses are Internet email addresses. However, the vCard specification allows other
    ///     email address formats, such as CompuServe and X400. Unless otherwise specified, an address is assumed
    ///     to be an Internet address.
    /// </remarks>
    /// <seealso cref="EmailAddressCollection" />
    /// <seealso cref="EmailAddressType" />
    public class EmailAddress
    {
        #region Fields
        private string _address;
        #endregion

        #region Constructors
        /// <summary>
        ///     Creates a new <see cref="EmailAddress" />.
        /// </summary>
        public EmailAddress()
        {
            _address = string.Empty;
            EmailType = EmailAddressType.Internet;
        }

        /// <summary>
        ///     Creates a new Internet <see cref="EmailAddress" />.
        /// </summary>
        /// <param name="address">
        ///     The Internet email address.
        /// </param>
        public EmailAddress(string address)
        {
            _address = address ?? string.Empty;
            EmailType = EmailAddressType.Internet;
        }

        /// <summary>
        ///     Creates a new <see cref="EmailAddress" /> of the specified type.
        /// </summary>
        /// <param name="address">
        ///     The email address.
        /// </param>
        /// <param name="emailType">
        ///     The type of email address.
        /// </param>
        public EmailAddress(string address, EmailAddressType emailType)
        {
            _address = address;
            EmailType = emailType;
        }
        #endregion

        #region Address
        /// <summary>
        ///     The email address.
        /// </summary>
        /// <remarks>
        ///     The format of the email address is not validated by the class.
        /// </remarks>
        public string Address
        {
            get { return _address ?? string.Empty; }
            set { _address = value; }
        }
        #endregion

        #region EmailType
        /// <summary>
        ///     The email address type.
        /// </summary>
        public EmailAddressType EmailType { get; set; }
        #endregion

        #region IsPreferred
        /// <summary>
        ///     Indicates a preferred (top priority) email address.
        /// </summary>
        public bool IsPreferred { get; set; }
        #endregion

        #region ToString
        /// <summary>
        ///     Builds a string that represents the email address.
        /// </summary>
        public override string ToString()
        {
            return _address;
        }
        #endregion
    }
}
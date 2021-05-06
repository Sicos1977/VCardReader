//
// EmailAddress.cs
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
        AOl = 1,

        /// <summary>
        ///     An AppleLink email address.
        /// </summary>
        AppleLink = 2,

        /// <summary>
        ///     An AT&amp;T Mail email address
        /// </summary>
        AttMail = 4,

        /// <summary>
        ///     A CompuServe Information Service (CIS) email address.
        /// </summary>
        CompuServe = 8,

        /// <summary>
        ///     An eWorld email address.
        /// </summary>
        /// <remarks>
        ///     eWorld was an online service by Apple Computer in the mid 1990s.
        ///     It was officially shut down on March 31, 1996.
        /// </remarks>
        EWorld = 16,

        // ReSharper disable InconsistentNaming
        /// <summary>
        ///     An IBM Mail email address.
        /// </summary>
        IBMMail = 32,

        /// <summary>
        ///     An MCI Mail email address.
        /// </summary>
        MCIMail = 64,
        // ReSharper restore InconsistentNaming

        /// <summary>
        ///     A PowerShare email address.
        /// </summary>
        PowerShare = 128,

        /// <summary>
        ///     A Prodigy Information Service email address.
        /// </summary>
        Prodigy = 256,

        /// <summary>
        ///     A telex email address.
        /// </summary>
        Telex = 512,

        /// <summary>
        ///     An X.400 service email address.
        /// </summary>
        X400 = 1024
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
//
// Phone.cs
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
    #region Public enum PhoneTypes
    /// <summary>
    ///     Identifies different phone types (e.g. Fax, BBS, etc).
    /// </summary>
    /// <seealso cref="Phone" />
    /// <seealso cref="PhoneCollection" />
    [Flags]
    public enum PhoneTypes
    {
        /// <summary>
        ///     Indicates default properties.
        /// </summary>
        Default = 0,

        /// <summary>
        ///     Indicates a bulletin board system.
        /// </summary>
        Bbs = 1,

        /// <summary>
        ///     Indicates a car phone.
        /// </summary>
        Car = 2,

        /// <summary>
        ///     Indicates a car phone.
        /// </summary>
        CarVoice = Car + Voice,

        /// <summary>
        ///     Indicates a cell phone.
        /// </summary>
        Cellular = 4,

        /// <summary>
        ///     Indicates a celluar voice number.
        /// </summary>
        CellularVoice = Cellular + Voice,

        /// <summary>
        ///     Indicates a facsimile number.
        /// </summary>
        Fax = 8,

        /// <summary>
        ///     Indicates a home number
        /// </summary>
        Home = 16,

        /// <summary>
        ///     Indicates a home and voice number.
        /// </summary>
        HomeVoice = Home + Voice,

        /// <summary>
        ///     Indicates an ISDN number.
        /// </summary>
        Isdn = 32,

        /// <summary>
        ///     Indicates a messaging service on the number.
        /// </summary>
        MessagingService = 64,

        /// <summary>
        ///     Indicates a MODEM number.
        /// </summary>
        Modem = 128,

        /// <summary>
        ///     Indicates a pager number.
        /// </summary>
        Pager = 256,

        /// <summary>
        ///     Indicates a preferred number.
        /// </summary>
        Preferred = 512,

        /// <summary>
        ///     Indicates a video number.
        /// </summary>
        Video = 1024,

        /// <summary>
        ///     Indicates a voice number.
        /// </summary>
        Voice = 2048,

        /// <summary>
        ///     Indicates a work number.
        /// </summary>
        Work = 4096,

        /// <summary>
        ///     Indicates a company numbe
        /// </summary>
        /// <remarks>
        ///     Only present when the contact card is made by Microsoft Outlook
        /// </remarks>
        Company = 8192,

        /// <summary>
        ///     Indicates a work number (Outlook VCARD only).
        /// </summary>
        /// <remarks>
        ///     Only present when the contact card is made by Microsoft Outlook
        /// </remarks>
        Callback = 16384,

        /// <summary>
        ///     Indicates a work number.
        /// </summary>
        /// <remarks>
        ///     Only present when the contact card is made by Microsoft Outlook
        /// </remarks>
        Radio = 32768,

        /// <summary>
        ///     Indicates a work number.
        /// </summary>
        /// <remarks>
        ///     Only present when the contact card is made by Microsoft Outlook
        /// </remarks>
        Assistant = 65536,

        /// <summary>
        ///     Indicates a work number.
        /// </summary>
        /// <remarks>
        ///     Only present when the contact card is made by Microsoft Outlook
        /// </remarks>
        Ttytdd = 131072,

        /// <summary>
        ///     Indicates a pager number
        /// </summary>
        /// <remarks>
        ///     Only present when the contact card is made by Microsoft Outlook
        /// </remarks>
        VoicePager = Voice + Pager,

        /// <summary>
        ///     Indicates a voice assistant number
        /// </summary>
        /// <remarks>
        ///     Only present when the contact card is made by Microsoft Outlook
        /// </remarks>
        VoiceAssistant = Voice + Assistant,

        /// <summary>
        ///     Indicates a voice company number
        /// </summary>
        /// <remarks>
        ///     Only present when the contact card is made by Microsoft Outlook
        /// </remarks>
        VoiceCompany = Voice + Company,

        /// <summary>
        ///     Indicates a voice callback number
        /// </summary>
        /// <remarks>
        ///     Only present when the contact card is made by Microsoft Outlook
        /// </remarks>
        VoiceCallback = Voice + Callback,

        /// <summary>
        ///     Indicates a voice radio number
        /// </summary>
        /// <remarks>
        ///     Only present when the contact card is made by Microsoft Outlook
        /// </remarks>
        VoiceRadio = Voice + Radio,

        /// <summary>
        ///     Indicates a voice fax number
        /// </summary>
        WorkFax = Work + Fax,

        /// <summary>
        ///     Indicates a work and voice number.
        /// </summary>
        WorkVoice = Work + Voice,

        /// <summary>
        ///     Indicates a home fax number
        /// </summary>
        HomeFax = Home + Fax
    }
    #endregion

    /// <summary>
    ///     Telephone information for a <see cref="VCard" />.
    /// </summary>
    /// <seealso cref="PhoneCollection" />
    /// <seealso cref="PhoneTypes" />
    [Serializable]
    public class Phone
    {
        #region Fields
        private string _fullNumber;
        private PhoneTypes _phoneType;
        #endregion

        #region Constructors
        /// <summary>
        ///     Creates a new <see cref="Phone" /> object.
        /// </summary>
        public Phone()
        {
        }

        /// <summary>
        ///     Creates a new <see cref="Phone" /> object with the specified number.
        /// </summary>
        /// <param name="fullNumber">
        ///     The phone number.
        /// </param>
        public Phone(string fullNumber)
        {
            _fullNumber = fullNumber;
        }


        /// <summary>
        ///     Creates a new <see cref="Phone" /> with the specified number and subtype.
        /// </summary>
        /// <param name="fullNumber">The phone number.</param>
        /// <param name="phoneType">The phone subtype.</param>
        public Phone(string fullNumber, PhoneTypes phoneType)
        {
            _fullNumber = fullNumber;
            _phoneType = phoneType;
        }
        #endregion

        #region FullNumber
        /// <summary>
        ///     The full telephone number.
        /// </summary>
        public string FullNumber
        {
            get { return _fullNumber ?? string.Empty; }
            set { _fullNumber = value; }
        }
        #endregion

        #region PhoneType
        /// <summary>
        ///     The phone subtype.
        /// </summary>
        public PhoneTypes PhoneType
        {
            get { return _phoneType; }
            set { _phoneType = value; }
        }
        #endregion
    }
}
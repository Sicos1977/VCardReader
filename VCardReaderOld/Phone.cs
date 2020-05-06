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
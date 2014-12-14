using System;
using VCardReader.Collections;

namespace VCardReader
{
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

        #region IsBbs
        /// <summary>
        ///     Indicates a BBS number.
        /// </summary>
        /// <seealso cref="IsModem" />
        /// <seealso cref="PhoneTypes" />
        public bool IsBbs
        {
            get { return (_phoneType & PhoneTypes.Bbs) == PhoneTypes.Bbs; }
            set
            {
                if (value)
                    _phoneType = _phoneType | PhoneTypes.Bbs;
                else
                    _phoneType = _phoneType & ~PhoneTypes.Bbs;
            }
        }
        #endregion

        #region IsCar
        /// <summary>
        ///     Indicates a car number.
        /// </summary>
        /// <seealso cref="PhoneTypes" />
        public bool IsCar
        {
            get { return (_phoneType & PhoneTypes.Car) == PhoneTypes.Car; }
            set
            {
                if (value)
                    _phoneType = _phoneType | PhoneTypes.Car;
                else
                    _phoneType = _phoneType & ~PhoneTypes.Car;
            }
        }
        #endregion

        #region IsCellular
        /// <summary>
        ///     Indicates a cellular number.
        /// </summary>
        /// <seealso cref="PhoneTypes" />
        public bool IsCellular
        {
            get { return (_phoneType & PhoneTypes.Cellular) == PhoneTypes.Cellular; }
            set
            {
                if (value)
                    _phoneType = _phoneType | PhoneTypes.Cellular;
                else
                    _phoneType = _phoneType & ~PhoneTypes.Cellular;
            }
        }
        #endregion

        #region IsFax
        /// <summary>
        ///     Indicates a fax number.
        /// </summary>
        /// <seealso cref="PhoneTypes" />
        public bool IsFax
        {
            get { return (_phoneType & PhoneTypes.Fax) == PhoneTypes.Fax; }
            set
            {
                if (value)
                    _phoneType = _phoneType | PhoneTypes.Fax;
                else
                    _phoneType = _phoneType & ~PhoneTypes.Fax;
            }
        }
        #endregion

        #region IsHome
        /// <summary>
        ///     Indicates a home number.
        /// </summary>
        /// <seealso cref="IsWork" />
        /// <seealso cref="PhoneTypes" />
        public bool IsHome
        {
            get { return (_phoneType & PhoneTypes.Home) == PhoneTypes.Home; }
            set
            {
                if (value)
                    _phoneType = _phoneType | PhoneTypes.Home;
                else
                    _phoneType = _phoneType & ~PhoneTypes.Home;
            }
        }
        #endregion

        #region IsIsdn
        /// <summary>
        ///     Indicates an ISDN number.
        /// </summary>
        /// <seealso cref="PhoneTypes" />
        public bool IsIsdn
        {
            get { return (_phoneType & PhoneTypes.Isdn) == PhoneTypes.Isdn; }
            set
            {
                if (value)
                    _phoneType = _phoneType | PhoneTypes.Isdn;
                else
                    _phoneType = _phoneType & ~PhoneTypes.Isdn;
            }
        }
        #endregion

        #region IsMessagingService
        /// <summary>
        ///     Indicates a messaging service number.
        /// </summary>
        /// <seealso cref="PhoneTypes" />
        public bool IsMessagingService
        {
            get
            {
                return (_phoneType & PhoneTypes.MessagingService) ==
                       PhoneTypes.MessagingService;
            }
            set
            {
                if (value)
                    _phoneType = _phoneType | PhoneTypes.MessagingService;
                else
                    _phoneType = _phoneType & ~PhoneTypes.MessagingService;
            }
        }
        #endregion

        #region IsModem
        /// <summary>
        ///     Indicates a modem number.
        /// </summary>
        /// <seealso cref="IsBbs" />
        /// <seealso cref="PhoneTypes" />
        public bool IsModem
        {
            get { return (_phoneType & PhoneTypes.Modem) == PhoneTypes.Modem; }
            set
            {
                if (value)
                    _phoneType = _phoneType | PhoneTypes.Modem;
                else
                    _phoneType = _phoneType & ~PhoneTypes.Modem;
            }
        }
        #endregion

        #region IsPager
        /// <summary>
        ///     Indicates a pager number.
        /// </summary>
        /// <seealso cref="PhoneTypes" />
        public bool IsPager
        {
            get { return (_phoneType & PhoneTypes.Pager) == PhoneTypes.Pager; }
            set
            {
                if (value)
                    _phoneType = _phoneType | PhoneTypes.Pager;
                else
                    _phoneType = _phoneType & ~PhoneTypes.Pager;
            }
        }
        #endregion

        #region IsPreferred
        /// <summary>
        ///     Indicates a preferred number.
        /// </summary>
        /// <seealso cref="PhoneTypes" />
        public bool IsPreferred
        {
            get { return (_phoneType & PhoneTypes.Preferred) == PhoneTypes.Preferred; }
            set
            {
                if (value)
                    _phoneType = _phoneType | PhoneTypes.Preferred;
                else
                    _phoneType = _phoneType & ~PhoneTypes.Preferred;
            }
        }
        #endregion

        #region IsVideo
        /// <summary>
        ///     Indicates a video number.
        /// </summary>
        /// <seealso cref="PhoneTypes" />
        public bool IsVideo
        {
            get { return (_phoneType & PhoneTypes.Video) == PhoneTypes.Video; }
            set
            {
                if (value)
                    _phoneType = _phoneType | PhoneTypes.Video;
                else
                    _phoneType = _phoneType & ~PhoneTypes.Video;
            }
        }
        #endregion

        #region IsVoice
        /// <summary>
        ///     Indicates a voice number.
        /// </summary>
        /// <seealso cref="PhoneTypes" />
        public bool IsVoice
        {
            get { return (_phoneType & PhoneTypes.Voice) == PhoneTypes.Voice; }
            set
            {
                if (value)
                    _phoneType = _phoneType | PhoneTypes.Voice;
                else
                    _phoneType = _phoneType & ~PhoneTypes.Voice;
            }
        }
        #endregion

        #region IsWork
        /// <summary>
        ///     Indicates a work number.
        /// </summary>
        /// <seealso cref="IsHome" />
        /// <seealso cref="PhoneTypes" />
        public bool IsWork
        {
            get { return (_phoneType & PhoneTypes.Work) == PhoneTypes.Work; }
            set
            {
                if (value)
                    _phoneType = _phoneType | PhoneTypes.Work;
                else
                    _phoneType = _phoneType & ~PhoneTypes.Work;
            }
        }
        #endregion

        #region PhoneType
        /// <summary>
        ///     The phone subtype.
        /// </summary>
        /// <seealso cref="IsVideo" />
        /// <seealso cref="IsVoice" />
        /// <seealso cref="IsWork" />
        public PhoneTypes PhoneType
        {
            get { return _phoneType; }
            set { _phoneType = value; }
        }
        #endregion
    }
}
using System;
using VCardReader.Collections;

namespace VCardReader
{
    #region Public enum DeliveryAddressTypes
    /// <summary>
    ///     The type of a delivery address.
    /// </summary>
    [Flags]
    public enum DeliveryAddressTypes
    {
        /// <summary>
        ///     Default address settings.
        /// </summary>
        Default = 0,

        /// <summary>
        ///     A domestic delivery address.
        /// </summary>
        Domestic,

        /// <summary>
        ///     An international delivery address.
        /// </summary>
        International,

        /// <summary>
        ///     A postal delivery address.
        /// </summary>
        Postal,

        /// <summary>
        ///     A parcel delivery address.
        /// </summary>
        Parcel,

        /// <summary>
        ///     A home delivery address.
        /// </summary>
        Home,

        /// <summary>
        ///     A work delivery address.
        /// </summary>
        Work
    }
    #endregion

    /// <summary>
    ///     A postal address.
    /// </summary>
    /// <seealso cref="DeliveryAddressCollection" />
    [Serializable]
    public class DeliveryAddress
    {
        #region Fields
        private string _city;
        private string _country;
        private string _postalCode;
        private string _region;
        private string _street;
        #endregion

        #region DeliveryAddress
        /// <summary>
        ///     Creates a new delivery address object.
        /// </summary>
        public DeliveryAddress()
        {
            _city = string.Empty;
            _country = string.Empty;
            _postalCode = string.Empty;
            _region = string.Empty;
            _street = string.Empty;
        }
        #endregion

        #region AddressType
        /// <summary>
        ///     The type of postal address.
        /// </summary>
        public DeliveryAddressTypes AddressType { get; set; }
        #endregion

        #region City
        /// <summary>
        ///     The city or locality of the address.
        /// </summary>
        public string City
        {
            get { return _city ?? string.Empty; }
            set { _city = value; }
        }
        #endregion

        #region Country
        /// <summary>
        ///     The country name of the address.
        /// </summary>
        public string Country
        {
            get { return _country ?? string.Empty; }
            set { _country = value; }
        }
        #endregion

        #region IsDomestic
        /// <summary>
        ///     Indicates a domestic delivery address.
        /// </summary>
        public bool IsDomestic
        {
            get
            {
                return (AddressType & DeliveryAddressTypes.Domestic) ==
                       DeliveryAddressTypes.Domestic;
            }
            set
            {
                if (value)
                {
                    AddressType |= DeliveryAddressTypes.Domestic;
                }
                else
                {
                    AddressType &= ~DeliveryAddressTypes.Domestic;
                }
            }
        }
        #endregion

        #region IsHome
        /// <summary>
        ///     Indicates a home address.
        /// </summary>
        public bool IsHome
        {
            get
            {
                return (AddressType & DeliveryAddressTypes.Home) ==
                       DeliveryAddressTypes.Home;
            }
            set
            {
                if (value)
                {
                    AddressType |= DeliveryAddressTypes.Home;
                }
                else
                {
                    AddressType &= ~DeliveryAddressTypes.Home;
                }
            }
        }
        #endregion

        #region IsInternational
        /// <summary>
        ///     Indicates an international address.
        /// </summary>
        public bool IsInternational
        {
            get
            {
                return (AddressType & DeliveryAddressTypes.International) ==
                       DeliveryAddressTypes.International;
            }
            set
            {
                if (value)
                {
                    AddressType |= DeliveryAddressTypes.International;
                }
                else
                {
                    AddressType &= ~DeliveryAddressTypes.International;
                }
            }
        }
        #endregion

        #region IsParcel
        /// <summary>
        ///     Indicates a parcel delivery address.
        /// </summary>
        public bool IsParcel
        {
            get
            {
                return (AddressType & DeliveryAddressTypes.Parcel) ==
                       DeliveryAddressTypes.Parcel;
            }
            set
            {
                if (value)
                {
                    AddressType |= DeliveryAddressTypes.Parcel;
                }
                else
                {
                    AddressType &= ~DeliveryAddressTypes.Parcel;
                }
            }
        }
        #endregion

        #region IsPostal
        /// <summary>
        ///     Indicates a postal address.
        /// </summary>
        public bool IsPostal
        {
            get
            {
                return (AddressType & DeliveryAddressTypes.Postal) ==
                       DeliveryAddressTypes.Postal;
            }
            set
            {
                if (value)
                {
                    AddressType |= DeliveryAddressTypes.Postal;
                }
                else
                {
                    AddressType &= ~DeliveryAddressTypes.Postal;
                }
            }
        }
        #endregion

        #region IsWork
        /// <summary>
        ///     Indicates a work address.
        /// </summary>
        public bool IsWork
        {
            get
            {
                return (AddressType & DeliveryAddressTypes.Work) ==
                       DeliveryAddressTypes.Work;
            }
            set
            {
                if (value)
                {
                    AddressType |= DeliveryAddressTypes.Work;
                }
                else
                {
                    AddressType &= ~DeliveryAddressTypes.Work;
                }
            }
        }
        #endregion

        #region PostalCode
        /// <summary>
        ///     The postal code (e.g. ZIP code) of the address.
        /// </summary>
        public string PostalCode
        {
            get { return _postalCode ?? string.Empty; }
            set { _postalCode = value; }
        }
        #endregion

        #region Region
        /// <summary>
        ///     The region (state or province) of the address.
        /// </summary>
        public string Region
        {
            get { return _region ?? string.Empty; }
            set { _region = value; }
        }
        #endregion

        #region Street
        /// <summary>
        ///     The street of the delivery address.
        /// </summary>
        public string Street
        {
            get { return _street ?? string.Empty; }
            set { _street = value; }
        }
        #endregion
    }
}
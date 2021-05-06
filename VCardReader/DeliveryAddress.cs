//
// DeliveryAddress.cs
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
        Domestic = 1,

        /// <summary>
        ///     An international delivery address.
        /// </summary>
        International = 2,

        /// <summary>
        ///     A postal delivery address.
        /// </summary>
        Postal = 4,

        /// <summary>
        ///     A parcel delivery address.
        /// </summary>
        Parcel = 8,

        /// <summary>
        ///     A home delivery address.
        /// </summary>
        Home = 16,

        /// <summary>
        ///     A work delivery address.
        /// </summary>
        Work = 32
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
                return AddressType.HasFlag(DeliveryAddressTypes.Domestic);
            }
            set
            {
                if (value)
                    AddressType |= DeliveryAddressTypes.Domestic;
                else
                    AddressType &= ~DeliveryAddressTypes.Domestic;
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
                return AddressType.HasFlag(DeliveryAddressTypes.Home);
            }
            set
            {
                if (value)
                    AddressType |= DeliveryAddressTypes.Home;
                else
                    AddressType &= ~DeliveryAddressTypes.Home;
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
                return AddressType.HasFlag(DeliveryAddressTypes.International);
            }
            set
            {
                if (value)
                    AddressType |= DeliveryAddressTypes.International;
                else
                    AddressType &= ~DeliveryAddressTypes.International;
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
                return AddressType.HasFlag(DeliveryAddressTypes.Parcel);
            }
            set
            {
                if (value)
                    AddressType |= DeliveryAddressTypes.Parcel;
                else
                    AddressType &= ~DeliveryAddressTypes.Parcel;
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
                return AddressType.HasFlag(DeliveryAddressTypes.Postal);
            }
            set
            {
                if (value)
                    AddressType |= DeliveryAddressTypes.Postal;
                else
                    AddressType &= ~DeliveryAddressTypes.Postal;
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
                return AddressType.HasFlag(DeliveryAddressTypes.Work);
            }
            set
            {
                if (value)
                    AddressType |= DeliveryAddressTypes.Work;
                else
                    AddressType &= ~DeliveryAddressTypes.Work;
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
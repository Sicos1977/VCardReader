//
// DeliveryLabel.cs
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

using VCardReader.Collections;

namespace VCardReader
{
    /// <summary>
    ///     A formatted delivery label.
    /// </summary>
    /// <seealso cref="DeliveryAddress" />
    /// <seealso cref="DeliveryLabelCollection" />
    public class DeliveryLabel
    {
        #region Fields
        private string _text;
        #endregion

        #region Constructors
        /// <summary>
        ///     Initializes a new <see cref="DeliveryLabel" />.
        /// </summary>
        public DeliveryLabel()
        {
        }

        /// <summary>
        ///     Initializes a new <see cref="DeliveryLabel" /> to the specified text.
        /// </summary>
        /// <param name="text">
        ///     The formatted text of a delivery label. The label may contain carriage returns, line feeds, and other
        ///     control characters.
        /// </param>
        public DeliveryLabel(string text)
        {
            _text = text ?? string.Empty;
        }
        #endregion

        #region AddressType
        /// <summary>
        ///     The type of delivery address for the label.
        /// </summary>
        public DeliveryAddressTypes AddressType { get; set; }
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

        #region Text
        /// <summary>
        ///     The formatted delivery text.
        /// </summary>
        public string Text
        {
            get { return _text ?? string.Empty; }
            set { _text = value; }
        }
        #endregion
    }
}
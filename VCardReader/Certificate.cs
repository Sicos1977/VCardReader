//
// Certificate.cs
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
using System.Security.Cryptography.X509Certificates;

namespace VCardReader
{
    /// <summary>
    ///     A certificate attached to a vCard.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A vCard can be associated with a public key or authentication certificate. This is typically
    ///         a public X509 certificate that allows people to use the key for validating messages.
    ///     </para>
    /// </remarks>
    [Serializable]
    public class Certificate
    {
        #region Fields
        private string _keyType;
        #endregion

        #region Properties
        /// <summary>
        ///     The raw data of the certificate as a byte array.
        /// </summary>
        /// <remarks>
        ///     Most certificates consist of 8-bit binary data that is encoded into a text format using BASE64
        ///     or a similar system. This property provides access to the computer-friendly, decoded data.
        /// </remarks>
        public byte[] Data { get; set; }

        /// <summary>
        ///     A short string that identifies the type of certificate.
        /// </summary>
        /// <remarks>
        ///     The most common type is X509.
        /// </remarks>
        public string KeyType
        {
            get { return _keyType ?? string.Empty; }
            set { _keyType = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        ///     Creates a new instance of the <see cref="Certificate" /> class.
        /// </summary>
        public Certificate()
        {
            _keyType = string.Empty;
        }

        /// <summary>
        ///     Creates a new instance of the <see cref="Certificate" /> class using the specified key type and raw certificate data.
        /// </summary>
        /// <param name="keyType">
        ///     A string that identifies the type of certificate, such as X509.
        /// </param>
        /// <param name="data">
        ///     The raw certificate data stored as a byte array.
        /// </param>
        public Certificate(string keyType, byte[] data)
        {
            if (string.IsNullOrEmpty(keyType))
                throw new ArgumentNullException("keyType");

            if (data == null)
                throw new ArgumentNullException("data");

            KeyType = keyType;
            Data = data;
        }


        /// <summary>
        ///     Creates a vCard certificate based on an X509 certificate.
        /// </summary>
        /// <param name="x509">
        ///     An initialized X509 certificate.
        /// </param>
        public Certificate(X509Certificate2 x509)
        {
            if (x509 == null)
                throw new ArgumentNullException("x509");

            Data = x509.RawData;
            _keyType = "X509";
        }
        #endregion
    }
}
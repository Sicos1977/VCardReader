using System;
using System.Security.Cryptography.X509Certificates;

namespace VCardReader
{
    /// <summary>
    ///     A certificate attached to a vCard.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A vCard can be associated with a public key or
    ///         authentication certificate.  This is typically
    ///         a public X509 certificate that allows people to
    ///         use the key for validating messages.
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
        ///     Most certificates consist of 8-bit binary data
        ///     that is encoded into a text format using BASE64
        ///     or a similar system.  This property provides
        ///     access to the computer-friendly, decoded data.
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
            get
            {
                return _keyType ?? string.Empty;
            }
            set
            {
                _keyType = value;
            }
        }
        #endregion
        
        #region Constructors
        /// <summary>
        ///     Creates a new instance of the <see cref="Certificate"/> class.
        /// </summary>
        public Certificate()
        {
            _keyType = string.Empty;
        }

        /// <summary>
        ///     Creates a new instance of the <see cref="Certificate"/>
        ///     class using the specified key type and raw certificate data.
        /// </summary>
        /// <param name="keyType">
        ///     A string that identifies the type of certificate,
        ///     such as X509.
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
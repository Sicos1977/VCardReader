namespace VCardReader
{
    /// <summary>
    ///     The encoding used to store a vCard property value in text format.
    /// </summary>
    public enum VCardEncoding
    {
        /// <summary>
        ///     Unknown or no encoding.
        /// </summary>
        Unknown = 0,

        /// <summary>
        ///     Standard escaped text.
        /// </summary>
        Escaped,

        /// <summary>
        ///     Binary or BASE64 encoding.
        /// </summary>
        Base64,

        /// <summary>
        ///     Quoted-Printable encoding.
        /// </summary>
        QuotedPrintable
    }
    #endrgion
}
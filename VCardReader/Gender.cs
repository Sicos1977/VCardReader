namespace VCardReader
{

    /// <summary>
    ///     The gender (male or female) of the contact.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Gender is not directly supported by the vCard specification.
    ///         It is recognized by Microsoft Outlook and the Windows Address
    ///         Book through an extended property called X-WAB-GENDER.  This
    ///         property has a value of 1 for women and 2 for men.
    ///     </para>
    /// </remarks>
    /// <seealso cref="VCard.Gender"/>
    public enum Gender
    {
        /// <summary>
        ///     Unknown gender.
        /// </summary>
        Unknown = 0,
        
        /// <summary>
        ///     Female gender.
        /// </summary>
        Female = 1,

        /// <summary>
        ///     Male gender.
        /// </summary>
        Male = 2
    }
}
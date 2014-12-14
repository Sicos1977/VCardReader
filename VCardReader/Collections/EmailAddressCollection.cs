using System.Collections.ObjectModel;

namespace VCardReader.Collections
{
    /// <summary>
    ///     A collection of <see cref="EmailAddress" /> objects.
    /// </summary>
    /// <seealso cref="EmailAddress" />
    /// <seealso cref="EmailAddressType" />
    public class EmailAddressCollection : Collection<EmailAddress>
    {
        #region GetFirstChoice
        /// <summary>
        ///     Locates the first email address of the specified type while giving preference to email addresses marked as preferred.
        /// </summary>
        /// <param name="emailType">
        ///     The type of email address to locate. This can be any combination of values from <see cref="EmailAddressType" />.
        /// </param>
        /// <returns>
        ///     The function returns the first preferred email address that matches the specified type. 
        ///     If the collection does not contain a preferred email address, then it will return the first 
        ///     non-preferred matching email address. The function returns null if no matches were found.
        /// </returns>
        // ReSharper disable once UnusedMember.Global
        public EmailAddress GetFirstChoice(EmailAddressType emailType)
        {
            EmailAddress firstNonPreferred = null;

            foreach (var email in this)
            {
                if ((email.EmailType & emailType) == emailType)
                {
                    if (firstNonPreferred == null)
                        firstNonPreferred = email;

                    if (email.IsPreferred)
                        return email;
                }
            }

            return firstNonPreferred;
        }
        #endregion
    }
}
using System.Collections.ObjectModel;

namespace VCardReader.Collections
{
    /// <summary>
    ///     A generic collection <see cref="Phone" /> objects.
    /// </summary>
    /// <seealso cref="Phone" />
    /// <seealso cref="PhoneTypes" />
    public class PhoneCollection : Collection<Phone>
    {
        #region GetFirstChoice
        /// <summary>
        ///     Looks for the first phone of the specified type that is a preferred phone.
        /// </summary>
        /// <param name="phoneType">
        ///     The type of phone to seek.
        /// </param>
        /// <returns>
        ///     The first <see cref="Phone " /> that matches the specified type. A preferred number is returned
        ///     before a non-preferred number.
        /// </returns>
        public Phone GetFirstChoice(PhoneTypes phoneType)
        {
            Phone firstNonPreferred = null;

            foreach (var phone in this)
            {
                if ((phone.PhoneType & phoneType) == phoneType)
                {
                    // This phone has the same phone type as
                    // specified by the caller.  Save a reference
                    // to the first such phone encountered.

                    if (firstNonPreferred == null)
                        firstNonPreferred = phone;

                    if (phone.IsPreferred)
                        return phone;
                }
            }

            // No phone had the specified phone type and was marked
            // as preferred.

            return firstNonPreferred;
        }
        #endregion
    }
}
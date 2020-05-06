using System.Collections.ObjectModel;

/*
   Copyright 2014-2016 Kees van Spelde

   Licensed under The Code Project Open License (CPOL) 1.02;
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

     http://www.codeproject.com/info/cpol10.aspx

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

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
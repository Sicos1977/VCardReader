using System.Collections.ObjectModel;

/*
   Copyright 2014-2015 Kees van Spelde

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

                    if (phone.PhoneType == PhoneTypes.Preferred)
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
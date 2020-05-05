//
// PhoneCollection.cs
//
// Author: Kees van Spelde <sicos2002@hotmail.com>
//
// Copyright (c) 2014-2020 Magic-Sessions. (www.magic-sessions.com)
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
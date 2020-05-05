//
// EmailAddressCollection.cs
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
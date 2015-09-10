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

namespace VCardReader
{
    /// <summary>
    ///     The gender (male or female) of the contact.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Gender is not directly supported by the vCard specification. It is recognized by Microsoft Outlook and the Windows Address
    ///         Book through an extended property called X-WAB-GENDER. This property has a value of 1 for women and 2 for men.
    ///     </para>
    /// </remarks>
    /// <seealso cref="VCard.Gender" />
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
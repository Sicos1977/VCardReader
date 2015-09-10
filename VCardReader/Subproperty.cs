using System;

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
    ///     A subproperty of a vCard property.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A vCard is fundamentally a set of properties in NAME:VALUE format, where the name is a keyword like "EMAIL" and the
    ///         value is a string appropriate for the keyword (e.g. an email address for the EMAIL property, or a BASE64 encoded image
    ///         for the PHOTO property).
    ///     </para>
    ///     <para>
    ///         All vCard properties support subproperties. These can be global options like encoding or value type, or might be
    ///         options specific to the keyword.  For example, all vCard properties can have an encoding subproperty that identifies
    ///         the text encoding of the value.  A phone property, however, supports special properties that identify the type and
    ///         purpose of the phone.
    ///     </para>
    ///     <para>
    ///         A subproperty is not required to have a value. In such a case the subproperty acts like a flag.  For example, the TEL
    ///         property of the vCard specification is used to indicate a telephone number associated with the person. This property
    ///         supports a subproperty called BBS, which indicates the telephone number is for a dial-up bulletin board system.
    ///         The BBS subproperty does not need a value; the existance of the BBS subproperty is sufficient to indicate the telephone
    ///         number is for a BBS system.
    ///     </para>
    /// </remarks>
    public class Subproperty
    {
        #region Properties
        /// <summary>
        ///     The name of the subproperty.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     The optional value of the subproperty.
        /// </summary>
        public string Value { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        ///     Creates a subproperty with the specified
        ///     name and no value.
        /// </summary>
        /// <param name="name">
        ///     The name of the subproperty.
        /// </param>
        public Subproperty(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            Name = name;
        }

        /// <summary>
        ///     Creates a subproperty with the specified
        ///     name and value.
        /// </summary>
        /// <param name="name">
        ///     The name of the subproperty.
        /// </param>
        /// <param name="value">
        ///     The value of the subproperty.  This can be null.
        /// </param>
        public Subproperty(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            Name = name;
            Value = value;
        }
        #endregion
    }
}
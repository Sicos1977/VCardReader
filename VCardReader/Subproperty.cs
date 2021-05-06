//
// Subproperty.cs
//
// Author: Kees van Spelde <sicos2002@hotmail.com>
//
// Copyright (c) 2014-2021 Magic-Sessions. (www.magic-sessions.com)
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

using System;

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
    ///         The BBS subproperty does not need a value; the existence of the BBS subproperty is sufficient to indicate the telephone
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
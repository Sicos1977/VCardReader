//
// Property.cs
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
using VCardReader.Collections;

namespace VCardReader
{
    /// <summary>
    ///     A property of a <see cref="VCard" />.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A vCard property specifies a single piece of information, such as an email address or telephone number.
    ///         A property can also specify meta-data like a revision number. A full vCards is basically a collection
    ///         of properties structured into a computer-friendly text format.
    ///     </para>
    ///     <para>
    ///         A property has a name, a value, and optionally one or more subproperties. A subproperty provides additional
    ///         information about the property (such as the encoding used to store the value).  The format of a value
    ///         depends on the property and in some cases may be broken into multiple values.
    ///     </para>
    /// </remarks>
    /// <seealso cref="PropertyCollection" />
    public class Property
    {
        #region Fields
        private string _group;
        private string _language;
        private string _name;
        #endregion

        #region Value
        /// <summary>
        ///     The value of the property.
        /// </summary>
        public object Value { get; set; }
        #endregion

        #region Group
        /// <summary>
        ///     The group name of the property.
        /// </summary>
        public string Group
        {
            get { return _group ?? string.Empty; }
            set { _group = value; }
        }
        #endregion

        #region Language
        /// <summary>
        ///     The language code of the property.
        /// </summary>
        public string Language
        {
            get { return _language ?? string.Empty; }
            set { _language = value; }
        }
        #endregion

        #region Name
        /// <summary>
        ///     The name of the property (e.g. TEL).
        /// </summary>
        public string Name
        {
            get { return _name ?? string.Empty; }
            set { _name = value; }
        }
        #endregion

        #region Subproperties
        /// <summary>
        ///     Subproperties of the vCard property, not including the name, encoding, and character set.
        /// </summary>
        public SubpropertyCollection Subproperties { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        ///     Creates a blank <see cref="Property" /> object.
        /// </summary>
        public Property()
        {
            Subproperties = new SubpropertyCollection();
        }

        /// <summary>
        ///     Creates a <see cref="Property" /> object
        ///     with the specified name and a null value.
        /// </summary>
        /// <param name="name">
        ///     The name of the property.
        /// </param>
        public Property(string name)
        {
            _name = name;
            Subproperties = new SubpropertyCollection();
        }

        /// <summary>
        ///     Creates a <see cref="Property" /> object with the
        ///     specified name and value.
        /// </summary>
        /// <remarks>
        ///     The vCard specification supports multiple values in
        ///     certain fields, such as the N field.  The value specified
        ///     in this constructor is loaded as the first value.
        /// </remarks>
        public Property(string name, string value)
        {
            _name = name;
            Subproperties = new SubpropertyCollection();
            Value = value;
        }

        /// <summary>
        ///     Initializes a vCardProperty with the specified
        ///     name, value and group.
        /// </summary>
        /// <param name="name">
        ///     The name of the vCard property.
        /// </param>
        /// <param name="value">
        ///     The value of the vCard property.
        /// </param>
        /// <param name="group">
        ///     The group name of the vCard property.
        /// </param>
        public Property(string name, string value, string group)
        {
            _group = group;
            _name = name;
            Subproperties = new SubpropertyCollection();
            Value = value;
        }

        /// <summary>
        ///     Creates a <see cref="Property" /> with the
        ///     specified name and a byte array as a value.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value as a byte array.</param>
        public Property(string name, byte[] value)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            _name = name;
            Subproperties = new SubpropertyCollection();
            Value = value;
        }

        /// <summary>
        ///     Creates a <see cref="Property" /> with
        ///     the specified name and date/time as a value.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The date/time value.</param>
        public Property(string name, DateTime value)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            _name = name;
            Subproperties = new SubpropertyCollection();
            Value = value;
        }

        /// <summary>
        ///     Initializes the vCard property with the specified
        ///     name and values.
        /// </summary>
        public Property(string name, ValueCollection values)
            : this()
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (values == null)
                throw new ArgumentNullException("values");

            Subproperties = new SubpropertyCollection();
            _name = name;
            Value = values;
        }
        #endregion

        #region ToString
        /// <summary>
        ///     Returns the value of the property as a string.
        /// </summary>
        public override string ToString()
        {
            return Value == null ? string.Empty : Value.ToString();
        }
        #endregion
    }
}
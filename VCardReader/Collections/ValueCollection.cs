//
// ValueCollection.cs
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

using System;
using System.Collections.Specialized;

namespace VCardReader.Collections
{
    /// <summary>
    ///     A collection of string values.
    /// </summary>
    public class ValueCollection : StringCollection
    {
        #region Properties
        /// <summary>
        ///     The suggested separator when writing values to a string.
        /// </summary>
        public char Separator { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        ///     Initializes an empty <see cref="ValueCollection" />.
        /// </summary>
        public ValueCollection()
        {
            Separator = ',';
        }

        /// <summary>
        ///     Initializes the value collection with the specified separator.
        /// </summary>
        /// <param name="separator">
        ///     The suggested character to use as a separator when writing the collection as a string.
        /// </param>
        public ValueCollection(char separator)
        {
            Separator = separator;
        }
        #endregion

        #region Add
        /// <summary>
        ///     Adds the contents of a StringCollection to the collection.
        /// </summary>
        /// <param name="values">
        ///     An initialized StringCollection containing zero or more values.
        /// </param>
        public void Add(StringCollection values)
        {
            if (values == null)
                throw new ArgumentNullException("values");

            foreach (var value in values)
                Add(value);
        }
        #endregion
    }
}
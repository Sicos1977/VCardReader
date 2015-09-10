using System;
using System.Collections.Specialized;

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
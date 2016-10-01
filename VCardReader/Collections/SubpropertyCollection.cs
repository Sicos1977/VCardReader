using System;
using System.Collections;
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
    ///     A collection of <see cref="Subproperty" /> objects.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This class is a general-purpose collection of <see cref="Subproperty" /> objects.
    ///     </para>
    ///     <para>
    ///         A property of a vCard contains a piece of contact information, such as an email address
    ///         or web site.  A subproperty indicates options or attributes of the property, such as the
    ///         type of email address or character set.
    ///     </para>
    /// </remarks>
    /// <seealso cref="Property" />
    /// <seealso cref="Subproperty" />
    public class SubpropertyCollection : Collection<Subproperty>
    {
        #region Add
        /// <summary>
        ///     Adds a subproperty without a value.
        /// </summary>
        /// <param name="name">
        ///     The name of the subproperty.
        /// </param>
        public void Add(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            Add(new Subproperty(name));
        }

        /// <summary>
        ///     Adds a subproperty with the specified name and value.
        /// </summary>
        /// <param name="name">
        ///     The name of the new subproperty to add.
        /// </param>
        /// <param name="value">
        ///     The value of the new subproperty to add.  This can be null.
        /// </param>
        public void Add(string name, string value)
        {
            Add(new Subproperty(name, value));
        }
        #endregion

        #region AddOrUpdate
        /// <summary>
        ///     Either adds or updates a subproperty with the specified name.
        /// </summary>
        /// <param name="name">
        ///     The name of the subproperty to add or update.
        /// </param>
        /// <param name="value">
        ///     The value of the subproperty to add or update.
        /// </param>
        public void AddOrUpdate(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            var index = IndexOf(name);

            if (index == -1)
                Add(name, value);
            else
                this[index].Value = value;
        }
        #endregion

        #region Contains
        /// <summary>
        ///     Determines if the collection contains a subproperty with the specified name.
        /// </summary>
        /// <param name="name">
        ///     The name of the subproperty.
        /// </param>
        /// <returns>
        ///     True if the collection contains a subproperty with the specified name, or False otherwise.
        /// </returns>
        public bool Contains(string name)
        {
            foreach (var subproperty in this)
            {
                if (string.Compare(name, subproperty.Name, StringComparison.OrdinalIgnoreCase) == 0)
                    return true;
            }

            return false;
        }
        #endregion

        #region GetNames
        /// <summary>
        ///     Builds a string array containing subproperty names.
        /// </summary>
        /// <returns>
        ///     A string array containing the unmodified name of each subproperty in the collection.
        /// </returns>
        public string[] GetNames()
        {
            var names = new ArrayList(Count);

            foreach (var subproperty in this)
                names.Add(subproperty.Name);

            return (string[]) names.ToArray(typeof (string));
        }

        /// <summary>
        ///     Builds a string array containing all subproperty
        ///     names that match one of the names in an array.
        /// </summary>
        /// <param name="filteredNames">
        ///     A list of valid subproperty names.
        /// </param>
        /// <returns>
        ///     A string array containing the names of all subproperties
        ///     that match an entry in the filterNames list.
        /// </returns>
        public string[] GetNames(string[] filteredNames)
        {
            if (filteredNames == null)
                throw new ArgumentNullException("filteredNames");

            // The vCard specification is not case-sensitive.  
            // Therefore the subproperty names and the filter names
            // list must be compared in a case-insensitive matter.
            // Whitespace will also be ignored.  For better-
            // performing comparisons, a processed version of
            // the filtered list will be constructed.

            var processedNames =
                (string[]) filteredNames.Clone();

            for (var index = 0; index < processedNames.Length; index++)
            {
                if (!string.IsNullOrEmpty(processedNames[index]))
                    processedNames[index] =
                        processedNames[index].Trim().ToUpperInvariant();
            }

            // Matching names will be stored in an array list,
            // and then converted to a string array for return.

            var matchingNames = new ArrayList();

            foreach (var subproperty in this)
            {
                // Convert this subproperty name to upper case.
                // The names in the processed array are already
                // in upper case.

                var subName =
                    subproperty.Name == null ? null : subproperty.Name.ToUpperInvariant();

                // See if the processed subproperty name has any
                // matches in the processed array. 

                var matchIndex =
                    Array.IndexOf(processedNames, subName);

                if (matchIndex != -1)
                    matchingNames.Add(processedNames[matchIndex]);
            }

            return (string[]) matchingNames.ToArray(typeof (string));
        }
        #endregion

        #region GetValue
        /// <summary>
        ///     Get the value of the subproperty with the specified name.
        /// </summary>
        /// <param name="name">
        ///     The name of the subproperty.
        /// </param>
        /// <returns>
        ///     The value of the subproperty or null if no such subproperty exists in the collection.
        /// </returns>
        public string GetValue(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            // Get the collection index of the subproperty
            // object that has the specified name.

            var index = IndexOf(name);
            return index == -1 ? null : this[index].Value;
        }

        /// <summary>
        ///     Gets the value of the first subproperty with the specified name, or the first value specified in
        ///     a list.
        /// </summary>
        /// <param name="name">
        ///     The expected name of the subproperty.
        /// </param>
        /// <param name="namelessValues">
        ///     A list of values that are sometimes listed as subproperty names. The first matching value is
        ///     returned if the name parameter does not match.
        /// </param>
        public string GetValue(
            string name,
            string[] namelessValues)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            // See if the subproperty exists with the
            // specified name.  If so, return the value
            // immediately.

            var index = IndexOf(name);
            if (index != -1)
                return this[index].Value;

            // A subproperty with the specified name does
            // not exist.  However, this does not mean that
            // the subproperty does not exist.  Some subproperty
            // values can be written directly without a name.
            // An example is the ENCODING property.  Example:
            //
            // New Format: KEY;ENCODING=BASE64:....
            // Old Format: KEY;BASE64:...

            if ((namelessValues == null) || (namelessValues.Length == 0))
                return null;

            var nameIndex = IndexOfAny(namelessValues);
            return nameIndex == -1 ? null : this[nameIndex].Name;
        }
        #endregion

        #region IndexOf
        /// <summary>
        ///     Searches for a subproperty with the specified name.
        /// </summary>
        /// <param name="name">
        ///     The name of the subproperty.
        /// </param>
        /// <returns>
        ///     The collection (zero-based) index of the first subproperty that matches the specified name. 
        ///     The function returns -1 if no match is found.
        /// </returns>
        public int IndexOf(string name)
        {
            for (var index = 0; index < Count; index++)
            {
                if (string.Compare(name, this[index].Name, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return index;
                }
            }

            return -1;
        }
        #endregion

        #region IndexOfAny
        /// <summary>
        ///     Finds the first subproperty that has any of the specified names.
        /// </summary>
        /// <param name="names">
        ///     An array of names to search.
        /// </param>
        /// <returns>
        ///     The collection index of the first subproperty with the specified name, or -1 if no subproperty was found.
        /// </returns>
        public int IndexOfAny(string[] names)
        {
            if (names == null)
                throw new ArgumentNullException("names");

            for (var index = 0; index < Count; index++)
            {
                foreach (var name in names)
                {
                    if (string.Compare(this[index].Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                        return index;
                }
            }
            return -1;
        }
        #endregion
    }
}
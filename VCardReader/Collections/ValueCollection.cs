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
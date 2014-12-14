using System.Collections.ObjectModel;

namespace VCardReader.Collections
{
    /// <summary>
    ///     A collection of <see cref="Note" /> objects.
    /// </summary>
    public class NoteCollection : Collection<Note>
    {
        #region Constructor
        #endregion

        #region Add
        /// <summary>
        ///     Adds a new note to the collection.
        /// </summary>
        /// <param name="text">
        ///     The text of the note.
        /// </param>
        /// <returns>
        ///     The <see cref="Note" /> object representing the note.
        /// </returns>
        public Note Add(string text)
        {
            var note = new Note(text);
            Add(note);
            return note;
        }
        #endregion
    }
}
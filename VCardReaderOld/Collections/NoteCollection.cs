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
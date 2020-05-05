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

namespace VCardReader
{
    /// <summary>
    ///     A note or comment in a vCard.
    /// </summary>
    public class Note
    {
        #region Fields
        private string _language;
        private string _text;
        #endregion

        #region Constructor
        /// <summary>
        ///     Initializes a new vCard note.
        /// </summary>
        public Note()
        {
        }
        #endregion

        #region Note
        /// <summary>
        ///     Initializes a new vCard note with the specified text.
        /// </summary>
        /// <param name="text">
        ///     The text of the note or comment.
        /// </param>
        public Note(string text)
        {
            _text = text;
        }
        #endregion

        #region Language
        /// <summary>
        ///     The language of the note.
        /// </summary>
        public string Language
        {
            get { return _language ?? string.Empty; }
            set { _language = value; }
        }
        #endregion

        #region Text
        /// <summary>
        ///     The text of the note.
        /// </summary>
        public string Text
        {
            get { return _text ?? string.Empty; }
            set { _text = value; }
        }
        #endregion

        #region ToString
        /// <summary>
        ///     Returns the text of the note.
        /// </summary>
        public override string ToString()
        {
            return _text ?? string.Empty;
        }
        #endregion
    }
}
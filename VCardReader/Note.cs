//
// Note.cs
//
// Author: Kees van Spelde <sicos2002@hotmail.com>
//
// Copyright (c) 2014-2023 Magic-Sessions. (www.magic-sessions.com)
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
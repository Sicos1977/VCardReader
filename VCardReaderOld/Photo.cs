using System;
using System.Drawing;
using System.IO;
using System.Net;

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
    ///     A photo embedded in a vCard.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         You must specify the photo using a path, a byte array, or a System.Drawing.Bitmap instance. The class will
    ///         extract the underlying raw bytes for storage into the vCard. You can call the <see cref="GetBitmap" /> function
    ///         to create a new Windows bitmap object (e.g. for display on a form) or <see cref="GetBytes" /> to extract the raw
    ///         bytes (e.g. for transmission from a web page).
    ///     </para>
    /// </remarks>
    [Serializable]
    public class Photo
    {
        #region Fields
        /// <summary>
        ///     The raw bytes of the image data.
        /// </summary>
        /// <remarks>
        ///     The raw bytes can be passed directly to the photo object  or fetched from a file or remote URL.  
        ///     A .NET bitmap object can also be specified, in which case the constructor will load the raw bytes from the bitmap.
        /// </remarks>
        private byte[] _data;

        /// <summary>
        ///     The url of the image.
        /// </summary>
        private Uri _url;
        #endregion

        #region GetBytes
        /// <summary>
        ///     Returns a copy of the raw bytes of the image.
        /// </summary>
        /// <returns>
        ///     A byte array containing the raw bytes of the image.
        /// </returns>
        /// <remarks>
        ///     A copy of the raw bytes are returned.  Modifying the
        ///     array will not modify the photo.
        /// </remarks>
        public byte[] GetBytes()
        {
            return (byte[]) _data.Clone();
        }
        #endregion

        #region IsLoaded
        /// <summary>
        ///     Indicates the bytes of the raw image have been loaded by the object.
        /// </summary>
        /// <seealso cref="Fetch" />
        public bool IsLoaded
        {
            get { return _data != null; }
        }
        #endregion

        #region Url
        /// <summary>
        ///     The URL of the image.
        /// </summary>
        /// <remarks>
        ///     Changing the URL will automatically invalidate the internal image data if previously fetched.
        /// </remarks>
        /// <seealso cref="Fetch" />
        public Uri Url
        {
            get { return _url; }
            set
            {
                // This class maintains a byte array containing the raw
                // bytes of the image.  The use can call the Fetch method
                // to load the raw bytes from a remote link.  If the
                // URL is changed (e.g. via this property), then the local
                // cache must be invalidated.
                if (value == null)
                {
                    _data = null;
                    _url = null;
                }
                else
                {
                    if (_url == value) return;
                    _data = null;
                    _url = value;
                }
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        ///     Loads a photograph from an array of bytes.
        /// </summary>
        /// <param name="buffer">
        ///     An array of bytes containing the raw data from
        ///     any of the supported image formats.
        /// </param>
        public Photo(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            _data = (byte[]) buffer.Clone();
        }

        /// <summary>
        ///     The URL of the image.
        /// </summary>
        /// <param name="url">
        ///     A URL pointing to an image.
        /// </param>
        public Photo(Uri url)
        {
            if (url == null)
                throw new ArgumentNullException("url");

            _url = url;
        }

        /// <summary>
        ///     Creates a new vCard photo from an image file.
        /// </summary>
        /// <param name="path">
        ///     The path to an image of any supported format.
        /// </param>
        public Photo(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            _url = new Uri(path);
        }

        /// <summary>
        ///     Creates a new vCard photo from an existing Bitmap object.
        /// </summary>
        /// <param name="bitmap">
        ///     A bitmap to be attached to the vCard as a photo.
        /// </param>
        public Photo(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            // Extract the raw bytes of the bitmap
            // to a stream.

            var bytes = new MemoryStream();
            bitmap.Save(bytes, bitmap.RawFormat);

            // Extract the bytes of the stream to the array.

            bytes.Seek(0, SeekOrigin.Begin);
            bytes.Read(_data, 0, (int) bytes.Length);
        }
        #endregion

        #region Fetch
        /// <summary>
        ///     Fetches a linked image asynchronously.
        /// </summary>
        /// <remarks>
        ///     This is a simple utility method for accessing the image referenced by the URL. For asynchronous or advanced
        ///     loading you will need to download the image yourself and load the bytes directly into the class.
        /// </remarks>
        /// <seealso cref="IsLoaded" />
        /// <seealso cref="Url" />
        public void Fetch()
        {
            // An image can be fetched only if the URL of the image is known.  
            // Otherwise the fetch operation makes no sense.

            if (_url == null)
                throw new InvalidOperationException();

            // Create a web request object that will handle the
            // specifics of downloading a file from the specified
            // URL.  For example, the URL is a file-based URL, then
            // the CreateDefault method will return a FileWebRequest
            // class.

            var request =
                WebRequest.CreateDefault(_url);

            // Start the request.  The request begins when
            // the GetResponse method is invoked.  This is a
            // synchronous (blocking) call (i.e. it will not
            // return until the file is downloaded or an 
            // exception is raised).

            var response = request.GetResponse();

            using (var responseStream = response.GetResponseStream())
            {
                // Allocate space to hold the entire image.

                _data = new byte[response.ContentLength];

                // The following call will fail if the image
                // size is larger than the capacity of an Int32.
                // This may be treated as a minor issue given
                // the fact that this is a vCard library and
                // such images are expected by humans to be small.
                // No reasonable person would embed a multi-gigabyte
                // image into a vCard.

                if (responseStream != null)
                    responseStream.Read(
                        _data,
                        0,
                        (int) response.ContentLength);
            }
        }
        #endregion

        #region GetBitmap
        /// <summary>
        ///     Creates a Bitmap object from the photo data.
        /// </summary>
        /// <remarks>
        ///     An initialized Bitmap object.  An exception is
        ///     raised if the .NET framework is unable to identify
        ///     the format of the image data, or if the format
        ///     is not supported.
        /// </remarks>
        public Bitmap GetBitmap()
        {
            var stream = new MemoryStream(_data);
            return new Bitmap(stream);
        }
        #endregion
    }
}
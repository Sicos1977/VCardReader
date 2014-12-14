using System;

namespace OfficeConverter.Exceptions
{
    /// <summary>
    ///     Raised when the file type is not supported
    /// </summary>
    public class VCRFileTypeNotSupported : Exception
    {
        internal VCRFileTypeNotSupported()
        {
        }

        internal VCRFileTypeNotSupported(string message) : base(message)
        {
        }

        internal VCRFileTypeNotSupported(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
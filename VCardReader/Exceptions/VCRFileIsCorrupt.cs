using System;

namespace OfficeConverter.Exceptions
{
    /// <summary>
    ///     Raised when the VCard file is corrupt
    /// </summary>
    public class VCRFileIsCorrupt : Exception
    {
        internal VCRFileIsCorrupt() {}

        internal VCRFileIsCorrupt(string message) : base(message) {}

        internal VCRFileIsCorrupt(string message, Exception inner) : base(message, inner) {}
    }
}
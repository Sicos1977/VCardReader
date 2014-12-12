using System;

namespace OfficeConverter.Exceptions
{
    /// <summary>
    ///     Raised when the VCard file contains no actual data
    /// </summary>
    public class VCRFileContainsNoData : Exception
    {
        internal VCRFileContainsNoData() { }

        internal VCRFileContainsNoData(string message) : base(message) { }

        internal VCRFileContainsNoData(string message, Exception inner) : base(message, inner) { }
    }
}
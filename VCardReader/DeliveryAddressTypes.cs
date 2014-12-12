using System;

namespace VCardReader
{
    /// <summary>
    ///     The type of a delivery address.
    /// </summary>
    [Flags]
    public enum DeliveryAddressTypes
    {
        /// <summary>
        ///     Default address settings.
        /// </summary>
        Default = 0,

        /// <summary>
        ///     A domestic delivery address.
        /// </summary>
        Domestic,

        /// <summary>
        ///     An international delivery address.
        /// </summary>
        International,

        /// <summary>
        ///     A postal delivery address.
        /// </summary>
        Postal,

        /// <summary>
        ///     A parcel delivery address.
        /// </summary>
        Parcel,

        /// <summary>
        ///     A home delivery address.
        /// </summary>
        Home,

        /// <summary>
        ///     A work delivery address.
        /// </summary>
        Work
    }
}
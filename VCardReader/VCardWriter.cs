using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using VCardReader.Collections;

namespace VCardReader
{
    #region Public enum WriterOptions
    /// <summary>
    ///     Extended options for the <see cref="VCardWriter" /> class.
    /// </summary>
    [Flags]
    public enum WriterOptions
    {
        /// <summary>
        ///     No options.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Indicates whether or not commas should be escaped in values.
        /// </summary>
        /// <remarks>
        ///     The vCard specification requires that commas be escaped
        ///     in values (e.g. a "," is translated to "\,").  However, Microsoft
        ///     Outlook(tm) does not properly decode these escaped commas.  This
        ///     option instruct the writer to ignored (not translate) embedded
        ///     commas for better compatibility with Outlook.
        /// </remarks>
        IgnoreCommas = 1
    }
    #endregion

    /// <summary>
    ///     Implements the standard vCard 2.1 and 3.0 text formats.
    /// </summary>
    public class VCardWriter
    {
        #region Fields
        private readonly bool _embedInternetImages;
        private readonly bool _embedLocalImages;

        /// <summary>
        ///     The characters that are escaped by Microsoft Outlook.
        /// </summary>
        /// <remarks>
        ///     Microsoft Outlook does not property decode escaped commas in values.
        /// </remarks>
        private readonly char[] _outlookEscapedCharacters = {'\\', ';', '\r', '\n'};

        /// <summary>
        ///     The characters that are escaped per the original vCard specification.
        /// </summary>
        private readonly char[] _standardEscapedCharacters = {',', '\\', ';', '\r', '\n'};
        #endregion

        #region Properties
        /// <summary>
        ///     A collection of warning messages that were generated during the output of a vCard.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public StringCollection Warnings { get; private set; }

        /// <summary>
        ///     Extended options for the vCard writer.
        /// </summary>
        public WriterOptions Options { get; set; }

        /// <summary>
        ///     The product ID to use when writing a vCard.
        /// </summary>
        public string ProductId { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        ///     Creates a new instance of the standard writer.
        /// </summary>
        /// <remarks>
        ///     The standard writer is configured to create vCard files in the highest supported version. 
        ///     This is currently version 3.0.
        /// </remarks>
        public VCardWriter(bool embedInternetImages)
        {
            _embedInternetImages = embedInternetImages;
            _embedLocalImages = true;
            Warnings = new StringCollection();
        }
        #endregion

        #region BuildProperties
        /// <summary>
        ///     Builds a collection of standard properties based on the specified vCard.
        /// </summary>
        /// <returns>
        ///     A <see cref="PropertyCollection" /> that contains all properties for the current vCard, 
        ///     including the header and footer properties.
        /// </returns>
        /// <seealso cref="VCard" />
        /// <seealso cref="Property" />
        private PropertyCollection BuildProperties(VCard card)
        {
            var properties = new PropertyCollection {new Property("BEGIN", "VCARD")};

            // The BEGIN:VCARD line marks the beginning of
            // the vCard contents.  Later it will end with END:VCARD.
            // See section 2.1.1 of RFC 2426.

            BuildPropertiesName(properties, card);
            BuildPropertiesSource(properties, card);
            BuildPropertiesN(properties, card);
            BuildPropertiesFn(properties, card);
            BuildPropertiesAdr(properties, card);
            BuildPropertiesBDay(properties, card);
            BuildPropertiesCategories(properties, card);
            BuildPropertiesClass(properties, card);
            BuildPropertiesEmail(properties, card);
            BuildPropertiesGeo(properties, card);
            BuildPropertiesKey(properties, card);
            BuildPropertiesLabel(properties, card);
            BuildPropertiesMailer(properties, card);
            BuildPropertiesNickName(properties, card);
            BuildPropertiesNote(properties, card);
            BuildPropertiesOrg(properties, card);
            BuildPropertiesPhoto(properties, card);
            BuildPropertiesProdId(properties, card);
            BuildPropertiesRev(properties, card);
            BuildPropertiesRole(properties, card);
            BuildPropertiesTel(properties, card);
            BuildPropertiesTitle(properties, card);
            BuildPropertiesTz(properties, card);
            BuildPropertiesUid(properties, card);
            BuildPropertiesUrl(properties, card);
            BuildPropertiesXWabGender(properties, card);

            // The end of the vCard is marked with an END:VCARD.
            properties.Add(new Property("END", "VCARD"));
            return properties;
        }
        #endregion

        #region BuildPropertiesAdr
        /// <summary>
        ///     Builds ADR properties.
        /// </summary>
        private static void BuildPropertiesAdr(ICollection<Property> properties, VCard card)
        {
            foreach (var deliveryAddress in card.DeliveryAddresses)
            {
                // Do not generate a postal address (ADR) property
                // if the entire address is blank.

                if ((string.IsNullOrEmpty(deliveryAddress.City)) && (string.IsNullOrEmpty(deliveryAddress.Country)) &&
                    (string.IsNullOrEmpty(deliveryAddress.PostalCode)) && (string.IsNullOrEmpty(deliveryAddress.Region)) &&
                    (string.IsNullOrEmpty(deliveryAddress.Street))) continue;

                // The ADR property contains the following subvalues in order.  
                // All are required:
                //
                //   - Post office box
                //   - Extended address
                //   - Street address
                //   - Locality (e.g. city)
                //   - Region (e.g. province or state)
                //   - Postal code (e.g. ZIP code)
                //   - Country name

                var values = new ValueCollection(';')
                {
                    string.Empty,
                    string.Empty,
                    deliveryAddress.Street,
                    deliveryAddress.City,
                    deliveryAddress.Region,
                    deliveryAddress.PostalCode,
                    deliveryAddress.Country
                };

                var property = new Property("ADR", values);

                if (deliveryAddress.IsDomestic)
                    property.Subproperties.Add("DOM");

                if (deliveryAddress.IsInternational)
                    property.Subproperties.Add("INTL");

                if (deliveryAddress.IsParcel)
                    property.Subproperties.Add("PARCEL");

                if (deliveryAddress.IsPostal)
                    property.Subproperties.Add("POSTAL");

                if (deliveryAddress.IsHome)
                    property.Subproperties.Add("HOME");

                if (deliveryAddress.IsWork)
                    property.Subproperties.Add("WORK");

                properties.Add(property);
            }
        }
        #endregion

        #region BuildPropertiesBDay
        /// <summary>
        ///     Builds the BDAY property.
        /// </summary>
        private static void BuildPropertiesBDay(ICollection<Property> properties, VCard card)
        {
            // The BDAY property indicates the birthdate
            // of the person.  The output format here is based on
            // Microsoft Outlook, which writes the date as YYYMMDD.

            if (!card.BirthDate.HasValue) return;
            var property = new Property("BDAY", card.BirthDate.Value);

            properties.Add(property);
        }
        #endregion

        #region BuildPropertiesCategories
        private static void BuildPropertiesCategories(ICollection<Property> properties, VCard card)
        {
            if (card.Categories.Count <= 0) return;
            var values = new ValueCollection(',');

            foreach (var category in card.Categories)
            {
                if (!string.IsNullOrEmpty(category))
                    values.Add(category);
            }

            properties.Add(
                new Property("CATEGORIES", values));
        }
        #endregion

        #region BuildPropertiesClass
        private static void BuildPropertiesClass(ICollection<Property> properties, VCard card)
        {
            var property = new Property("CLASS");

            switch (card.AccessClassification)
            {
                case AccessClassification.Unknown:
                    // No value is written.
                    return;

                case AccessClassification.Confidential:
                    property.Value = "CONFIDENTIAL";
                    break;

                case AccessClassification.Private:
                    property.Value = "PRIVATE";
                    break;

                case AccessClassification.Public:
                    property.Value = "PUBLIC";
                    break;

                default:
                    throw new NotSupportedException();
            }

            properties.Add(property);
        }
        #endregion

        #region BuildPropertiesEmail
        /// <summary>
        ///     Builds EMAIL properties.
        /// </summary>
        private static void BuildPropertiesEmail(ICollection<Property> properties, VCard card)
        {
            // The EMAIL property contains an electronic
            // mail address for the purpose.  A vCard may contain
            // as many email addresses as needed.  The format also
            // supports various vendors, such as CompuServe addresses
            // and Internet SMTP addresses.
            //
            // EMAIL;INTERNET:support@fairmetric.com

            foreach (var emailAddress in card.EmailAddresses)
            {
                var property = new Property {Name = "EMAIL", Value = emailAddress.Address};

                if (emailAddress.IsPreferred)
                {
                    property.Subproperties.Add("PREF");
                }

                switch (emailAddress.EmailType)
                {
                    case EmailAddressType.Internet:
                        property.Subproperties.Add("INTERNET");
                        break;

                    case EmailAddressType.AOl:
                        property.Subproperties.Add("AOL");
                        break;

                    case EmailAddressType.AppleLink:
                        property.Subproperties.Add("AppleLink");
                        break;

                    case EmailAddressType.AttMail:
                        property.Subproperties.Add("ATTMail");
                        break;

                    case EmailAddressType.CompuServe:
                        property.Subproperties.Add("CIS");
                        break;

                    case EmailAddressType.EWorld:
                        property.Subproperties.Add("eWorld");
                        break;

                    case EmailAddressType.IBMMail:
                        property.Subproperties.Add("IBMMail");
                        break;

                    case EmailAddressType.MCIMail:
                        property.Subproperties.Add("MCIMail");
                        break;

                    case EmailAddressType.PowerShare:
                        property.Subproperties.Add("POWERSHARE");
                        break;

                    case EmailAddressType.Prodigy:
                        property.Subproperties.Add("PRODIGY");
                        break;

                    case EmailAddressType.Telex:
                        property.Subproperties.Add("TLX");
                        break;

                    case EmailAddressType.X400:
                        property.Subproperties.Add("X400");
                        break;

                    default:
                        property.Subproperties.Add("INTERNET");
                        break;
                }

                properties.Add(property);
            }
        }
        #endregion

        #region BuildPropertiesFn
        private static void BuildPropertiesFn(ICollection<Property> properties, VCard card)
        {
            // The FN property indicates the formatted 
            // name of the person.  This can be something
            // like "John Smith".

            if (string.IsNullOrEmpty(card.FormattedName)) return;
            var property = new Property("FN", card.FormattedName);

            properties.Add(property);
        }
        #endregion

        #region BuildPropertiesGeo
        /// <summary>
        ///     Builds the GEO property.
        /// </summary>
        private static void BuildPropertiesGeo(ICollection<Property> properties, VCard card)
        {
            // The GEO properties contains the latitude and
            // longitude of the person or company of the vCard.

            if (!card.Latitude.HasValue || !card.Longitude.HasValue) return;
            var property = new Property
            {
                Name = "GEO",
                Value = card.Latitude + ";" + card.Longitude
            };

            properties.Add(property);
        }
        #endregion

        #region BuildPropertiesKey
        /// <summary>
        ///     Builds KEY properties.
        /// </summary>
        private static void BuildPropertiesKey(ICollection<Property> properties, VCard card)
        {
            // A KEY field contains an embedded security certificate.

            foreach (var certificate in card.Certificates)
            {
                var property = new Property {Name = "KEY", Value = certificate.Data};

                property.Subproperties.Add(certificate.KeyType);

                properties.Add(property);
            }
        }
        #endregion

        #region BuildPropertiesLabel
        private static void BuildPropertiesLabel(ICollection<Property> properties, VCard card)
        {
            foreach (var deliveryLabel in card.DeliveryLabels)
            {
                if (deliveryLabel.Text.Length <= 0) continue;
                var property = new Property("LABEL", deliveryLabel.Text);

                if (deliveryLabel.IsDomestic)
                    property.Subproperties.Add("DOM");

                if (deliveryLabel.IsInternational)
                    property.Subproperties.Add("INTL");

                if (deliveryLabel.IsParcel)
                    property.Subproperties.Add("PARCEL");

                if (deliveryLabel.IsPostal)
                    property.Subproperties.Add("POSTAL");

                if (deliveryLabel.IsHome)
                    property.Subproperties.Add("HOME");

                if (deliveryLabel.IsWork)
                    property.Subproperties.Add("WORK");

                // Give a hint to use QUOTED-PRINTABLE.

                property.Subproperties.Add("ENCODING", "QUOTED-PRINTABLE");
                properties.Add(property);
            }
        }
        #endregion

        #region BuildPropertiesMailer
        /// <summary>
        ///     Builds the MAILER property.
        /// </summary>
        private static void BuildPropertiesMailer(ICollection<Property> properties, VCard card)
        {
            // The MAILER property indicates the software that
            // generated the vCard.  See section 2.4.3 of the
            // vCard 2.1 specification.  Support is not widespread.
            if (string.IsNullOrEmpty(card.Mailer)) return;
            var property = new Property("MAILER", card.Mailer);

            properties.Add(property);
        }
        #endregion

        #region BuildPropertiesN
        private static void BuildPropertiesN(ICollection<Property> properties, VCard card)
        {
            // The property has the following components: Family Name,
            // Given Name, Additional Names, Name Prefix, and Name
            // Suffix.  Example:
            //
            //   N:Pinch;David
            //   N:Pinch;David;John
            //
            // The N property is required (see section 3.1.2 of RFC 2426).

            var values = new ValueCollection(';')
            {
                card.FamilyName,
                card.GivenName,
                card.AdditionalNames,
                card.NamePrefix,
                card.NameSuffix
            };

            var property = new Property("N", values);

            properties.Add(property);
        }
        #endregion

        #region BuildPropertiesName
        private static void BuildPropertiesName(ICollection<Property> properties, VCard card)
        {
            if (string.IsNullOrEmpty(card.DisplayName)) return;
            var property = new Property("NAME", card.DisplayName);

            properties.Add(property);
        }
        #endregion

        #region BuildPropertiesNickName
        /// <summary>
        ///     Builds the NICKNAME property.
        /// </summary>
        private static void BuildPropertiesNickName(ICollection<Property> properties, VCard card)
        {
            // The NICKNAME property specifies the familiar name
            // of the person, such as Jim.  This is defined in
            // section 3.1.3 of RFC 2426.  Multiple names can
            // be listed, separated by commas.

            if (card.Nicknames.Count <= 0) return;
            // A NICKNAME property is a comma-separated
            // list of values.  Create a value list and
            // add the nicknames collection to it.

            var values = new ValueCollection(',') {card.Nicknames};

            // Create the new properties with each name separated
            // by a comma.

            var property = new Property("NICKNAME", values);
            properties.Add(property);
        }
        #endregion

        #region BuildPropertiesNote
        /// <summary>
        ///     Builds the NOTE property.
        /// </summary>
        private static void BuildPropertiesNote(ICollection<Property> properties, VCard card)
        {
            foreach (var note in card.Notes)
            {
                if (string.IsNullOrEmpty(note.Text)) continue;
                var property = new Property {Name = "NOTE", Value = note.Text};

                if (!string.IsNullOrEmpty(note.Language))
                    property.Subproperties.Add("language", note.Language);

                property.Subproperties.Add("ENCODING", "QUOTED-PRINTABLE");
                properties.Add(property);
            }
        }
        #endregion

        #region BuildPropertiesOrg
        /// <summary>
        ///     Builds the ORG property.
        /// </summary>
        private static void BuildPropertiesOrg(ICollection<Property> properties, VCard card)
        {
            // The ORG property specifies the name of the
            // person's company or organization. Example:
            //
            // ORG:FairMetric LLC

            if (string.IsNullOrEmpty(card.Organization)) return;
            var property = new Property("ORG", card.Organization);

            properties.Add(property);
        }
        #endregion

        #region BuildPropertiesPhoto
        private void BuildPropertiesPhoto(ICollection<Property> properties, VCard card)
        {
            foreach (var photo in card.Photos)
            {
                if (photo.Url == null)
                {
                    // This photo does not have a URL associated
                    // with it.  Therefore a property can be
                    // generated only if the image data is loaded.
                    // Otherwise there is not enough information.

                    if (photo.IsLoaded)
                        properties.Add(new Property("PHOTO", photo.GetBytes()));
                }
                else
                {
                    // This photo has a URL associated with it.  The
                    // PHOTO property can either be linked as an image
                    // or embedded, if desired.

                    var doEmbedded = photo.Url.IsFile ? _embedLocalImages : _embedInternetImages;

                    if (doEmbedded)
                    {
                        // According to the settings of the card writer,
                        // this linked image should be embedded into the
                        // vCard data.  Attempt to fetch the data.

                        try
                        {
                            photo.Fetch();
                        }
                        catch
                        {
                            // An error was encountered.  The image can
                            // still be written as a link, however.

                            doEmbedded = false;
                        }
                    }

                    // At this point, doEmbedded is true only if (a) the
                    // writer was configured to embed the image, and (b)
                    // the image was successfully downloaded.

                    if (doEmbedded)
                        properties.Add(
                            new Property("PHOTO", photo.GetBytes()));
                    else
                    {
                        var uriPhotoProperty = new Property("PHOTO");

                        // Set the VALUE property to indicate that
                        // the data for the photo is a URI.
                        uriPhotoProperty.Subproperties.Add("VALUE", "URI");
                        uriPhotoProperty.Value = photo.Url.ToString();
                        properties.Add(uriPhotoProperty);
                    }
                }
            }
        }
        #endregion

        #region BuildPropertiesProdId
        /// <summary>
        ///     Builds PRODID properties.
        /// </summary>
        private static void BuildPropertiesProdId(ICollection<Property> properties, VCard card)
        {
            if (string.IsNullOrEmpty(card.ProductId)) return;
            var property = new Property {Name = "PRODID", Value = card.ProductId};
            properties.Add(property);
        }
        #endregion

        #region BuildPropertiesRev
        /// <summary>
        ///     Builds the REV property.
        /// </summary>
        private static void BuildPropertiesRev(ICollection<Property> properties, VCard card)
        {
            if (!card.RevisionDate.HasValue) return;
            var property = new Property("REV", card.RevisionDate.Value);

            properties.Add(property);
        }
        #endregion

        #region BuildPropertiesRole
        /// <summary>
        ///     Builds the ROLE property.
        /// </summary>
        private static void BuildPropertiesRole(ICollection<Property> properties, VCard card)
        {
            // The ROLE property identifies the role of
            // the person at his/her organization.

            if (!string.IsNullOrEmpty(card.Role))
            {
                var property =
                    new Property("ROLE", card.Role);

                properties.Add(property);
            }
        }
        #endregion

        #region BuildPropertiesSource
        /// <summary>
        ///     Builds SOURCE properties.
        /// </summary>
        private static void BuildPropertiesSource(ICollection<Property> properties, VCard card)
        {
            foreach (var source in card.Sources)
            {
                var property = new Property {Name = "SOURCE", Value = source.Uri.ToString()};

                if (!string.IsNullOrEmpty(source.Context))
                    property.Subproperties.Add("CONTEXT", source.Context);

                properties.Add(property);
            }
        }
        #endregion

        #region BuildPropertiesTel
        /// <summary>
        ///     Builds TEL properties.
        /// </summary>
        private static void BuildPropertiesTel(ICollection<Property> properties, VCard card)
        {
            // The TEL property indicates a telephone number of
            // the person (including non-voice numbers like fax
            // and BBS numbers).
            //
            // TEL;VOICE;WORK:1-800-929-5805
            foreach (var phone in card.Phones)
            {
                // A telephone entry has the property name TEL and
                // can have zero or more subproperties like FAX
                // or HOME.  Examples:
                //
                //   TEL;HOME:+1-612-555-1212
                //   TEL;FAX;HOME:+1-612-555-1212
                var property = new Property {Name = "TEL"};

                if (phone.IsBbs)
                    property.Subproperties.Add("BBS");

                if (phone.IsCar)
                    property.Subproperties.Add("CAR");

                if (phone.IsCellular)
                    property.Subproperties.Add("CELL");

                if (phone.IsFax)
                    property.Subproperties.Add("FAX");

                if (phone.IsHome)
                    property.Subproperties.Add("HOME");

                if (phone.IsIsdn)
                    property.Subproperties.Add("ISDN");

                if (phone.IsMessagingService)
                    property.Subproperties.Add("MSG");

                if (phone.IsModem)
                    property.Subproperties.Add("MODEM");

                if (phone.IsPager)
                    property.Subproperties.Add("PAGER");

                if (phone.IsPreferred)
                    property.Subproperties.Add("PREF");

                if (phone.IsVideo)
                    property.Subproperties.Add("VIDEO");

                if (phone.IsVoice)
                    property.Subproperties.Add("VOICE");

                if (phone.IsWork)
                    property.Subproperties.Add("WORK");

                property.Value = phone.FullNumber;
                properties.Add(property);
            }
        }
        #endregion

        #region BuildPropertiesTitle
        private static void BuildPropertiesTitle(ICollection<Property> properties, VCard card)
        {
            // The TITLE property specifies the job title of 
            // the person.  Example:
            //
            // TITLE:Systems Analyst
            // TITLE:President

            if (string.IsNullOrEmpty(card.Title)) return;
            var property = new Property("TITLE", card.Title);

            properties.Add(property);
        }
        #endregion

        #region BuildPropertiesTz
        private static void BuildPropertiesTz(ICollection<Property> properties, VCard card)
        {
            if (!string.IsNullOrEmpty(card.TimeZone))
                properties.Add(new Property("TZ", card.TimeZone));
        }
        #endregion

        #region BuildPropertiesUid
        private static void BuildPropertiesUid(ICollection<Property> properties, VCard card)
        {
            if (string.IsNullOrEmpty(card.UniqueId)) return;
            var property = new Property {Name = "UID", Value = card.UniqueId};
            properties.Add(property);
        }
        #endregion

        #region BuildPropertiesUrl
        private static void BuildPropertiesUrl(ICollection<Property> properties, VCard card)
        {
            foreach (var webSite in card.Websites)
            {
                if (string.IsNullOrEmpty(webSite.Url)) continue;
                var property = new Property("URL", webSite.Url);

                if (webSite.IsWorkSite)
                    property.Subproperties.Add("WORK");

                properties.Add(property);
            }
        }
        #endregion

        #region BuildPropertiesXWabGender
        private static void BuildPropertiesXWabGender(ICollection<Property> properties, VCard card)
        {
            // The X-WAB-GENDER property is an extended (custom)
            // property supported by Microsoft Outlook.
            switch (card.Gender)
            {
                case Gender.Female:
                    properties.Add(new Property("X-WAB-GENDER", "1"));
                    break;

                case Gender.Male:
                    properties.Add(new Property("X-WAB-GENDER", "2"));
                    break;
            }
        }
        #endregion

        #region EncodeBase64(byte)
        /// <summary>
        ///     Converts a byte to a BASE64 string.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public static string EncodeBase64(byte value)
        {
            return Convert.ToBase64String(new[] {value});
        }
        #endregion

        #region EncodeBase64(byte[])
        /// <summary>
        ///     Converts a byte array to a BASE64 string.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public static string EncodeBase64(byte[] value)
        {
            return Convert.ToBase64String(value);
        }
        #endregion

        #region EncodeBase64(int)
        /// <summary>
        ///     Converts an integer to a BASE64 string.
        /// </summary>
        public static string EncodeBase64(int value)
        {
            var buffer = new byte[4];

            buffer[0] = (byte) (value);
            buffer[1] = (byte) (value >> 8);
            buffer[2] = (byte) (value >> 16);
            buffer[3] = (byte) (value >> 24);

            return Convert.ToBase64String(buffer);
        }
        #endregion

        #region EncodeEscaped(string)
        /// <summary>
        ///     Encodes a string using simple escape codes.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public string EncodeEscaped(string value)
        {
            return EncodeEscaped(value, (Options & WriterOptions.IgnoreCommas) ==
                                        WriterOptions.IgnoreCommas
                ? _outlookEscapedCharacters
                : _standardEscapedCharacters);
        }
        #endregion

        #region EncodeEscaped(string, char[])
        /// <summary>
        ///     Encodes a character array using simple escape sequences.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public static string EncodeEscaped(string value, char[] escaped)
        {
            if (escaped == null)
                throw new ArgumentNullException("escaped");

            if (string.IsNullOrEmpty(value))
                return value;

            var buffer = new StringBuilder();

            var startIndex = 0;

            do
            {
                // Get the index of the next character
                // to be escaped (e.g. the next semicolon).

                var nextIndex = value.IndexOfAny(escaped, startIndex);

                if (nextIndex == -1)
                {
                    // No more characters need to be escaped.
                    // Any characters between the start index
                    // and the end of the string can be copied
                    // to the buffer.

                    buffer.Append(
                        value,
                        startIndex,
                        value.Length - startIndex);

                    break;
                }

                char replacement;
                switch (value[nextIndex])
                {
                    case '\n':
                        replacement = 'n';
                        break;

                    case '\r':
                        replacement = 'r';
                        break;

                    default:
                        replacement = value[nextIndex];
                        break;
                }

                buffer.Append(
                    value,
                    startIndex,
                    nextIndex - startIndex);

                buffer.Append('\\');
                buffer.Append(replacement);

                startIndex = nextIndex + 1;
            } while (startIndex < value.Length);

            return buffer.ToString();

            // The following must be encoded:
            //
            // Backslash (\\)
            // Colon (\:)
            // Semicolon (\;)
        }
        #endregion

        #region EncodeQuotedPrintable
        /// <summary>
        ///     Converts a string to quoted-printable format.
        /// </summary>
        /// <param name="value">
        ///     The value to encode in Quoted Printable format.
        /// </param>
        /// <returns>
        ///     The value encoded in Quoted Printable format.
        /// </returns>
        // ReSharper disable once MemberCanBePrivate.Global
        public static string EncodeQuotedPrintable(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            var builder = new StringBuilder();

            foreach (var c in value)
            {
                var v = (int) c;

                // The following are not required to be encoded:
                //
                // - Tab (ASCII 9)
                // - Space (ASCII 32)
                // - Characters 33 to 126, except for the equal sign (61).

                if (
                    (v == 9) ||
                    ((v >= 32) && (v <= 60)) ||
                    ((v >= 62) && (v <= 126)))
                {
                    builder.Append(c);
                }
                else
                {
                    builder.Append('=');
                    builder.Append(v.ToString("X2"));
                }
            }

            char lastChar = builder[builder.Length - 1];
            if (char.IsWhiteSpace(lastChar))
            {
                builder.Remove(builder.Length - 1, 1);
                builder.Append('=');
                builder.Append(((int) lastChar).ToString("X2"));
            }

            return builder.ToString();
        }
        #endregion

        #region EncodeProperty
        /// <summary>
        ///     Returns property encoded into a standard vCard NAME:VALUE format.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public string EncodeProperty(Property property)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            if (string.IsNullOrEmpty(property.Name))
                throw new ArgumentException();

            var builder = new StringBuilder();

            builder.Append(property.Name);

            foreach (var subproperty in property.Subproperties)
            {
                builder.Append(';');
                builder.Append(subproperty.Name);

                if (string.IsNullOrEmpty(subproperty.Value)) continue;
                builder.Append('=');
                builder.Append(subproperty.Value);
            }

            // The property name and all subproperties have been
            // written to the string builder (the colon separator
            // has not been written).  The next step is to write
            // the value.  Depending on the type of value and any
            // characters in the value, it may be necessary to
            // use an non-default encoding.  For example, byte arrays
            // are written encoded in BASE64.

            if (property.Value == null)
                builder.Append(':');
            else
            {
                var valueType = property.Value.GetType();

                if (valueType == typeof (byte[]))
                {
                    // A byte array should be encoded in BASE64 format.

                    builder.Append(";ENCODING=BASE64:");
                    builder.Append(EncodeBase64((byte[]) property.Value));
                }
                else if (valueType == typeof (ValueCollection))
                {
                    var values = (ValueCollection) property.Value;

                    builder.Append(':');
                    for (var index = 0; index < values.Count; index++)
                    {
                        builder.Append(EncodeEscaped(values[index]));
                        if (index < values.Count - 1)
                            builder.Append(values.Separator);
                    }
                }
                else
                {
                    // The object will be converted to a string (if it is
                    // not a string already) and encoded if necessary.
                    // The first step is to get the string value.

                    var stringValue = valueType == typeof (char[])
                        ? new string(((char[]) property.Value))
                        : property.Value.ToString();

                    builder.Append(':');

                    switch (property.Subproperties.GetValue("ENCODING"))
                    {
                        case "QUOTED-PRINTABLE":
                            builder.Append(EncodeQuotedPrintable(stringValue));
                            break;

                        default:
                            builder.Append(EncodeEscaped(stringValue));
                            break;
                    }
                }
            }

            return builder.ToString();
        }
        #endregion

        #region Write
        /// <summary>
        ///     Writes a vCard to an output text writer.
        /// </summary>
        public void Write(VCard card, TextWriter output)
        {
            if (card == null)
                throw new ArgumentNullException("card");

            if (output == null)
                throw new ArgumentNullException("output");

            // Get the properties of the vCard.
            var properties = BuildProperties(card);
            Write(properties, output);
        }

        /// <summary>
        ///     Writes a collection of vCard properties to an output text writer.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public void Write(ICollection<Property> properties, TextWriter output)
        {
            if (properties == null)
                throw new ArgumentNullException("properties");

            if (output == null)
                throw new ArgumentNullException("output");

            foreach (var property in properties)
                output.WriteLine(EncodeProperty(property));
        }
        #endregion
    }
}
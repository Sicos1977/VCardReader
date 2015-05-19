using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;

namespace VCardReader
{
    /// <summary>
    ///     Reads a vCard written in the standard 2.0 or 3.0 text formats.
    ///     This is the primary (standard) vCard format used by most applications.
    /// </summary>
    [SuppressMessage("ReSharper", "FunctionComplexityOverflow")]
    public class VCardReader
    {
        #region Enum QuotedPrintableState
        /// <summary>
        ///     The state of the quoted-printable decoder (private).
        /// </summary>
        /// <remarks>
        ///     The <see cref="DecodeQuotedPrintable(string)" /> function is a utility function that parses a string that
        ///     has been encoded with the QUOTED-PRINTABLE format. The function is implemented as a state-pased parser
        ///     where the state is updated after examining each character of the input string. This enumeration
        ///     defines the various states of the parser.
        /// </remarks>
        private enum QuotedPrintableState
        {
            None,
            ExpectingHexChar1,
            ExpectingHexChar2,
            ExpectingLineFeed
        }
        #endregion

        #region Fields
        /// <summary>
        ///     The DeliveryAddressTypeNames array contains the recognized TYPE values for an ADR (delivery address).
        /// </summary>
        // ReSharper disable once UnusedMember.Local
        private readonly string[] _deliveryAddressTypeNames =
        {
            "DOM", // Domestic address
            "INTL", // International address
            "POSTAL", // Postal address
            "PARCEL", // Parcel delivery address
            "HOME", // Home address
            "WORK", // Work address
            "PREF"
        }; // Preferred address

        /// <summary>
        ///     The PhoneTypeNames constant defines the recognized subproperty names that identify the category or
        ///     classification of a phone. The names are used with the TEL property.
        /// </summary>
        // ReSharper disable once UnusedMember.Local
        private readonly string[] _phoneTypeNames =
        {
            "BBS",
            "CAR",
            "CELL",
            "FAX",
            "HOME",
            "ISDN",
            "MODEM",
            "MSG",
            "PAGER",
            "PREF",
            "VIDEO",
            "VOICE",
            "WORK"
        };
        #endregion

        #region Properties
        /// <summary>
        ///     A collection of warning messages that were generated  during the output of a vCard.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public StringCollection Warnings { get; private set; }
        #endregion

        #region Constructor
        /// <summary>
        ///     Initializes a new instance of the <see cref="VCardReader" />.
        /// </summary>
        // ReSharper disable once EmptyConstructor
        public VCardReader()
        {
            Warnings = new StringCollection();
        }
        #endregion

        #region DecodeBase64(string)
        /// <summary>
        ///     Decodes a string containing BASE64 characters.
        /// </summary>
        /// <param name="value">
        ///     A string containing data that has been encoded with the BASE64 format.
        /// </param>
        /// <returns>
        ///     The decoded data as a byte array.
        /// </returns>
        public static byte[] DecodeBase64(string value)
        {
            // Currently the .NET implementation is acceptable. However, a different algorithm may be used in the future. 
            // For this reason callers should use this function instead of the FromBase64String function in .NET.
            // Performance is not an issue because the runtime engine will inline the code or eliminate the extra call.
            return Convert.FromBase64String(value);
        }
        #endregion

        #region DecodeBase64(char[])
        /// <summary>
        ///     Converts BASE64 data that has been stored in a character array.
        /// </summary>
        /// <param name="value">
        ///     A character array containing BASE64 data.
        /// </param>
        /// <returns>
        ///     A byte array containing the decoded BASE64 data.
        /// </returns>
        // ReSharper disable once UnusedMember.Global
        public static byte[] DecodeBase64(char[] value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return Convert.FromBase64CharArray(value, 0, value.Length);
        }
        #endregion

        #region DecodeEmailAddressType
        /// <summary>
        ///     Parses the name of an email address type.
        /// </summary>
        /// <param name="keyword">
        ///     The email address type keyword found in the vCard file (e.g. AOL or INTERNET).
        /// </param>
        /// <returns>
        ///     Null or the decoded <see cref="EmailAddressType" />.
        /// </returns>
        /// <seealso cref="EmailAddress" />
        /// <seealso cref="EmailAddressType" />
        public static EmailAddressType? DecodeEmailAddressType(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
                return null;

            switch (keyword.ToUpperInvariant())
            {
                case "INTERNET":
                    return EmailAddressType.Internet;

                case "AOL":
                    return EmailAddressType.AOl;

                case "APPLELINK":
                    return EmailAddressType.AppleLink;

                case "ATTMAIL":
                    return EmailAddressType.AttMail;

                case "CIS":
                    return EmailAddressType.CompuServe;

                case "EWORLD":
                    return EmailAddressType.EWorld;

                case "IBMMAIL":
                    return EmailAddressType.IBMMail;

                case "MCIMAIL":
                    return EmailAddressType.MCIMail;

                case "POWERSHARE":
                    return EmailAddressType.PowerShare;

                case "PRODIGY":
                    return EmailAddressType.Prodigy;

                case "TLX":
                    return EmailAddressType.Telex;

                case "X400":
                    return EmailAddressType.X400;

                default:
                    return null;
            }
        }
        #endregion

        #region DecodeEscaped
        /// <summary>
        ///     Decodes a string that has been encoded with the standard vCard escape codes.
        /// </summary>
        /// <param name="value">
        ///     A string encoded with vCard escape codes.
        /// </param>
        /// <returns>
        ///     The decoded string.
        /// </returns>
        public static string DecodeEscaped(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            var builder = new StringBuilder(value.Length);
            var startIndex = 0;

            do
            {
                // Get the index of the next backslash character.
                // This marks the beginning of an escape sequence.

                var nextIndex = value.IndexOf('\\', startIndex);

                if ((nextIndex == -1) || (nextIndex == value.Length - 1))
                {
                    // There are no more escape codes, or the backslash
                    // is located at the very end of the string.  The
                    // characters between the index and the end of the
                    // string need to be copied to the output buffer.

                    builder.Append(
                        value,
                        startIndex,
                        value.Length - startIndex);

                    break;
                }

                // A backslash was located somewhere in the string.
                // The previous statement ensured the backslash is
                // not the very last character, and therefore the
                // following statement is safe.

                char code = value[nextIndex + 1];

                // Any characters between the starting point and
                // the index must be pushed into the buffer.

                builder.Append(
                    value,
                    startIndex,
                    nextIndex - startIndex);

                switch (code)
                {
                    case '\\':
                    case ',':
                    case ';':

                        builder.Append(code);
                        nextIndex += 2;
                        break;

                    case 'n':
                    case 'N':
                        builder.Append('\n');
                        nextIndex += 2;
                        break;

                    case 'r':
                    case 'R':
                        builder.Append('\r');
                        nextIndex += 2;
                        break;

                    default:
                        builder.Append('\\');
                        builder.Append(code);
                        nextIndex += 2;
                        break;
                }

                startIndex = nextIndex;
            } while (startIndex < value.Length);

            return builder.ToString();
        }
        #endregion

        #region DecodeHexadecimal
        /// <summary>
        ///     Converts a single hexadecimal character to its integer value.
        /// </summary>
        /// <param name="value">
        ///     A Unicode character.
        /// </param>
        public static int DecodeHexadecimal(char value)
        {
            if (char.IsDigit(value))
                return Convert.ToInt32(char.GetNumericValue(value));

            // A = ASCII 65
            // F = ASCII 70
            // a = ASCII 97
            // f = ASCII 102

            if ((value >= 'A') && (value <= 'F'))
            {
                // The character is one of the characters
                // between 'A' (value 65) and 'F' (value 70).
                // The character "A" (hex) is "10" (decimal).

                return Convert.ToInt32(value) - 55;
            }

            if ((value >= 'a') && (value <= 'f'))
            {
                // The character is one of the characters
                // between 'a' (value 97) and 'f' (value 102).
                // The character "A" or "a" (hex) is "10" (decimal).

                return Convert.ToInt32(value) - 87;
            }

            // The specified character cannot be interpreted
            // as a written hexadecimal character.  Raise an
            // exception.

            throw new ArgumentOutOfRangeException("value");
        }
        #endregion

        #region DecodeQuotedPrintable
        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DecodeQuotedPrintable(string value)
        {
            return DecodeQuotedPrintable(value, Encoding.Default);
        }

        /// <summary>
        ///     Decodes a string that has been encoded in QUOTED-PRINTABLE format.
        /// </summary>
        /// <param name="value">
        ///     A string that has been encoded in QUOTED-PRINTABLE.
        /// </param>
        /// <param name="encoding">
        ///     charset encoding
        /// </param>
        /// <returns>
        ///     The decoded string.
        /// </returns>
        public static string DecodeQuotedPrintable(string value, Encoding encoding)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            var firstHexChar = '\x0';
            var state = QuotedPrintableState.None;

            var charList = new List<Char>();

            foreach (var c in value)
            {
                switch (state)
                {
                    case QuotedPrintableState.None:

                        // The parser is not expacting any particular
                        // type of character.  If the character is an
                        // equal sign (=), then this point in the string
                        // is the start of a character encoded in hexadecimal
                        // format.  There are two hexadecimal characters
                        // expected.

                        if (c == '=')
                            state = QuotedPrintableState.ExpectingHexChar1;
                        else
                            charList.Add(c);
                        break;

                    case QuotedPrintableState.ExpectingHexChar1:

                        // The parser previously encountered an equal sign.
                        // This has two purposes: it marks the beginning of
                        // a hexadecimal escape sequence, or it marks a
                        // so-called software end-of-line.

                        if (IsHexDigit(c))
                        {
                            // The next character is a hexadecimal character.
                            // Therefore the equal sign marks the beginning
                            // of an escape sequence.

                            firstHexChar = c;
                            state = QuotedPrintableState.ExpectingHexChar2;
                        }

                        else
                            switch (c)
                            {
                                case '\r':
                                    state = QuotedPrintableState.ExpectingLineFeed;
                                    break;
                                case '=':
                                    charList.Add('=');
                                    state = QuotedPrintableState.ExpectingHexChar1;
                                    break;
                                default:
                                    charList.Add('=');
                                    charList.Add(c);
                                    state = QuotedPrintableState.None;
                                    break;
                            }
                        break;

                    case QuotedPrintableState.ExpectingHexChar2:

                        // The parser previously encountered an equal
                        // sign and the first of two hexadecimal
                        // characters.  This character is expected to
                        // be the second (final) hexadecimal character.

                        if (IsHexDigit(c))
                        {
                            // Each hexadecimal character represents
                            // four bits of the encoded ASCII value.
                            // The first character was the upper 4 bits.

                            int charValue =
                                (DecodeHexadecimal(firstHexChar) << 4) +
                                DecodeHexadecimal(c);

                            charList.Add((char) charValue);

                            state = QuotedPrintableState.None;
                        }
                        else
                        {
                            // The parser was expecting the second
                            // hexadecimal character after the equal sign.
                            // Since this is not a hexadecimal character,
                            // the partial sequence is dumped to the output
                            // and skipped.

                            charList.Add('=');
                            charList.Add(firstHexChar);
                            charList.Add(c);
                            state = QuotedPrintableState.None;
                        }
                        break;

                    case QuotedPrintableState.ExpectingLineFeed:

                        // Previously the parser encountered an equal sign
                        // followed by a carriage-return.  This is an indicator
                        // to the decoder that the encoded value contains a 
                        // soft line break.  The line break is ignored.
                        // Per mime standards, the character following the
                        // carriage-return should be a line feed.

                        switch (c)
                        {
                            case '\n':
                                state = QuotedPrintableState.None;
                                break;
                            case '=':
                                state = QuotedPrintableState.ExpectingHexChar1;
                                break;
                            default:
                                charList.Add(c);
                                state = QuotedPrintableState.None;
                                break;
                        }

                        break;
                }
            }

            // The parser has examined each character in the input string.
            // In theory (for a correct string), the parser state should be
            // none -- that is, all codes were property terminated.  If not,
            // the partial codes should be flushed to the output.

            switch (state)
            {
                case QuotedPrintableState.ExpectingHexChar1:
                    charList.Add('=');
                    break;

                case QuotedPrintableState.ExpectingHexChar2:
                    charList.Add('=');
                    charList.Add(firstHexChar);
                    break;

                case QuotedPrintableState.ExpectingLineFeed:
                    charList.Add('=');
                    charList.Add('\r');
                    break;
            }

            var by = new byte[charList.Count];
            for (var i = 0; i < charList.Count; i++)
                by[i] = Convert.ToByte(charList[i]);

            var ret = encoding.GetString(by);

            return ret;
        }
        #endregion

        #region IsHexDigit
        /// <summary>
        ///     Indicates whether the specified character is a hexadecimal digit.
        /// </summary>
        /// <param name="value">
        ///     A unicode character
        /// </param>
        public static bool IsHexDigit(char value)
        {
            // First, see if the character is
            // a decimal digit.  All decimal digits
            // are also hexadecimal digits.

            if (char.IsDigit(value))

                return true;

            return
                ((value >= 'A') && (value <= 'F')) ||
                ((value >= 'a') && (value <= 'f'));
        }
        #endregion

        #region ParseDate
        /// <summary>
        ///     Parses a string containing a date/time value.
        /// </summary>
        /// <param name="value">
        ///     A string containing a date/time value.
        /// </param>
        /// <returns>
        ///     The parsed date, or null if no date could be parsed.
        /// </returns>
        /// <remarks>
        ///     Some revision dates, such as those generated by Outlook, are not directly supported by the .NET DateTime parser.
        ///     This function attempts to accomodate the non-standard formats.
        /// </remarks>
        public static DateTime? ParseDate(string value)
        {
            DateTime parsed;
            if (DateTime.TryParse(value, out parsed))
                return parsed;

            // Outlook generates a revision date like this:
            //
            //   20061130T234000Z
            //   |   | | || | ++------- Seconds (2 digits)
            //   |   | | || ++--------- Minutes (2 digits)
            //   |   | | |++----------- Hour (2 digits)
            //   |   | | +------------- T (literal)
            //   |   | ++-------------- Day (2 digits)
            //   |   ++---------------- Month (2 digits)             
            //   +--+------------------ Year (4 digits)
            //
            // This format does not seem to be recognized by the standard DateTime parser.
            // A custom string can be defined:
            //
            // yyyyMMdd\THHmmss\Z

            if (DateTime.TryParseExact(
                value,
                @"yyyyMMdd\THHmmss\Z",
                null,
                DateTimeStyles.AssumeUniversal,
                out parsed))
                return parsed;

            return null;
        }
        #endregion

        #region ParseEncoding
        /// <summary>
        ///     Parses an encoding name (such as "BASE64") and returns the corresponding <see cref="VCardEncoding" /> value.
        /// </summary>
        /// <param name="name">
        ///     The name of an encoding from a standard vCard property.
        /// </param>
        /// <returns>
        ///     The enumerated value of the encoding.
        /// </returns>
        public static VCardEncoding ParseEncoding(string name)
        {
            // If not specified, the default encoding (escaped) used by the vCard file specification is assumed.

            if (string.IsNullOrEmpty(name))
                return VCardEncoding.Unknown;

            switch (name.ToUpperInvariant())
            {
                case "B":
                    // Some vCard specification documents list the encoding name "b" instead of "base64".
                    return VCardEncoding.Base64;

                case "BASE64":
                    return VCardEncoding.Base64;

                case "QUOTED-PRINTABLE":
                    return VCardEncoding.QuotedPrintable;

                default:
                    return VCardEncoding.Unknown;
            }
        }
        #endregion

        #region ParsePhoneType
        /// <summary>
        ///     Parses the name of a phone type and returns the corresponding <see cref="PhoneTypes" /> value.
        /// </summary>
        /// <param name="name">
        ///     The name of a phone type from a TEL vCard property.
        /// </param>
        /// <returns>
        ///     The enumerated value of the phone type, or Default if the phone type could not be determined.
        /// </returns>
        public static PhoneTypes ParsePhoneType(string name)
        {
            if (string.IsNullOrEmpty(name))
                return PhoneTypes.Default;

            switch (name.Trim().ToUpperInvariant())
            {
                case "BBS":
                    return PhoneTypes.Bbs;

                case "CAR":
                    return PhoneTypes.Car;

                case "CELL":
                    return PhoneTypes.Cellular;

                case "FAX":
                    return PhoneTypes.Fax;

                case "HOME":
                    return PhoneTypes.Home;

                case "ISDN":
                    return PhoneTypes.Isdn;

                case "MODEM":
                    return PhoneTypes.Modem;

                case "MSG":
                    return PhoneTypes.MessagingService;

                case "PAGER":
                    return PhoneTypes.Pager;

                case "PREF":
                    return PhoneTypes.Preferred;

                case "VIDEO":
                    return PhoneTypes.Video;

                case "VOICE":
                    return PhoneTypes.Voice;

                case "WORK":
                    return PhoneTypes.Work;

                case "COMPANY":
                    return PhoneTypes.Company;

                case "CALLBACK":
                    return PhoneTypes.Callback;

                case "RADIO":
                    return PhoneTypes.Radio;

                case "ASSISTANT":
                    return PhoneTypes.Assistant;

                case "TTYTDD":
                    return PhoneTypes.Ttytdd;

                default:
                    return PhoneTypes.Default;
            }
        }

        /// <summary>
        ///     Decodes the bitmapped phone type given an array of phone type names.
        /// </summary>
        /// <param name="names">
        ///     An array containing phone type names such as BBS or VOICE.
        /// </param>
        /// <returns>
        ///     The phone type value that represents the combination of all names defined in the array.  
        ///     Unknown names are ignored.
        /// </returns>
        public static PhoneTypes ParsePhoneType(string[] names)
        {
            var sum = PhoneTypes.Default;

            foreach (var name in names)
                sum |= ParsePhoneType(name);

            return sum;
        }
        #endregion

        #region ParseDeliveryAddressType(string)
        /// <summary>
        ///     Parses the type of postal address.
        /// </summary>
        /// <param name="value">
        ///     The single value of a TYPE subproperty for the ADR property.
        /// </param>
        /// <returns>
        ///     The <see cref="DeliveryAddressTypes" /> that corresponds with the TYPE keyword, or vCardPostalAddressType.
        ///     Default if the type could not be identified.
        /// </returns>
        public static DeliveryAddressTypes ParseDeliveryAddressType(string value)
        {
            if (string.IsNullOrEmpty(value))
                return DeliveryAddressTypes.Default;

            switch (value.ToUpperInvariant())
            {
                case "DOM":
                    return DeliveryAddressTypes.Domestic;

                case "HOME":
                    return DeliveryAddressTypes.Home;

                case "INTL":
                    return DeliveryAddressTypes.International;

                case "PARCEL":
                    return DeliveryAddressTypes.Parcel;

                case "POSTAL":
                    return DeliveryAddressTypes.Postal;

                case "WORK":
                    return DeliveryAddressTypes.Work;

                default:
                    return DeliveryAddressTypes.Default;
            }
        }
        #endregion

        #region ParseDeliveryAddressType(string[])
        /// <summary>
        ///     Parses a string array containing one or more postal address types.
        /// </summary>
        /// <param name="typeNames">
        ///     A string array containing zero or more keywords used with the TYPE subproperty of the ADR property.
        /// </param>
        /// <returns>
        ///     A <see cref="DeliveryAddressTypes" /> flags enumeration that corresponds with all known type names from the array.
        /// </returns>
        public static DeliveryAddressTypes ParseDeliveryAddressType(string[] typeNames)
        {
            var allTypes = DeliveryAddressTypes.Default;

            foreach (var typeName in typeNames)
                allTypes |= ParseDeliveryAddressType(typeName);

            return allTypes;
        }
        #endregion

        #region ReadInto(vCard, TextReader)
        /// <summary>
        ///     Reads a vCard (VCF) file from an input stream.
        /// </summary>
        /// <param name="card">
        ///     An initialized vCard.
        /// </param>
        /// <param name="reader">
        ///     A text reader pointing to the beginning of a standard vCard file.
        /// </param>
        /// <returns>
        ///     The vCard with values updated from the file.
        /// </returns>
        public void ReadInto(VCard card, TextReader reader)
        {
            Property property;

            do
            {
                property = ReadProperty(reader);
                if (property == null) continue;
                if (
                    (string.Compare("END", property.Name, StringComparison.OrdinalIgnoreCase) == 0) &&
                    (string.Compare("VCARD", property.ToString(), StringComparison.OrdinalIgnoreCase) == 0))
                {
                    // This is a special type of property that marks
                    // the last property of the vCard. 

                    break;
                }
                ReadInto(card, property);
            } while (property != null);
        }
        #endregion

        #region ReadInto(vCard, vCardProperty)
        /// <summary>
        ///     Updates a vCard object based on the contents of a vCardProperty.
        /// </summary>
        /// <param name="card">
        ///     An initialized vCard object.
        /// </param>
        /// <param name="property">
        ///     An initialized vCardProperty object.
        /// </param>
        /// <remarks>
        ///     <para>
        ///         This method examines the contents of a property and attempts to update an existing vCard based on
        ///         the property name and value. This function must be updated when new vCard properties are implemented.
        ///     </para>
        /// </remarks>
        public void ReadInto(VCard card, Property property)
        {
            if (card == null)
                throw new ArgumentNullException("card");

            if (property == null)
                throw new ArgumentNullException("property");

            if (string.IsNullOrWhiteSpace(property.Name))
                return;

            switch (property.Name.ToUpperInvariant())
            {
                case "ADR":
                    ReadIntoAdr(card, property);
                    break;

                case "BDAY":
                    ReadIntoBday(card, property);
                    break;
               
                case "CATEGORIES":
                    ReadIntoCategories(card, property);
                    break;

                case "CLASS":
                    ReadIntoClass(card, property);
                    break;

                case "EMAIL":
                    ReadIntoEmail(card, property);
                    break;

                case "FN":
                    ReadIntoFn(card, property);
                    break;

                case "GEO":
                    ReadIntoGeo(card, property);
                    break;

                case "KEY":
                    ReadIntoKey(card, property);
                    break;

                case "LABEL":
                    ReadIntoLabel(card, property);
                    break;

                case "MAILER":
                    ReadIntoMailer(card, property);
                    break;

                case "N":
                    ReadIntoN(card, property);
                    break;

                case "NAME":
                    ReadIntoName(card, property);
                    break;

                case "NICKNAME":
                    ReadIntoNickName(card, property);
                    break;

                case "NOTE":
                    ReadIntoNote(card, property);
                    break;

                case "ORG":
                    ReadIntoOrg(card, property);
                    break;

                case "PHOTO":
                case "X-MS-CARDPICTURE":
                    ReadIntoPhoto(card, property);
                    break;

                case "PRODID":
                    ReadIntoProdId(card, property);
                    break;

                case "REV":
                    ReadIntoRev(card, property);
                    break;

                case "ROLE":
                    ReadIntoRole(card, property);
                    break;

                case "SOURCE":
                    ReadIntoSource(card, property);
                    break;

                case "TEL":
                    ReadIntoTel(card, property);
                    break;

                case "X-MS-TEL":
                    ReadIntoTel(card, property);
                    break;

                case "TITLE":
                    ReadIntoTitle(card, property);
                    break;

                case "TZ":
                    ReadIntoTz(card, property);
                    break;

                case "UID":
                    ReadIntoUid(card, property);
                    break;

                case "URL":
                    ReadIntoUrl(card, property);
                    break;

                case "X-WAB-GENDER":
                    ReadIntoXWabGender(card, property);
                    break;

                case "X-MS-ANNIVERSARY":
                    ReadIntoAnniversary(card, property);
                    break;

                case "X-MS-IMADDRESS":
                    ReadIntoInstantMessagingAddress(card, property);
                    break;

                case "X-MS-MANAGER":
                    ReadIntoManager(card, property);
                    break;

                case "X-MS-ASSISTANT":
                    ReadIntoAssistant(card, property);
                    break;
                    
                case "X-MS-SPOUSE":
                    ReadIntoSpouse(card, property);
                    break;

                default:
                    // The property name is not recognized and
                    // will be ignored.
                    break;
            }
        }
        #endregion

        #region ReadIntoAdr
        /// <summary>
        ///     Reads an ADR property.
        /// </summary>
        private void ReadIntoAdr(VCard card, Property property)
        {
            // The ADR property defines a delivery address, such as a home postal address.  
            // The property contains the following components separated by semicolons:
            //
            //   0. Post office box
            //   1. Extended address
            //   2. Street
            //   3. Locality (e.g. city)
            //   4. Region (e.g. state or province)
            //   5. Postal code
            //   6. Country name
            //
            // This version of the reader ignores any ADR properties with a lesser number of components.  
            // If more than 7 components exist, then the lower seven components are assumed to still match 
            // the specification (e.g. the additional components may be from a future specification).

            var addressParts =
                property.Value.ToString().Split(';');

            var deliveryAddress = new DeliveryAddress();

            if (addressParts.Length >= 7)
                deliveryAddress.Country = addressParts[6].Trim();

            if (addressParts.Length >= 6)
                deliveryAddress.PostalCode = addressParts[5].Trim();

            if (addressParts.Length >= 5)
                deliveryAddress.Region = addressParts[4].Trim();

            if (addressParts.Length >= 4)
                deliveryAddress.City = addressParts[3].Trim();

            if (addressParts.Length >= 3)
                deliveryAddress.Street = addressParts[2].Trim();

            if (
                (string.IsNullOrEmpty(deliveryAddress.City)) &&
                (string.IsNullOrEmpty(deliveryAddress.Country)) &&
                (string.IsNullOrEmpty(deliveryAddress.PostalCode)) &&
                (string.IsNullOrEmpty(deliveryAddress.Region)) &&
                (string.IsNullOrEmpty(deliveryAddress.Street)))
            {
                // No address appears to be defined.
                // Ignore.

                return;
            }

            // Handle the old 2.1 format in which the ADR type names (e.g.
            // DOM, HOME, etc) were written directly as subproperties.
            // For example, "ADR;HOME;POSTAL:...".

            deliveryAddress.AddressType =
                ParseDeliveryAddressType(property.Subproperties.GetNames(_deliveryAddressTypeNames));

            // Handle the new 3.0 format in which the delivery address
            // type is a comma-delimited list, e.g. "ADR;TYPE=HOME,POSTAL:".
            // It is possible for the TYPE subproperty to be listed multiple
            // times (this is allowed by the RFC, although irritating that
            // the authors allowed it).

            foreach (var subproperty in property.Subproperties)
            {
                // If this subproperty is a TYPE subproperty and
                // has a non-null value, then parse it.

                if (
                    (!string.IsNullOrEmpty(subproperty.Value)) &&
                    (string.Compare("TYPE", subproperty.Name, StringComparison.OrdinalIgnoreCase) == 0))
                {
                    deliveryAddress.AddressType |=
                        ParseDeliveryAddressType(subproperty.Value.Split(new[] {','}));
                }
            }
            card.DeliveryAddresses.Add(deliveryAddress);
        }
        #endregion

        #region ReadIntoBday
        /// <summary>
        ///     Reads the BDAY property.
        /// </summary>
        private static void ReadIntoBday(VCard card, Property property)
        {
            DateTime bday;
            if (DateTime.TryParse(property.ToString(), out bday))
                card.BirthDate = bday;
            else
            {
                // Microsoft Outlook writes the birthdate in YYYYMMDD, e.g. 20091015 for October 15, 2009.

                if (DateTime.TryParseExact(
                    property.ToString(),
                    "yyyyMMdd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out bday))
                    card.BirthDate = bday;
                else
                    card.BirthDate = null;
            }
        }
        #endregion

        #region ReadIntoAnniversary
        /// <summary>
        ///     Reads the X-MS-ANNIVERSARY property.
        /// </summary>
        /// <remarks>
        ///     Only available when the contact card has been generated with Microsoft Outlook
        /// </remarks>
        private static void ReadIntoAnniversary(VCard card, Property property)
        {
            DateTime anniversary;

            if (DateTime.TryParseExact(property.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out anniversary))
                card.Anniversary = anniversary;
            else
                card.Anniversary = null;
        }
        #endregion

        #region ReadIntoCategories
        /// <summary>
        ///     Reads the CATEGORIES property.
        /// </summary>
        private static void ReadIntoCategories(VCard card, Property property)
        {
            // The CATEGORIES value is expected to be a comma-delimited list.
            var cats = property.ToString().Split(new[] {','});

            // Add each non-blank line to the categories collection.
            foreach (var cat in cats)
            {
                if (cat.Length > 0)
                    card.Categories.Add(cat);
            }
        }
        #endregion

        #region ReadIntoClass
        /// <summary>
        ///     Reads the CLASS property.
        /// </summary>
        private static void ReadIntoClass(VCard card, Property property)
        {
            if (property.Value == null)
                return;

            switch (property.ToString().ToUpperInvariant())
            {
                case "PUBLIC":
                    card.AccessClassification = AccessClassification.Public;
                    break;

                case "PRIVATE":
                    card.AccessClassification = AccessClassification.Private;
                    break;

                case "CONFIDENTIAL":
                    card.AccessClassification = AccessClassification.Confidential;
                    break;
            }
        }
        #endregion

        #region ReadIntoEmail
        /// <summary>
        ///     Reads an EMAIL property.
        /// </summary>
        private static void ReadIntoEmail(VCard card, Property property)
        {
            var email = new EmailAddress {Address = property.Value.ToString()};

            // The email address is stored as the value of the property.
            // The format of the address depends on the type of email
            // address. The current version of the library does not perform any validation.

            // Loop through each subproperty and look for flags that indicate the type of email address.

            foreach (var subproperty in property.Subproperties)
            {
                switch (subproperty.Name.ToUpperInvariant())
                {
                    case "PREF":

                        // The PREF subproperty indicates the email address is the preferred email address to
                        // use when contacting the person.

                        email.IsPreferred = true;
                        break;

                    case "TYPE":

                        // The TYPE subproperty is new in vCard 3.0. It identifies the type and can also indicate
                        // the PREF attribute.

                        var typeValues =
                            subproperty.Value.Split(new[] {','});

                        foreach (var typeValue in typeValues)
                        {
                            if (string.Compare("PREF", typeValue, StringComparison.OrdinalIgnoreCase) == 0)
                                email.IsPreferred = true;
                            else
                            {
                                var typeType =
                                    DecodeEmailAddressType(typeValue);

                                if (typeType.HasValue)
                                    email.EmailType = typeType.Value;
                            }
                        }
                        break;

                    default:

                        // All other subproperties are probably vCard 2.1 subproperties.  
                        // This was before the email type was supposed to be specified with TYPE=VALUE.

                        var emailType =
                            DecodeEmailAddressType(subproperty.Name);

                        if (emailType.HasValue)
                            email.EmailType = emailType.Value;

                        break;
                }
            }
            card.EmailAddresses.Add(email);
        }
        #endregion

        #region ReadIntoFn
        /// <summary>
        ///     Reads the FN property.
        /// </summary>
        private static void ReadIntoFn(VCard card, Property property)
        {
            // The FN property defines the formatted display name of the person. This is used for presentation.

            card.FormattedName = property.Value.ToString();
        }
        #endregion

        #region ReadIntoGeo
        /// <summary>
        ///     Reads the GEO property.
        /// </summary>
        private static void ReadIntoGeo(VCard card, Property property)
        {
            // The GEO property specifies latitude and longitude of the entity associated with the vCard.

            var coordinates =
                property.Value.ToString().Split(new[] {';'});

            if (coordinates.Length == 2)
            {
                float geoLatitude;
                float geoLongitude;

                if (
                    float.TryParse(coordinates[0], out geoLatitude) &&
                    float.TryParse(coordinates[1], out geoLongitude))
                {
                    card.Latitude = geoLatitude;
                    card.Longitude = geoLongitude;
                }
            }
        }
        #endregion

        #region ReadIntoInstantMessagingAddress
        /// <summary>
        ///     Reads the X-MS-IMADDRESS property.
        /// </summary>
        private static void ReadIntoInstantMessagingAddress(VCard card, Property property)
        {
            card.InstantMessagingAddress = property.Value.ToString();
        }
        #endregion

        #region ReadIntoKey
        /// <summary>
        ///     Reads the KEY property.
        /// </summary>
        private static void ReadIntoKey(VCard card, Property property)
        {
            // The KEY property defines a security certificate
            // that has been attached to the vCard.  Key values
            // are usually encoded in BASE64 because they
            // often consist of binary data.

            var certificate = new Certificate {Data = (byte[]) property.Value};

            // TODO: Support other key types.

            if (property.Subproperties.Contains("X509"))
                certificate.KeyType = "X509";

            card.Certificates.Add(certificate);
        }
        #endregion

        #region ReadIntoLabel
        /// <summary>
        ///     Reads the LABEL property.
        /// </summary>
        private void ReadIntoLabel(VCard card, Property property)
        {
            var deliveryLabel = new DeliveryLabel
            {
                Text = property.Value.ToString(),
                AddressType = ParseDeliveryAddressType(property.Subproperties.GetNames(_deliveryAddressTypeNames))
            };

            // Handle the old 2.1 format in which the ADR type names (e.g. DOM, HOME, etc) were written directly as subproperties.
            // For example, "LABEL;HOME;POSTAL:...".

            // Handle the new 3.0 format in which the delivery address type is a comma-delimited list, e.g. "ADR;TYPE=HOME,POSTAL:".
            // It is possible for the TYPE subproperty to be listed multiple times (this is allowed by the RFC, although irritating that
            // the authors allowed it).

            foreach (var subproperty in property.Subproperties)
            {
                // If this subproperty is a TYPE subproperty and has a non-null value, then parse it.
                if (
                    (!string.IsNullOrEmpty(subproperty.Value)) &&
                    (string.Compare("TYPE", subproperty.Name, StringComparison.OrdinalIgnoreCase) == 0))
                {
                    deliveryLabel.AddressType |=
                        ParseDeliveryAddressType(subproperty.Value.Split(','));
                }
            }

            card.DeliveryLabels.Add(deliveryLabel);
        }
        #endregion

        #region ReadIntoMailer
        /// <summary>
        ///     Reads the MAILER property.
        /// </summary>
        private static void ReadIntoMailer(VCard card, Property property)
        {
            // The MAILER property identifies the mail software/ used by the person.  
            // This can be examined by a  program to detect software-specific conventions.
            // See section 2.4.3 of the vCard 2.1 spec. This property is not common.
            card.Mailer = property.Value.ToString();
        }
        #endregion

        #region ReadIntoN
        /// <summary>
        ///     Reads the N property.
        /// </summary>
        private static void ReadIntoN(VCard card, Property property)
        {
            // The N property defines the name of the person. The propery value has several components, such as the
            // given name, family name, and suffix. This is a  core field found in almost all vCards.
            //
            // Each component is supposed to be separated with a semicolon.  However, some vCard writers do not
            // write out training semicolons. For example, the last two components are the prefix (e.g. Mr.)
            // and suffix (e.g. Jr) of the name. The semicolons will be missing in some vCards if these components
            // are blank.

            var names = property.ToString().Split(';');

            // The first value is the family (last) name.
            card.FamilyName = names[0];
            if (names.Length == 1)
                return;

            // The next value is the given (first) name.
            card.GivenName = names[1];
            if (names.Length == 2)
                return;

            // The next value contains the middle name.
            card.AdditionalNames = names[2];
            if (names.Length == 3)
                return;

            // The next value contains the prefix, e.g. Mr.
            card.NamePrefix = names[3];
            if (names.Length == 4)
                return;

            // The last value contains the suffix, e.g. Jr.
            card.NameSuffix = names[4];
        }
        #endregion

        #region ReadIntoName
        /// <summary>
        ///     Reads the NAME property.
        /// </summary>
        private static void ReadIntoName(VCard card, Property property)
        {
            // The NAME property is used to define the displayable
            // name of the vCard.  Because it is intended for display
            // purposes, any whitespace at the beginning or end of
            // the name is trimmed.
            card.DisplayName = property.ToString().Trim();
        }
        #endregion

        #region ReadIntoNickName
        /// <summary>
        ///     Reads the NICKNAME property.
        /// </summary>
        private static void ReadIntoNickName(VCard card, Property property)
        {
            if (property.Value == null)
                return;

            // The nicknames are comma-separated values.
            var nicknames =
                property.Value.ToString().Split(new[] {','});

            foreach (var nickname in nicknames)
            {
                var trimmedNickname = nickname.Trim();
                if (trimmedNickname.Length > 0)
                    card.Nicknames.Add(trimmedNickname);
            }
        }
        #endregion

        #region ReadIntoNote
        /// <summary>
        ///     Reads the NOTE property.
        /// </summary>
        private static void ReadIntoNote(VCard card, Property property)
        {
            if (property.Value != null)
            {
                var note = new Note
                {
                    Language = property.Subproperties.GetValue("language"),
                    Text = property.Value.ToString()
                };

                if (!string.IsNullOrEmpty(note.Text))
                    card.Notes.Add(note);
            }
        }
        #endregion

        #region ReadIntoOrg
        /// <summary>
        ///     Reads the ORG property.
        /// </summary>
        private static void ReadIntoOrg(VCard card, Property property)
        {
            // The ORG property contains the name of the company
            // or organization of the person.
            card.Organization = property.Value.ToString();
        }
        #endregion

        #region ReadIntoPhoto
        /// <summary>
        ///     Reads the PHOTO property.
        /// </summary>
        private static void ReadIntoPhoto(VCard card, Property property)
        {
            // The PHOTO property contains an embedded (encoded) image or a link to an image.  
            // A URL (linked) image is supposed to be indicated with the VALUE=URI subproperty.

            var valueType = property.Subproperties.GetValue("VALUE");

            card.Photos.Add(
                string.Compare(valueType, "URI", StringComparison.OrdinalIgnoreCase) == 0
                    ? new Photo(new Uri(property.ToString()))
                    : new Photo((byte[]) property.Value));
        }
        #endregion

        #region ReadIntoProdId
        /// <summary>
        ///     Reads the PRODID property.
        /// </summary>
        private static void ReadIntoProdId(VCard card, Property property)
        {
            // The PRODID property contains the name of the
            // software that generated the vCard.  This is not
            // a common property.  Also note: this library
            // does not automatically generate a PRODID when
            // creating a vCard file.  The developer can set
            // the PRODID (via the ProductId parameter) to
            // anything desired.
            card.ProductId = property.ToString();
        }
        #endregion

        #region ReadIntoRev
        /// <summary>
        ///     Reads the REV property.
        /// </summary>
        private static void ReadIntoRev(VCard card, Property property)
        {
            // The REV property indicates the last revision date
            // of the vCard.  Note that Outlook and perhaps other
            // clients generate the revision date in a format not
            // recognized directly by the .NET DateTime parser.
            // A custom format is used; see ParseDate for details.
            card.RevisionDate = ParseDate(property.Value.ToString());
        }
        #endregion

        #region ReadIntoRole
        /// <summary>
        ///     Reads the ROLE property.
        /// </summary>
        private static void ReadIntoRole(VCard card, Property property)
        {
            // The ROLE property describes the role of the
            // person at his/her organization (e.g. Programmer
            // or Executive, etc).

            card.Role = property.Value.ToString();
        }
        #endregion

        #region ReadIntoSource
        /// <summary>
        ///     Reads the SOURCE property.
        /// </summary>
        private static void ReadIntoSource(VCard card, Property property)
        {
            // The SOURCE property identifies the source of
            // directory information (e.g. an LDAP server).  This
            // is not widely supported.  See RFC 2425, sec. 6.1.

            var source = new Source
            {
                Context = property.Subproperties.GetValue("CONTEXT"),
                Uri = new Uri(property.Value.ToString())
            };
            card.Sources.Add(source);
        }
        #endregion

        #region ReadIntoTel
        /// <summary>
        ///     Reads the TEL property.
        /// </summary>
        private static void ReadIntoTel(VCard card, Property property)
        {
            var phone = new Phone {FullNumber = property.ToString()};

            // The full telephone number is stored as the 
            // value of the property. Currently no formatted
            // rules are applied since the vCard specification
            // is somewhat confusing on this matter.
            if (string.IsNullOrEmpty(phone.FullNumber))
                return;

            foreach (var subproperty in property.Subproperties)
            {
                // If this subproperty is a TYPE subproperty
                // and it has a value, then it is expected
                // to contain a comma-delimited list of phone types.
                if (
                    (string.Compare(subproperty.Name, "TYPE", StringComparison.OrdinalIgnoreCase) == 0) &&
                    (!string.IsNullOrEmpty(subproperty.Value)))
                {
                    // This is a vCard 3.0 subproperty. It defines the
                    // the list of phone types in a comma-delimited list.
                    // Note that the vCard specification allows for
                    // multiple TYPE subproperties (why ?!).
                    phone.PhoneType |=
                        ParsePhoneType(subproperty.Value.Split(','));
                }
                else
                {
                    // The other subproperties in a TEL property
                    // define the phone type.  The only exception
                    // are meta fields like ENCODING, CHARSET, etc,
                    // but these are probably rare with TEL.

                    phone.PhoneType |= ParsePhoneType(subproperty.Name);
                }
            }

            card.Phones.Add(phone);
        }
        #endregion

        #region ReadIntoTitle
        /// <summary>
        ///     Reads the TITLE property.
        /// </summary>
        private static void ReadIntoTitle(VCard card, Property property)
        {
            // The TITLE property defines the job title of the
            // person.  This should not be confused by the name
            // prefix (e.g. "Mr"), which is called "Title" in
            // some vCard-compatible software like Outlook.
            card.Title = property.ToString();
        }
        #endregion

        #region ReadIntoTz
        /// <summary>
        ///     Reads a TZ property.
        /// </summary>
        private static void ReadIntoTz(VCard card, Property property)
        {
            card.TimeZone = property.ToString();
        }
        #endregion

        #region ReadIntoUid
        /// <summary>
        ///     Reads the UID property.
        /// </summary>
        private static void ReadIntoUid(VCard card, Property property)
        {
            card.UniqueId = property.ToString();
        }
        #endregion

        #region ReadIntoUrl
        /// <summary>
        ///     Reads the URL property.
        /// </summary>
        private static void ReadIntoUrl(VCard card, Property property)
        {
            var webSite = new Website {Url = property.ToString()};

            if (property.Subproperties.Contains("HOME"))
                webSite.IsPersonalSite = true;

            if (property.Subproperties.Contains("WORK"))
                webSite.IsWorkSite = true;

            card.Websites.Add(webSite);
        }
        #endregion

        #region ReadIntoXWabGender
        /// <summary>
        ///     Reads the X-WAB-GENDER property.
        /// </summary>
        private static void ReadIntoXWabGender(VCard card, Property property)
        {
            // The X-WAB-GENDER property is a custom property generated by
            // Microsoft Outlook 2003.  It contains the value 1 for females
            // or 2 for males.  It is not known if other PIM clients 
            // recognize this value.

            int genderId;

            if (!int.TryParse(property.ToString(), out genderId)) return;
            switch (genderId)
            {
                case 1:
                    card.Gender = Gender.Female;
                    break;

                case 2:
                    card.Gender = Gender.Male;
                    break;
            }
        }
        #endregion

        #region ReadIntoManager
        /// <summary>
        ///     Reads a X-MS-MANAGER property.
        /// </summary>
        private static void ReadIntoManager(VCard card, Property property)
        {
            card.Manager = property.ToString();
        }
        #endregion

        #region ReadIntoAssistant
        /// <summary>
        ///     Reads a X-MS-ASSISTANT property.
        /// </summary>
        private static void ReadIntoAssistant(VCard card, Property property)
        {
            card.Assistant = property.ToString();
        }
        #endregion

        #region ReadIntoSpouse
        /// <summary>
        ///     Reads a X-MS-SPOUSE property.
        /// </summary>
        private static void ReadIntoSpouse(VCard card, Property property)
        {
            card.Spouse = property.ToString();
        }
        #endregion

        #region ReadProperty(string)
        /// <summary>
        ///     Reads a property from a string.
        /// </summary>
        // ReSharper disable UnusedMember.Global
        public Property ReadProperty(string text)
            // ReSharper restore UnusedMember.Global
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");

            using (var reader = new StringReader(text))
            {
                return ReadProperty(reader);
            }
        }
        #endregion

        #region ReadProperty(TextReader)
        /// <summary>
        ///     Reads a property from a text reader.
        /// </summary>
        // ReSharper disable MemberCanBePrivate.Global
        public Property ReadProperty(TextReader reader)
            // ReSharper restore MemberCanBePrivate.Global
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            do
            {
                // Read the first line of the next property
                // from the input stream.  If a null string
                // is returned, then the end of the input
                // stream has been reached.

                var firstLine = reader.ReadLine();
                if (firstLine == null)
                    return null;

                // See if this line is a blank line.  It is
                // blank if (a) it has no characters, or (b)
                // it consists of whitespace characters only.

                firstLine = firstLine.Trim();
                if (firstLine.Length == 0)
                {
                    Warnings.Add(
                        "Line {0} A blank line was encountered.  This is not allowed in the vCard specification.");
                    continue;
                }

                // Get the index of the colon (:) in this
                // property line.  All vCard properties are
                // written in NAME:VALUE format.

                var colonIndex = firstLine.IndexOf(':');
                if (colonIndex == -1)
                {
                    Warnings.Add("Line {0}: A colon (:) is missing.  All properties must be in NAME:VALUE format.");
                    continue;
                }

                // Get the name portion of the property.  This
                // portion contains the property name as well
                // as any subproperties.

                var namePart = firstLine.Substring(0, colonIndex).Trim();
                if (string.IsNullOrEmpty(namePart))
                {
                    Warnings.Add("Line {0}: The name section of the property is empty.");
                    continue;
                }

                // Split apart the name portion of the property.
                // A property can have subproperties, separated
                // by semicolons.

                var nameParts = namePart.Split(';');
                for (var i = 0; i < nameParts.Length; i++)
                    nameParts[i] = nameParts[i].Trim();

                // The name of the property is supposed to
                // be first on the line.  An empty name is not
                // legal syntax.

                if (nameParts[0].Length == 0)
                {
                    Warnings.Add("Line {0}: The name section of the property is empty.");
                    continue;
                }

                // At this point there is sufficient text
                // to define a vCard property.  The only
                // true minimum requirement is a name.

                var property = new Property {Name = nameParts[0]};

                // Next, store any subproperties.  Subproperties
                // are defined like "NAME;SUBNAME=VALUE:VALUE".  Note
                // that subproperties do not necessarily have to have
                // a subvalue.

                for (var index = 1; index < nameParts.Length; index++)
                {
                    // Split the subproperty into its name and 
                    // value components.  If multiple equal signs
                    // happen to exist, they are interpreted as
                    // part of the value.  This may change in a 
                    // future version of the parser.

                    var subNameValue =
                        nameParts[index].Split(new[] {'='}, 2);

                    if (subNameValue.Length == 1)
                    {
                        // The Split function above returned a single
                        // array element.  This means no equal (=) sign
                        // was present.  The subproperty consists of
                        // a name only.

                        property.Subproperties.Add(
                            nameParts[index].Trim());
                    }
                    else
                    {
                        property.Subproperties.Add(
                            subNameValue[0].Trim(),
                            subNameValue[1].Trim());
                    }
                }

                // The subproperties have been defined.  The next
                // step is to try to identify the encoding of the
                // value.  The encoding is supposed to be specified
                // with a subproperty called ENCODING.  However, older
                // versions of the format just wrote the plain
                // encoding value, e.g. "NAME;BASE64:VALUE" instead
                // of the normalized "NAME;ENCODING=BASE64:VALUE" form.

                var encodingName =
                    property.Subproperties.GetValue("ENCODING",
                        new[] {"B", "BASE64", "QUOTED-PRINTABLE"});

                var hasCharset = property.Subproperties.Contains("CHARSET");
                var charsetEncoding = Encoding.Default;
                if (hasCharset)
                {
                    var charsetEncodingName = property.Subproperties.GetValue("CHARSET");
                    charsetEncoding = GetCharsetEncoding(charsetEncodingName);
                }

                // Convert the encoding name into its corresponding
                // vCardEncoding enumeration value.

                var encoding =
                    ParseEncoding(encodingName);

                // At this point, the first line of the property has been
                // loaded and the suggested value encoding has been
                // determined.  Get the raw value as encoded in the file.

                var rawValue = firstLine.Substring(colonIndex + 1);

                // The vCard specification allows long values
                // to be folded across multiple lines.  An example
                // is a security key encoded in MIME format.
                // When folded, each subsequent line begins with
                // a space or tab character instead of the next property.
                //
                // See: RFC 2425, Section 5.8.1

                do
                {
                    var peekChar = reader.Peek();

                    if ((peekChar == 32) || (peekChar == 9))
                    {
                        var foldedLine = reader.ReadLine();
                        if (foldedLine != null) rawValue += foldedLine.Substring(1);
                    }
                    else
                        break;
                } while (true);

                if (encoding == VCardEncoding.QuotedPrintable && rawValue.Length > 0)
                {
                    while (rawValue[rawValue.Length - 1] == '=')
                        rawValue += "\r\n" + reader.ReadLine();
                }

                // The full value has finally been loaded from the
                // input stream.  The next step is to decode it.

                switch (encoding)
                {
                    case VCardEncoding.Base64:
                        property.Value = DecodeBase64(rawValue);
                        break;

                    case VCardEncoding.Escaped:
                        property.Value = DecodeEscaped(rawValue);
                        break;

                    case VCardEncoding.QuotedPrintable:
                        property.Value = DecodeQuotedPrintable(rawValue, charsetEncoding);
                        break;

                    default:
                        property.Value = DecodeEscaped(rawValue);
                        break;
                }

                return property;
            } while (true);
        }

        private static Encoding GetCharsetEncoding(string encodingName)
        {
            switch (encodingName)
            {
                case "UTF-8":
                    return Encoding.UTF8;
                case "ASCII":
                    return Encoding.ASCII;
                default:
                    return Encoding.GetEncoding(encodingName);
            }
        }
        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using OfficeConverter.Exceptions;
using VCardReader.Helpers;
using VCardReader.Localization;

namespace VCardReader
{
    /// <summary>
    ///     Will read an CVF file and returns it as an HTML document
    /// </summary>
    public class Reader
    {
        #region Fields
        /// <summary>
        ///     Used to keep track if we already did write an empty line
        /// </summary>
        private static bool _emptyLineWritten;

        /// <summary>
        ///     Contains an error message when something goes wrong in the <see cref="ExtractToFolderFromCom" /> method.
        ///     This message can be retreived with the GetErrorMessage. This way we keep .NET exceptions inside
        ///     when this code is called from a COM language
        /// </summary>
        private string _errorMessage;
        #endregion

        #region SetCulture
        /// <summary>
        ///     Sets the culture that needs to be used to localize the output of this class.
        ///     Default the current system culture is set. When there is no localization available the
        ///     default will be used. This will be en-US.
        /// </summary>
        /// <param name="name">The name of the cultere eg. nl-NL</param>
        public void SetCulture(string name)
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(name);
        }
        #endregion

        #region CheckFileNameAndOutputFolder
        /// <summary>
        ///     Checks if the <paramref name="inputFile" /> and <paramref name="outputFolder" /> is valid
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="outputFolder"></param>
        /// <exception cref="ArgumentNullException">Raised when the <paramref name="inputFile" /> or
        ///     <paramref name="outputFolder" /> is null or empty</exception>
        /// <exception cref="FileNotFoundException">Raised when the <paramref name="inputFile" /> does not exists</exception>
        /// <exception cref="DirectoryNotFoundException">Raised when the <paramref name="outputFolder" /> does not exist</exception>
        /// <exception cref="VCRFileTypeNotSupported">Raised when the extension is not .vcf </exception>
        private static string CheckFileNameAndOutputFolder(string inputFile, string outputFolder)
        {
            if (string.IsNullOrEmpty(inputFile))
                throw new ArgumentNullException(inputFile);

            if (string.IsNullOrEmpty(outputFolder))
                throw new ArgumentNullException(outputFolder);

            if (!File.Exists(inputFile))
                throw new FileNotFoundException(inputFile);

            if (!Directory.Exists(outputFolder))
                throw new DirectoryNotFoundException(outputFolder);

            var extension = Path.GetExtension(inputFile);
            if (string.IsNullOrEmpty(extension))
                throw new VCRFileTypeNotSupported("Expected .vcf extension on the inputfile");

            extension = extension.ToUpperInvariant();

            switch (extension)
            {
                case ".VCF":
                    return extension;

                default:
                    throw new VCRFileTypeNotSupported("Wrong file extension, expected .vcf");
            }
        }
        #endregion

        #region ExtractToFolder
        /// <summary>
        ///     This method reads the <paramref name="inputFile" /> and when the file is supported it will do the following: <br />
        ///     - Extract the HTML, RTF (will be converted to html) or TEXT body (in these order) <br />
        ///     - Puts a header (with the sender, to, cc, etc... (depends on the message type) on top of the body so it looks
        ///     like if the object is printed from Outlook <br />
        ///     - Reads all the attachents <br />
        ///     And in the end writes everything to the given <paramref name="outputFolder" />
        /// </summary>
        /// <param name="inputFile">The msg file</param>
        /// <param name="outputFolder">The folder where to save the extracted msg file</param>
        /// <param name="hyperlinks">When true hyperlinks are generated for the To, CC, BCC and attachments</param>
        /// <param name="culture"></param>
        public string[] ExtractToFolderFromCom(string inputFile,
            string outputFolder,
            bool hyperlinks = false,
            string culture = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(culture))
                    SetCulture(culture);

                return ExtractToFolder(inputFile, outputFolder, hyperlinks);
            }
            catch (Exception e)
            {
                _errorMessage = ExceptionHelpers.GetInnerException(e);
                return new string[0];
            }
        }

        /// <summary>
        ///     This method reads the <paramref name="inputFile" /> and when the file is supported it will do the following: <br />
        ///     - Extract the HTML, RTF (will be converted to html) or TEXT body (in these order) <br />
        ///     - Puts a header (with the sender, to, cc, etc... (depends on the message type) on top of the body so it looks
        ///     like if the object is printed from Outlook <br />
        ///     - Reads all the attachents <br />
        ///     And in the end writes everything to the given <paramref name="outputFolder" />
        /// </summary>
        /// <param name="inputFile">The msg file</param>
        /// <param name="outputFolder">The folder where to save the extracted msg file</param>
        /// <param name="hyperlinks">When true hyperlinks are generated for the To, CC, BCC and attachments</param>
        /// <returns>String array containing the full path to the message body and its attachments</returns>
        /// <exception cref="MRFileTypeNotSupported">Raised when the Microsoft Outlook message type is not supported</exception>
        /// <exception cref="MRInvalidSignedFile">Raised when the Microsoft Outlook signed message is invalid</exception>
        /// <exception cref="ArgumentNullException">Raised when the
        ///     <param ref="inputFile" />
        ///     or
        ///     <param ref="outputFolder" />
        ///     is null or empty</exception>
        /// <exception cref="FileNotFoundException">Raised when the
        ///     <param ref="inputFile" />
        ///     does not exists</exception>
        /// <exception cref="DirectoryNotFoundException">Raised when the
        ///     <param ref="outputFolder" />
        ///     does not exists</exception>
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public string[] ExtractToFolder(string inputFile, string outputFolder, bool hyperlinks = false)
        {
            outputFolder = FileManager.CheckForBackSlash(outputFolder);

            _errorMessage = string.Empty;

            var extension = CheckFileNameAndOutputFolder(inputFile, outputFolder);

            switch (extension)
            {
                case ".EML":
                    using (var stream = File.Open(inputFile, FileMode.Open, FileAccess.Read))
                    {
                        var message = Mime.Message.Load(stream);
                        return WriteEmlEmail(message, outputFolder, hyperlinks).ToArray();
                    }
            }

            return new string[0];
        }
        #endregion

        #region WriteHeader methods
        /// <summary>
        ///     Writes the start of the header
        /// </summary>
        /// <param name="header">The <see cref="StringBuilder" /> object that is used to write a header</param>
        private static void WriteHeaderStart(StringBuilder header)
        {
            header.AppendLine("<table style=\"font-family: Times New Roman; font-size: 12pt;\">");
            _emptyLineWritten = false;
        }

        /// <summary>
        ///     Writes a line into the header
        /// </summary>
        /// <param name="header">The <see cref="StringBuilder" /> object that is used to write a header</param>
        /// <param name="label">The label text that needs to be written</param>
        /// <param name="text">The text that needs to be written after the <paramref name="label" /></param>
        private static void WriteHeaderLine(StringBuilder header, 
                                            string label,
                                            string text)
        {
            var lines = text.Split('\n');
            var newText = string.Empty;

            foreach (var line in lines)
                newText += HttpUtility.HtmlEncode(line) + "<br/>";

            header.AppendLine(
                "<tr style=\"height: 18px; vertical-align: top; \"><td style=\"font-weight: bold; white-space:nowrap;\">" +
                HttpUtility.HtmlEncode(label) + ":</td><td>" + newText + "</td></tr>");

            _emptyLineWritten = false;
        }

        /// <summary>
        ///     Writes a line into the header without Html encoding the <paramref name="text" />
        /// </summary>
        /// <param name="header">The <see cref="StringBuilder" /> object that is used to write a header</param>
        /// <param name="label">The label text that needs to be written</param>
        /// <param name="text">The text that needs to be written after the <paramref name="label" /></param>
        private static void WriteHeaderLineNoEncoding(StringBuilder header,
                                                      string label,
                                                      string text)
        {
            text = text.Replace("\n", "<br/>");

            header.AppendLine(
                "<tr style=\"height: 18px; vertical-align: top; \"><td style=\"font-weight: bold; white-space:nowrap; \">" +
                HttpUtility.HtmlEncode(label) + ":</td><td>" + text + "</td></tr>");

            _emptyLineWritten = false;
        }

        /// <summary>
        ///     Writes
        /// </summary>
        /// <param name="header"></param>
        private static void WriteHeaderEmptyLine(StringBuilder header)
        {
            // Prevent that we write 2 empty lines in a row
            if (_emptyLineWritten)
                return;

            header.AppendLine("<tr style=\"height: 18px; vertical-align: top; \"><td>&nbsp;</td><td>&nbsp;</td></tr>");
            _emptyLineWritten = true;
        }

        /// <summary>
        ///     Writes the end of the header
        /// </summary>
        /// <param name="header">The <see cref="StringBuilder" /> object that is used to write a header</param>
        private static void WriteHeaderEnd(StringBuilder header)
        {
            header.AppendLine("</table><br/>");
        }
        #endregion

        #region WriteVCard
        /// <summary>
        ///     Writes the body of the MSG Contact to html or text and extracts all the attachments. The
        ///     result is return as a List of strings
        /// </summary>
        /// <param name="vcard">
        ///     <see cref="VCard" />
        /// </param>
        /// <param name="outputFolder">The folder where we need to write the output</param>
        /// <param name="hyperlinks">When true then hyperlinks are generated for the To, CC, BCC and attachments</param>
        /// <returns></returns>
        private List<string> WriteVCard(VCard vcard, string outputFolder, bool hyperlinks)
        {
            var fileName = "contact";
            string body;
            string contactPhotoFileName;
            List<string> attachmentList;
            List<string> files;

            var contactHeader = new StringBuilder();

            // Start of table
            WriteHeaderStart(contactHeader);

            // TODO: Foto verwerking uitprogrammeren
            //if (!string.IsNullOrEmpty(vcard.Photos[0]))
            //    contactHeader.Append(
            //        "<div style=\"height: 250px; position: absolute; top: 20px; right: 20px;\"><img alt=\"\" src=\"" +
            //        contactPhotoFileName + "\" height=\"100%\"></div>");

            // Full name
            if (!string.IsNullOrEmpty(vcard.DisplayName))
                WriteHeaderLine(contactHeader, LanguageConsts.DisplayNameLabel, vcard.DisplayName);

            // Last name
            if (!string.IsNullOrEmpty(vcard.FamilyName))
                WriteHeaderLine(contactHeader, LanguageConsts.FamilyName, vcard.FamilyName);

            // First name
            if (!string.IsNullOrEmpty(vcard.GivenName))
                WriteHeaderLine(contactHeader, LanguageConsts.GivenNameLabel,
                    vcard.GivenName);

            // Job title
            if (!string.IsNullOrEmpty(vcard.Title))
                WriteHeaderLine(contactHeader, LanguageConsts.TitleLabel,
                    vcard.Title);

            // Department
            if (!string.IsNullOrEmpty(vcard.Department))
                WriteHeaderLine(contactHeader, LanguageConsts.DepartmentLabel,
                    vcard.Department);

            // Company
            if (!string.IsNullOrEmpty(vcard.Organization))
                WriteHeaderLine(contactHeader, LanguageConsts.OrganizationLabel, vcard.Organization);

            // Empty line
            WriteHeaderEmptyLine(contactHeader);

            // Business address
            foreach (var deliveryLabel in vcard.DeliveryLabels)
            {
                switch (deliveryLabel.AddressType)
                {
                    case DeliveryAddressTypes.Domestic:
                        //WriteHeaderLine(contactHeader, LanguageConsts.HomeDeliveryAddressLabel, deliveryLabel.Text);
                        break;

                    case DeliveryAddressTypes.Home:
                        WriteHeaderLine(contactHeader, LanguageConsts.HomeDeliveryAddressLabel, deliveryLabel.Text);
                        break;

                    case DeliveryAddressTypes.International:
                        WriteHeaderLine(contactHeader, LanguageConsts.HomeDeliveryAddressLabel, deliveryLabel.Text);
                        break;

                    case DeliveryAddressTypes.Parcel:
                        WriteHeaderLine(contactHeader, LanguageConsts.HomeDeliveryAddressLabel, deliveryLabel.Text);
                        break;

                    case DeliveryAddressTypes.Postal:
                        WriteHeaderLine(contactHeader, LanguageConsts.HomeDeliveryAddressLabel, deliveryLabel.Text);
                        break;

                    case DeliveryAddressTypes.Work:
                        WriteHeaderLine(contactHeader, LanguageConsts.HomeDeliveryAddressLabel, deliveryLabel.Text);
                        break;

                    case DeliveryAddressTypes.Default:
                        WriteHeaderLine(contactHeader, LanguageConsts.HomeDeliveryAddressLabel, deliveryLabel.Text);
                        break;
                }
            }

            //// Empty line
            WriteHeaderEmptyLine(contactHeader);

            foreach (var phone in vcard.Phones)
            {
                switch (phone.PhoneType)
                {
                    case PhoneTypes.Bbs:
                        //WriteHeaderLine(contactHeader, LanguageConsts.HomeDeliveryAddressLabel, deliveryLabel.Text);
                        break;

                    case PhoneTypes.Car:
                        WriteHeaderLine(contactHeader, LanguageConsts.HomeDeliveryAddressLabel, phone.FullNumber);
                        break;

                    case PhoneTypes.Cellular:
                        WriteHeaderLine(contactHeader, LanguageConsts.HomeDeliveryAddressLabel, phone.FullNumber);
                        break;

                    case PhoneTypes.CellularVoice:
                        WriteHeaderLine(contactHeader, LanguageConsts.HomeDeliveryAddressLabel, phone.FullNumber);
                        break;

                    case PhoneTypes.Fax:
                        WriteHeaderLine(contactHeader, LanguageConsts.HomeDeliveryAddressLabel, phone.FullNumber);
                        break;

                    case PhoneTypes.Home:
                        WriteHeaderLine(contactHeader, LanguageConsts.HomeDeliveryAddressLabel, phone.FullNumber);
                        break;

                    case PhoneTypes.Isdn:
                        WriteHeaderLine(contactHeader, LanguageConsts.HomeDeliveryAddressLabel, phone.FullNumber);
                        break;

                    case PhoneTypes.MessagingService:
                        WriteHeaderLine(contactHeader, LanguageConsts.HomeDeliveryAddressLabel, phone.FullNumber);
                        break;

                    case PhoneTypes.Modem:
                        WriteHeaderLine(contactHeader, LanguageConsts.HomeDeliveryAddressLabel, phone.FullNumber);
                        break;

                    case PhoneTypes.Preferred:
                        WriteHeaderLine(contactHeader, LanguageConsts.HomeDeliveryAddressLabel, phone.FullNumber);
                        break;

                    case PhoneTypes.Video:
                        WriteHeaderLine(contactHeader, LanguageConsts.HomeDeliveryAddressLabel, phone.FullNumber);
                        break;

                    case PhoneTypes.Voice:
                        WriteHeaderLine(contactHeader, LanguageConsts.HomeDeliveryAddressLabel, phone.FullNumber);
                        break;

                    case PhoneTypes.Work:
                        WriteHeaderLine(contactHeader, LanguageConsts.HomeDeliveryAddressLabel, phone.FullNumber);
                        break;

                    case PhoneTypes.WorkFax:
                        WriteHeaderLine(contactHeader, LanguageConsts.HomeDeliveryAddressLabel, phone.FullNumber);
                        break;
                }
            }

            //// Empty line
            WriteHeaderEmptyLine(contactHeader);

            foreach (var email in vcard.EmailAddresses)
            {
                switch (email.EmailType)
                {
                    case EmailAddressType.AOl:
                        break;

                    case EmailAddressType.AppleLink:
                        break;

                    case EmailAddressType.AttMail:
                        break;

                    case EmailAddressType.CompuServe:
                        break;

                    case EmailAddressType.EWorld:
                        break;

                    case EmailAddressType.IBMMail:
                        break;

                    case EmailAddressType.Internet:
                        break;

                    case EmailAddressType.MCIMail:
                        break;

                    case EmailAddressType.PowerShare:
                        break;

                    case EmailAddressType.Prodigy:
                        break;

                    case EmailAddressType.Telex:
                        break;
                }
            }

            //// E-mail
            //if (!string.IsNullOrEmpty(vcard.Email1EmailAddress))
            //    WriteHeaderLine(contactHeader, LanguageConsts.Email1EmailAddressLabel,
            //        vcard.Email1EmailAddress);

            //// E-mail display as
            //if (!string.IsNullOrEmpty(vcard.Email1DisplayName))
            //    WriteHeaderLine(contactHeader, LanguageConsts.Email1DisplayNameLabel,
            //        vcard.Email1DisplayName);

            //// E-mail 2
            //if (!string.IsNullOrEmpty(vcard.Email2EmailAddress))
            //    WriteHeaderLine(contactHeader, LanguageConsts.Email2EmailAddressLabel,
            //        vcard.Email2EmailAddress);

            //// E-mail display as 2
            //if (!string.IsNullOrEmpty(vcard.Email2DisplayName))
            //    WriteHeaderLine(contactHeader, LanguageConsts.Email2DisplayNameLabel,
            //        vcard.Email2DisplayName);

            //// E-mail 3
            //if (!string.IsNullOrEmpty(vcard.Email3EmailAddress))
            //    WriteHeaderLine(contactHeader, LanguageConsts.Email3EmailAddressLabel,
            //        vcard.Email3EmailAddress);

            //// E-mail display as 3
            //if (!string.IsNullOrEmpty(vcard.Email3DisplayName))
            //    WriteHeaderLine(contactHeader, LanguageConsts.Email3DisplayNameLabel,
            //        vcard.Email3DisplayName);

            //// Empty line
            //WriteHeaderEmptyLine(contactHeader);

            //// Birthday
            //if (vcard.BirthDate != null)
            //    WriteHeaderLine(contactHeader, LanguageConsts.BirthdayLabel,
            //        ((DateTime) vcard.BirthDate).ToString(LanguageConsts.DataFormat));

            //// Anniversary
            //if (vcard.WeddingAnniversary != null)
            //    WriteHeaderLine(contactHeader, LanguageConsts.WeddingAnniversaryLabel,
            //        ((DateTime) vcard.WeddingAnniversary).ToString(LanguageConsts.DataFormat));

            //// Spouse/Partner
            //if (!string.IsNullOrEmpty(vcard.SpouseName))
            //    WriteHeaderLine(contactHeader, LanguageConsts.SpouseNameLabel,
            //        vcard.SpouseName);

            //// Profession
            //if (!string.IsNullOrEmpty(vcard.Role))
            //    WriteHeaderLine(contactHeader, LanguageConsts.RoleLabel,
            //        vcard.Role);

            //// Assistant
            //if (!string.IsNullOrEmpty(vcard.AssistantName))
            //    WriteHeaderLine(contactHeader, LanguageConsts.AssistantTelephoneNumberLabel,
            //        vcard.AssistantName);

            // Web page
            foreach (var webpage in vcard.Websites)
            {
                if (!string.IsNullOrEmpty(webpage.Url))
                {
                    switch (webpage.WebsiteType)
                    {
                        case WebsiteTypes.Personal:
                            WriteHeaderLine(contactHeader, LanguageConsts.WebpagePersonalLabel, webpage.Url);
                            break;

                        case WebsiteTypes.Work:
                            WriteHeaderLine(contactHeader, LanguageConsts.WebPageWorkLabel, webpage.Url);
                            break;

                        case WebsiteTypes.Default:
                            WriteHeaderLine(contactHeader, LanguageConsts.WebpageDefaultLabel, webpage.Url);
                            break;
                    }
                }
            }

            // Empty line
            WriteHeaderEmptyLine(contactHeader);

            // Categories
            var categories = vcard.Categories;
            if (categories != null)
                WriteHeaderLine(contactHeader, LanguageConsts.CategoriesLabel,
                    String.Join("; ", categories));

            // Empty line
            WriteHeaderEmptyLine(contactHeader);

            WriteHeaderEnd(contactHeader);

            body = InjectHeader(body, contactHeader.ToString());

            // Write the body to a file
            File.WriteAllText(fileName, body, Encoding.UTF8);

            return files;
        }
        #endregion

        #region InjectHeader
        /// <summary>
        /// Inject an outlook style header into the top of the html
        /// </summary>
        /// <param name="body"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        private static string InjectHeader(string body, string header)
        {
            var temp = body.ToUpperInvariant();

            var begin = temp.IndexOf("<BODY", StringComparison.Ordinal);

            if (begin > 0)
            {
                begin = temp.IndexOf(">", begin, StringComparison.Ordinal);
                return body.Insert(begin + 1, header);
            }

            return header + body;
        }
        #endregion
    }
}
using System;
using System.Collections.Generic;
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
        /// <exception cref="ArgumentNullException">Raised when the <paramref name="inputFile" /> or <paramref name="outputFolder" /> is null or empty</exception>
        /// <exception cref="FileNotFoundException">Raised when the <paramref name="inputFile" /> does not exists</exception>
        /// <exception cref="DirectoryNotFoundException">Raised when the <paramref name="outputFolder" /> does not exist</exception>
        /// <exception cref="VCRFileTypeNotSupported">Raised when the extension is not .vcf</exception>
        private static void CheckFileNameAndOutputFolder(string inputFile, string outputFolder)
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
                    return;

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
        /// <param name="inputFile">The vcf file</param>
        /// <param name="outputFolder">The folder where to save the extracted vcf file</param>
        /// <param name="hyperlinks">When true hyperlinks are clickable, otherwhise they are written as plain text</param>
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

                return ExtractToFolder(inputFile, outputFolder);
            }
            catch (Exception exception)
            {
                _errorMessage = ExceptionHelpers.GetInnerException(exception);
                return new string[0];
            }
        }

        /// <summary>
        /// This method will read the given <paramref name="inputFile"/> convert it to HTML and write it to the <paramref name="outputFolder"/>
        /// </summary>
        /// <param name="inputFile">The vcf file</param>
        /// <param name="outputFolder">The folder where to save the converted vcf file</param>
        /// <param name="hyperlinks">When true hyperlinks are clickable, otherwhise they are written as plain text</param>
        /// <returns>String array containing the full path to the converted VCF file</returns>
        /// <exception cref="ArgumentNullException">Raised when the <paramref name="inputFile" /> or <paramref name="outputFolder" /> is null or empty</exception>
        /// <exception cref="FileNotFoundException">Raised when the <paramref name="inputFile" /> does not exists</exception>
        /// <exception cref="DirectoryNotFoundException">Raised when the <paramref name="outputFolder" /> does not exist</exception>
        /// <exception cref="VCRFileTypeNotSupported">Raised when the extension is not .vcf</exception>
        public string[] ExtractToFolder(string inputFile, string outputFolder, bool hyperlinks = false)
        {
            outputFolder = FileManager.CheckForBackSlash(outputFolder);

            _errorMessage = string.Empty;

            CheckFileNameAndOutputFolder(inputFile, outputFolder);

            using (TextReader textReader = File.OpenText(inputFile))
            {
                var vcardReader = new VCardReader();
                var vCard = new VCard();
                vcardReader.ReadInto(vCard, textReader);
                return WriteVCard(vCard, outputFolder, hyperlinks).ToArray();
            }
        }
        #endregion

        #region WriteTable methods
        /// <summary>
        ///     Writes the start of the table
        /// </summary>
        /// <param name="table">The <see cref="StringBuilder" /> object that is used to write a table</param>
        private static void WriteTableStart(StringBuilder table)
        {
            table.AppendLine("<table style=\"font-family: Times New Roman; font-size: 12pt;\">");
            _emptyLineWritten = false;
        }

        /// <summary>
        ///     Writes a row tot the table
        /// </summary>
        /// <param name="table">The <see cref="StringBuilder" /> object that is used to write a table</param>
        /// <param name="label">The label text that needs to be written</param>
        /// <param name="text">The text that needs to be written after the <paramref name="label" /></param>
        private static void WriteTableRow(StringBuilder table, 
                                          string label,
                                          string text)
        {
            var lines = text.Split('\n');
            var newText = string.Empty;

            foreach (var line in lines)
                newText += HttpUtility.HtmlEncode(line) + "<br/>";

            table.AppendLine(
                "<tr style=\"height: 18px; vertical-align: top; \"><td style=\"font-weight: bold; white-space:nowrap;\">" +
                HttpUtility.HtmlEncode(label) + ":</td><td>" + newText + "</td></tr>");

            _emptyLineWritten = false;
        }

        /// <summary>
        ///     Writes a tow tot the table without Html encoding the <paramref name="text" />
        /// </summary>
        /// <param name="header">The <see cref="StringBuilder" /> object that is used to write a table</param>
        /// <param name="label">The label text that needs to be written</param>
        /// <param name="text">The text that needs to be written after the <paramref name="label" /></param>
        private static void WriteTableRowNoEncoding(StringBuilder header,
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
        /// <param name="table"></param>
        private static void WriteEmptyTableRow(StringBuilder table)
        {
            // Prevent that we write 2 empty lines in a row
            if (_emptyLineWritten)
                return;

            table.AppendLine("<tr style=\"height: 18px; vertical-align: top; \"><td>&nbsp;</td><td>&nbsp;</td></tr>");
            _emptyLineWritten = true;
        }

        /// <summary>
        ///     Writes the end of the table
        /// </summary>
        /// <param name="table">The <see cref="StringBuilder" /> object that is used to write a table</param>
        private static void WriteTableEnd(StringBuilder table)
        {
            table.AppendLine("</table><br/>");
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
            var fileName = Path.Combine(outputFolder, "contact.html");
            string contactPhotoFileName;
            var files = new List<string> {fileName};
            
            var output = new StringBuilder();

            // Start of table
            WriteTableStart(output);

            //foreach (var photo in vcard.Photos)
            //{
            //    if (photo.Url)
            //        contactHeader.Append(
            //            "<div style=\"height: 250px; position: absolute; top: 20px; right: 20px;\"><img alt=\"\" src=\"" +
            //            contactPhotoFileName + "\" height=\"100%\"></div>");
            //}

            // Full name
            if (!string.IsNullOrEmpty(vcard.FormattedName))
                WriteTableRow(output, LanguageConsts.DisplayNameLabel, vcard.FormattedName);

            // Last name
            if (!string.IsNullOrEmpty(vcard.FamilyName))
                WriteTableRow(output, LanguageConsts.SurNameLabel, vcard.FamilyName);

            // First name
            if (!string.IsNullOrEmpty(vcard.GivenName))
                WriteTableRow(output, LanguageConsts.GivenNameLabel,
                    vcard.GivenName);

            // Job title
            if (!string.IsNullOrEmpty(vcard.Title))
                WriteTableRow(output, LanguageConsts.FunctionLabel,
                    vcard.Title);

            // Department
            if (!string.IsNullOrEmpty(vcard.Department))
                WriteTableRow(output, LanguageConsts.DepartmentLabel,
                    vcard.Department);

            // Company
            if (!string.IsNullOrEmpty(vcard.Organization))
                WriteTableRow(output, LanguageConsts.CompanyLabel, vcard.Organization);

            // Empty line
            WriteEmptyTableRow(output);
            
            // Business address
            foreach (var deliveryLabel in vcard.DeliveryLabels)
            {
                switch (deliveryLabel.AddressType)
                {
                    case DeliveryAddressTypes.Domestic:
                        WriteTableRow(output, LanguageConsts.DomesticAddressLabel, deliveryLabel.Text);
                        break;

                    case DeliveryAddressTypes.Home:
                        WriteTableRow(output, LanguageConsts.HomeAddressLabel, deliveryLabel.Text);
                        break;

                    case DeliveryAddressTypes.International:
                        WriteTableRow(output, LanguageConsts.InternationalAddressLabel, deliveryLabel.Text);
                        break;

                    case DeliveryAddressTypes.Parcel:
                        WriteTableRow(output, LanguageConsts.PostalAddressLabel, deliveryLabel.Text);
                        break;

                    case DeliveryAddressTypes.Postal:
                        WriteTableRow(output, LanguageConsts.PostalAddressLabel, deliveryLabel.Text);
                        break;

                    case DeliveryAddressTypes.Work:
                        WriteTableRow(output, LanguageConsts.WorkAddressLabel, deliveryLabel.Text);
                        break;

                    case DeliveryAddressTypes.Default:
                        WriteTableRow(output, LanguageConsts.OtherAddressLabel, deliveryLabel.Text);
                        break;
                }
            }

            // Instant messaging
            if (!string.IsNullOrEmpty(vcard.InstantMessagingAddress))
                WriteTableRow(output, LanguageConsts.InstantMessagingAddressLabel, vcard.InstantMessagingAddress);
            
            //// Empty line
            WriteEmptyTableRow(output);
            
            foreach (var phone in vcard.Phones)
            {
                switch (phone.PhoneType)
                {
                    case PhoneTypes.Bbs:
                        WriteTableRow(output, LanguageConsts.BBSTelephoneNumberLabel, phone.FullNumber);
                        break;

                    case PhoneTypes.Car:
                        WriteTableRow(output, LanguageConsts.CarTelephoneNumberLabel, phone.FullNumber);
                        break;

                    case PhoneTypes.Cellular:
                        WriteTableRow(output, LanguageConsts.CellularTelephoneNumberLabel, phone.FullNumber);
                        break;

                    case PhoneTypes.CellularVoice:
                        WriteTableRow(output, LanguageConsts.CellularTelephoneNumberLabel, phone.FullNumber);
                        break;

                    case PhoneTypes.Fax:
                        WriteTableRow(output, LanguageConsts.OtherFaxLabel, phone.FullNumber);
                        break;

                    case PhoneTypes.Home:
                    case PhoneTypes.HomeVoice:
                        WriteTableRow(output, LanguageConsts.HomeTelephoneNumberLabel, phone.FullNumber);
                        break;

                    case PhoneTypes.Isdn:
                        WriteTableRow(output, LanguageConsts.ISDNNumberLabel, phone.FullNumber);
                        break;

                    case PhoneTypes.MessagingService:
                        WriteTableRow(output, LanguageConsts.BeeperTelephoneNumberLabel, phone.FullNumber);
                        break;

                    case PhoneTypes.Modem:
                        WriteTableRow(output, LanguageConsts.ModemTelephoneNumberLabel, phone.FullNumber);
                        break;

                    case PhoneTypes.Pager:
                    case PhoneTypes.Ttytdd:
                        WriteTableRow(output, LanguageConsts.TextTelephoneLabel, phone.FullNumber);
                        break;

                    case PhoneTypes.Preferred:
                        WriteTableRow(output, LanguageConsts.PrimaryTelephoneNumberLabel, phone.FullNumber);
                        break;

                    case PhoneTypes.Video:
                        WriteTableRow(output, LanguageConsts.VideoTelephoneNumberLabel, phone.FullNumber);
                        break;

                    case PhoneTypes.Voice:
                        WriteTableRow(output, LanguageConsts.VoiceTelephoneNumberLabel, phone.FullNumber);
                        break;

                    case PhoneTypes.Work:
                    case PhoneTypes.WorkVoice:
                        WriteTableRow(output, LanguageConsts.BusinessTelephoneNumberLabel, phone.FullNumber);
                        break;

                    case PhoneTypes.VoiceCompany:
                        WriteTableRow(output, LanguageConsts.CompanyMainTelephoneNumberLabel, phone.FullNumber);
                        break;

                    case PhoneTypes.VoiceCallback:
                        WriteTableRow(output, LanguageConsts.CallbackTelephoneNumberLabel, phone.FullNumber);
                        break;

                    case PhoneTypes.VoiceRadio:
                        WriteTableRow(output, LanguageConsts.RadioTelephoneNumberLabel, phone.FullNumber);
                        break;

                    case PhoneTypes.VoiceAssistant:
                        WriteTableRow(output, LanguageConsts.AssistantLabel, phone.FullNumber);
                        break;

                    case PhoneTypes.WorkFax:
                        WriteTableRow(output, LanguageConsts.BusinessFaxNumberLabel, phone.FullNumber);
                        break;
                }
            }

            //// Empty line
            WriteEmptyTableRow(output);

            var i = 1;

            foreach (var email in vcard.EmailAddresses)
            {
                switch (email.EmailType)
                {
                    case EmailAddressType.AOl:
                    case EmailAddressType.AppleLink:
                    case EmailAddressType.AttMail:
                    case EmailAddressType.CompuServe:
                    case EmailAddressType.EWorld:
                    case EmailAddressType.IBMMail:
                    case EmailAddressType.Internet:
                    case EmailAddressType.MCIMail:
                    case EmailAddressType.PowerShare:
                    case EmailAddressType.Prodigy:
                        WriteTableRow(output, LanguageConsts.EmailEmailAddressLabel + " " + i, email.ToString());
                        i += 1;
                        break;

                    case EmailAddressType.Telex:
                        WriteTableRow(output, LanguageConsts.TelexNumberLabel, email.ToString());
                        break;

                }
            }

            // Empty line
            WriteEmptyTableRow(output);
            /*
            // Birthday
            if (vcard.BirthDate != null)
                WriteTableRow(output, LanguageConsts.BirthdayLabel,
                    ((DateTime) vcard.BirthDate).ToString(LanguageConsts.DataFormat));

            // Anniversary
            if (vcard.Anniversary != null)
                WriteTableRow(output, LanguageConsts.WeddingAnniversaryLabel,
                    ((DateTime) vcard.Anniversary).ToString(LanguageConsts.DataFormat));

            // Spouse/Partner
            if (!string.IsNullOrEmpty(vcard.SpouseName))
                WriteTableRow(output, LanguageConsts.SpouseNameLabel,
                    vcard.SpouseName);

            // Profession
            if (!string.IsNullOrEmpty(vcard.Role))
                WriteTableRow(output, LanguageConsts.RoleLabel,
                    vcard.Role);

            // Assistant
            if (!string.IsNullOrEmpty(vcard.AssistantName))
                WriteTableRow(output, LanguageConsts.AssistantTelephoneNumberLabel,
                    vcard.AssistantName);

            /*
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
            */
            // Categories
            var categories = vcard.Categories;
            if (categories != null && categories.Count > 0)
                WriteTableRow(output, LanguageConsts.CategoriesLabel,
                    String.Join("; ", categories));

            // Empty line
            WriteEmptyTableRow(output);
            
            WriteTableEnd(output);

            // Write the body to a file
            File.WriteAllText(fileName, output.ToString(), Encoding.UTF8);

            return files;
        }
        #endregion
    }
}
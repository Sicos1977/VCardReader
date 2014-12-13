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
    /// Will read an CVF file and returns it as an HTML document
    /// </summary>
    public class Reader
    {
        #region Fields
        /// <summary>
        /// Contains an error message when something goes wrong in the <see cref="ExtractToFolderFromCom"/> method.
        /// This message can be retreived with the GetErrorMessage. This way we keep .NET exceptions inside
        /// when this code is called from a COM language
        /// </summary>
        private string _errorMessage;

        /// <summary>
        /// Used to keep track if we already did write an empty line
        /// </summary>
        private static bool _emptyLineWritten;
        #endregion

        #region SetCulture
        /// <summary>
        /// Sets the culture that needs to be used to localize the output of this class. 
        /// Default the current system culture is set. When there is no localization available the
        /// default will be used. This will be en-US.
        /// </summary>
        /// <param name="name">The name of the cultere eg. nl-NL</param>
        public void SetCulture(string name)
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(name);
        }
        #endregion

        #region CheckFileNameAndOutputFolder
        /// <summary>
        /// Checks if the <paramref name="inputFile"/> and <paramref name="outputFolder"/> is valid
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="outputFolder"></param>
        /// <exception cref="ArgumentNullException">Raised when the <paramref name="inputFile"/> or <paramref name="outputFolder"/> is null or empty</exception>
        /// <exception cref="FileNotFoundException">Raised when the <paramref name="inputFile"/> does not exists</exception>
        /// <exception cref="DirectoryNotFoundException">Raised when the <paramref name="outputFolder"/> does not exist</exception>
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
        /// This method reads the <paramref name="inputFile"/> and when the file is supported it will do the following: <br/>
        /// - Extract the HTML, RTF (will be converted to html) or TEXT body (in these order) <br/>
        /// - Puts a header (with the sender, to, cc, etc... (depends on the message type) on top of the body so it looks 
        ///   like if the object is printed from Outlook <br/>
        /// - Reads all the attachents <br/>
        /// And in the end writes everything to the given <paramref name="outputFolder"/>
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
        /// This method reads the <paramref name="inputFile"/> and when the file is supported it will do the following: <br/>
        /// - Extract the HTML, RTF (will be converted to html) or TEXT body (in these order) <br/>
        /// - Puts a header (with the sender, to, cc, etc... (depends on the message type) on top of the body so it looks 
        ///   like if the object is printed from Outlook <br/>
        /// - Reads all the attachents <br/>
        /// And in the end writes everything to the given <paramref name="outputFolder"/>
        /// </summary>
        /// <param name="inputFile">The msg file</param>
        /// <param name="outputFolder">The folder where to save the extracted msg file</param>
        /// <param name="hyperlinks">When true hyperlinks are generated for the To, CC, BCC and attachments</param>
        /// <returns>String array containing the full path to the message body and its attachments</returns>
        /// <exception cref="MRFileTypeNotSupported">Raised when the Microsoft Outlook message type is not supported</exception>
        /// <exception cref="MRInvalidSignedFile">Raised when the Microsoft Outlook signed message is invalid</exception>
        /// <exception cref="ArgumentNullException">Raised when the <param ref="inputFile"/> or <param ref="outputFolder"/> is null or empty</exception>
        /// <exception cref="FileNotFoundException">Raised when the <param ref="inputFile"/> does not exists</exception>
        /// <exception cref="DirectoryNotFoundException">Raised when the <param ref="outputFolder"/> does not exists</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
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
        /// Writes the start of the header
        /// </summary>
        /// <param name="header">The <see cref="StringBuilder"/> object that is used to write a header</param>
        /// <param name="htmlBody">When true then html will be written into the <param ref="header"/> otherwise text will be written</param>
        private static void WriteHeaderStart(StringBuilder header, bool htmlBody)
        {
            if (!htmlBody)
                return;

            header.AppendLine("<table style=\"font-family: Times New Roman; font-size: 12pt;\">");

            _emptyLineWritten = false;
        }

        /// <summary>
        /// Writes a line into the header
        /// </summary>
        /// <param name="header">The <see cref="StringBuilder"/> object that is used to write a header</param>
        /// <param name="htmlBody">When true then html will be written into the <paramref name="header"/> otherwise text will be written</param>
        /// <param name="labelPadRightWidth">Used to pad the label size, ignored when <paramref name="htmlBody"/> is true</param>
        /// <param name="label">The label text that needs to be written</param>
        /// <param name="text">The text that needs to be written after the <paramref name="label"/></param>
        private static void WriteHeaderLine(StringBuilder header,
            bool htmlBody,
            int labelPadRightWidth,
            string label,
            string text)
        {
            if (htmlBody)
            {
                var lines = text.Split('\n');
                var newText = string.Empty;

                foreach (var line in lines)
                    newText += HttpUtility.HtmlEncode(line) + "<br/>";

                header.AppendLine(
                    "<tr style=\"height: 18px; vertical-align: top; \"><td style=\"font-weight: bold; white-space:nowrap;\">" +
                    HttpUtility.HtmlEncode(label) + ":</td><td>" + newText + "</td></tr>");
            }
            else
            {
                text = text.Replace("\n", "".PadRight(labelPadRightWidth));
                header.AppendLine((label + ":").PadRight(labelPadRightWidth) + text);
            }

            _emptyLineWritten = false;
        }

        /// <summary>
        /// Writes a line into the header without Html encoding the <paramref name="text"/>
        /// </summary>
        /// <param name="header">The <see cref="StringBuilder"/> object that is used to write a header</param>
        /// <param name="htmlBody">When true then html will be written into the <paramref name="header"/> otherwise text will be written</param>
        /// <param name="labelPadRightWidth">Used to pad the label size, ignored when <paramref name="htmlBody"/> is true</param>
        /// <param name="label">The label text that needs to be written</param>
        /// <param name="text">The text that needs to be written after the <paramref name="label"/></param>
        private static void WriteHeaderLineNoEncoding(StringBuilder header,
            bool htmlBody,
            int labelPadRightWidth,
            string label,
            string text)
        {
            if (htmlBody)
            {
                text = text.Replace("\n", "<br/>");

                header.AppendLine(
                    "<tr style=\"height: 18px; vertical-align: top; \"><td style=\"font-weight: bold; white-space:nowrap; \">" +
                    HttpUtility.HtmlEncode(label) + ":</td><td>" + text + "</td></tr>");
            }
            else
            {
                text = text.Replace("\n", "".PadRight(labelPadRightWidth));
                header.AppendLine((label + ":").PadRight(labelPadRightWidth) + text);
            }

            _emptyLineWritten = false;
        }

        /// <summary>
        /// Writes
        /// </summary>
        /// <param name="header"></param>
        /// <param name="htmlBody"></param>
        private static void WriteHeaderEmptyLine(StringBuilder header, bool htmlBody)
        {
            // Prevent that we write 2 empty lines in a row
            if (_emptyLineWritten)
                return;

            header.AppendLine(
                htmlBody
                    ? "<tr style=\"height: 18px; vertical-align: top; \"><td>&nbsp;</td><td>&nbsp;</td></tr>"
                    : string.Empty);

            _emptyLineWritten = true;
        }

        /// <summary>
        /// Writes the end of the header
        /// </summary>
        /// <param name="header">The <see cref="StringBuilder"/> object that is used to write a header</param>
        /// <param name="htmlBody">When true then html will be written into the <param ref="header"/> otherwise text will be written</param>
        private static void WriteHeaderEnd(StringBuilder header, bool htmlBody)
        {
            header.AppendLine(!htmlBody ? string.Empty : "</table><br/>");
        }
        #endregion

        #region WriteMsgContact
        /// <summary>
        /// Writes the body of the MSG Contact to html or text and extracts all the attachments. The
        /// result is return as a List of strings
        /// </summary>
        /// <param name="message"><see cref="Storage.Message"/></param>
        /// <param name="outputFolder">The folder where we need to write the output</param>
        /// <param name="hyperlinks">When true then hyperlinks are generated for the To, CC, BCC and attachments</param>
        /// <returns></returns>
        private List<string> WriteMsgContact(Storage.Message message, string outputFolder, bool hyperlinks)
        {
            var fileName = "contact";
            string body;
            string contactPhotoFileName;
            List<string> attachmentList;
            List<string> files;
            
            var contactHeader = new StringBuilder();

            // Start of table
            WriteHeaderStart(contactHeader, htmlBody);

            if (htmlBody && !string.IsNullOrEmpty(contactPhotoFileName))
                contactHeader.Append(
                    "<div style=\"height: 250px; position: absolute; top: 20px; right: 20px;\"><img alt=\"\" src=\"" +
                    contactPhotoFileName + "\" height=\"100%\"></div>");

            // Full name
            if (!string.IsNullOrEmpty(message.Contact.DisplayName))
                WriteHeaderLine(contactHeader, htmlBody, maxLength, LanguageConsts.DisplayNameLabel,
                    message.Contact.DisplayName);

            // Last name
            if (!string.IsNullOrEmpty(message.Contact.SurName))
                WriteHeaderLine(contactHeader, htmlBody, maxLength, LanguageConsts.SurNameLabel, message.Contact.SurName);

            // First name
            if (!string.IsNullOrEmpty(message.Contact.GivenName))
                WriteHeaderLine(contactHeader, htmlBody, maxLength, LanguageConsts.GivenNameLabel, message.Contact.GivenName);

            // Job title
            if (!string.IsNullOrEmpty(message.Contact.Function))
                WriteHeaderLine(contactHeader, htmlBody, maxLength, LanguageConsts.FunctionLabel, message.Contact.Function);

            // Department
            if (!string.IsNullOrEmpty(message.Contact.Department))
                WriteHeaderLine(contactHeader, htmlBody, maxLength, LanguageConsts.DepartmentLabel,
                    message.Contact.Department);

            // Company
            if (!string.IsNullOrEmpty(message.Contact.Company))
                WriteHeaderLine(contactHeader, htmlBody, maxLength, LanguageConsts.CompanyLabel, message.Contact.Company);

            // Empty line
            WriteHeaderEmptyLine(contactHeader, htmlBody);

            // Business address
            if (!string.IsNullOrEmpty(message.Contact.WorkAddress))
                WriteHeaderLine(contactHeader, htmlBody, maxLength, LanguageConsts.WorkAddressLabel,
                    message.Contact.WorkAddress);

            // Home address
            if (!string.IsNullOrEmpty(message.Contact.HomeAddress))
                WriteHeaderLine(contactHeader, htmlBody, maxLength, LanguageConsts.HomeAddressLabel,
                    message.Contact.HomeAddress);

            // Other address
            if (!string.IsNullOrEmpty(message.Contact.OtherAddress))
                WriteHeaderLine(contactHeader, htmlBody, maxLength, LanguageConsts.OtherAddressLabel,
                    message.Contact.OtherAddress);

            // Instant messaging
            if (!string.IsNullOrEmpty(message.Contact.InstantMessagingAddress))
                WriteHeaderLine(contactHeader, htmlBody, maxLength, LanguageConsts.InstantMessagingAddressLabel,
                    message.Contact.InstantMessagingAddress);

            // Empty line
            WriteHeaderEmptyLine(contactHeader, htmlBody);

            // Business telephone number
            if (!string.IsNullOrEmpty(message.Contact.BusinessTelephoneNumber))
                WriteHeaderLine(contactHeader, htmlBody, maxLength, LanguageConsts.BusinessTelephoneNumberLabel,
                    message.Contact.BusinessTelephoneNumber);

            // Business telephone number 2
            if (!string.IsNullOrEmpty(message.Contact.BusinessTelephoneNumber2))
                WriteHeaderLine(contactHeader, htmlBody, maxLength, LanguageConsts.BusinessTelephoneNumber2Label,
                    message.Contact.BusinessTelephoneNumber2);

            // Assistant's telephone number
            if (!string.IsNullOrEmpty(message.Contact.AssistantTelephoneNumber))
                WriteHeaderLine(contactHeader, htmlBody, maxLength, LanguageConsts.AssistantTelephoneNumberLabel,
                    message.Contact.AssistantTelephoneNumber);

            // Company main phone
            if (!string.IsNullOrEmpty(message.Contact.CompanyMainTelephoneNumber))
                WriteHeaderLine(contactHeader, htmlBody, maxLength, LanguageConsts.CompanyMainTelephoneNumberLabel,
                    message.Contact.CompanyMainTelephoneNumber);

            // Home telephone number
            if (!string.IsNullOrEmpty(message.Contact.HomeTelephoneNumber))
                WriteHeaderLine(contactHeader, htmlBody, maxLength, LanguageConsts.HomeTelephoneNumberLabel,
                    message.Contact.HomeTelephoneNumber);

            // Home telephone number 2
            if (!string.IsNullOrEmpty(message.Contact.HomeTelephoneNumber2))
                WriteHeaderLine(contactHeader, htmlBody, maxLength, LanguageConsts.HomeTelephoneNumber2Label,
                    message.Contact.HomeTelephoneNumber2);

            // Mobile phone
            if (!string.IsNullOrEmpty(message.Contact.CellularTelephoneNumber))
                WriteHeaderLine(contactHeader, htmlBody, maxLength, LanguageConsts.CellularTelephoneNumberLabel,
                    message.Contact.CellularTelephoneNumber);

            // Car phone
            if (!string.IsNullOrEmpty(message.Contact.CarTelephoneNumber))
                WriteHeaderLine(contactHeader, htmlBody, maxLength, LanguageConsts.CarTelephoneNumberLabel,
                    message.Contact.CarTelephoneNumber);

            // Radio
            if (!string.IsNullOrEmpty(message.Contact.RadioTelephoneNumber))
                WriteHeaderLine(contactHeader, htmlBody, maxLength, LanguageConsts.RadioTelephoneNumberLabel,
                    message.Contact.RadioTelephoneNumber);

            // Beeper
            if (!string.IsNullOrEmpty(message.Contact.BeeperTelephoneNumber))
                WriteHeaderLine(contactHeader, htmlBody, maxLength, LanguageConsts.BeeperTelephoneNumberLabel,
                    message.Contact.BeeperTelephoneNumber);

            // Callback
            if (!string.IsNullOrEmpty(message.Contact.CallbackTelephoneNumber))
                WriteHeaderLine(contactHeader, htmlBody, maxLength, LanguageConsts.CallbackTelephoneNumberLabel,
                    message.Contact.CallbackTelephoneNumber);

            // Other
            if (!string.IsNullOrEmpty(message.Contact.OtherTelephoneNumber))
                WriteHeaderLine(contactHeader, htmlBody, maxLength, LanguageConsts.OtherTelephoneNumberLabel,
                    message.Contact.OtherTelephoneNumber);

            // Primary telephone number
            if (!string.IsNullOrEmpty(message.Contact.PrimaryTelephoneNumber))
                WriteHeaderLine(contactHeader, htmlBody, maxLength, LanguageConsts.PrimaryTelephoneNumberLabel,
                    message.Contact.PrimaryTelephoneNumber);

            // Telex
            if (!string.IsNullOrEmpty(message.Contact.TelexNumber))
                WriteHeaderLine(contactHeader, htmlBody, maxLength, LanguageConsts.TelexNumberLabel,
                    message.Contact.TelexNumber);

            // TTY/TDD phone
            if (!string.IsNullOrEmpty(message.Contact.TextTelephone))
                WriteHeaderLine(contactHeader, htmlBody, maxLength, LanguageConsts.TextTelephoneLabel,
                    message.Contact.TextTelephone);

            // ISDN
            if (!string.IsNullOrEmpty(message.Contact.ISDNNumber))
                WriteHeaderLine(contactHeader, htmlBody, maxLength, LanguageConsts.ISDNNumberLabel,
                    message.Contact.ISDNNumber);

            // Other fax (primary fax, weird that they call it like this in Outlook)
            if (!string.IsNullOrEmpty(message.Contact.PrimaryFaxNumber))
                WriteHeaderLine(contactHeader, htmlBody, maxLength, LanguageConsts.PrimaryFaxNumberLabel,
                    message.Contact.OtherTelephoneNumber);

            // Business fax
            if (!string.IsNullOrEmpty(message.Contact.BusinessFaxNumber))
                WriteHeaderLine(contactHeader, htmlBody, maxLength, LanguageConsts.BusinessFaxNumberLabel,
                    message.Contact.BusinessFaxNumber);

            // Home fax
            if (!string.IsNullOrEmpty(message.Contact.HomeFaxNumber))
                WriteHeaderLine(contactHeader, htmlBody, maxLength, LanguageConsts.HomeFaxNumberLabel,
                    message.Contact.HomeFaxNumber);

            // Empty line
            WriteHeaderEmptyLine(contactHeader, htmlBody);

            // E-mail
            if (!string.IsNullOrEmpty(message.Contact.Email1EmailAddress))
                WriteHeaderLine(contactHeader, htmlBody, maxLength, LanguageConsts.Email1EmailAddressLabel,
                    message.Contact.Email1EmailAddress);

            // E-mail display as
            if (!string.IsNullOrEmpty(message.Contact.Email1DisplayName))
                WriteHeaderLine(contactHeader, htmlBody, maxLength, LanguageConsts.Email1DisplayNameLabel,
                    message.Contact.Email1DisplayName);

            // E-mail 2
            if (!string.IsNullOrEmpty(message.Contact.Email2EmailAddress))
                WriteHeaderLine(contactHeader, htmlBody, maxLength, LanguageConsts.Email2EmailAddressLabel,
                    message.Contact.Email2EmailAddress);

            // E-mail display as 2
            if (!string.IsNullOrEmpty(message.Contact.Email2DisplayName))
                WriteHeaderLine(contactHeader, htmlBody, maxLength, LanguageConsts.Email2DisplayNameLabel,
                    message.Contact.Email2DisplayName);

            // E-mail 3
            if (!string.IsNullOrEmpty(message.Contact.Email3EmailAddress))
                WriteHeaderLine(contactHeader, htmlBody, maxLength, LanguageConsts.Email3EmailAddressLabel,
                    message.Contact.Email3EmailAddress);

            // E-mail display as 3
            if (!string.IsNullOrEmpty(message.Contact.Email3DisplayName))
                WriteHeaderLine(contactHeader, htmlBody, maxLength, LanguageConsts.Email3DisplayNameLabel,
                    message.Contact.Email3DisplayName);

            // Empty line
            WriteHeaderEmptyLine(contactHeader, htmlBody);

            // Birthday
            if (message.Contact.Birthday != null)
                WriteHeaderLine(contactHeader, htmlBody, maxLength, LanguageConsts.BirthdayLabel,
                    ((DateTime)message.Contact.Birthday).ToString(LanguageConsts.DataFormat));

            // Anniversary
            if (message.Contact.WeddingAnniversary != null)
                WriteHeaderLine(contactHeader, htmlBody, maxLength, LanguageConsts.WeddingAnniversaryLabel,
                    ((DateTime)message.Contact.WeddingAnniversary).ToString(LanguageConsts.DataFormat));

            // Spouse/Partner
            if (!string.IsNullOrEmpty(message.Contact.SpouseName))
                WriteHeaderLine(contactHeader, htmlBody, maxLength, LanguageConsts.SpouseNameLabel,
                    message.Contact.SpouseName);

            // Profession
            if (!string.IsNullOrEmpty(message.Contact.Profession))
                WriteHeaderLine(contactHeader, htmlBody, maxLength, LanguageConsts.ProfessionLabel,
                    message.Contact.Profession);

            // Assistant
            if (!string.IsNullOrEmpty(message.Contact.AssistantName))
                WriteHeaderLine(contactHeader, htmlBody, maxLength, LanguageConsts.AssistantTelephoneNumberLabel,
                    message.Contact.AssistantName);

            // Web page
            if (!string.IsNullOrEmpty(message.Contact.Html))
                WriteHeaderLine(contactHeader, htmlBody, maxLength, LanguageConsts.HtmlLabel, message.Contact.Html);

            // Empty line
            WriteHeaderEmptyLine(contactHeader, htmlBody);

            // Categories
            var categories = message.Categories;
            if (categories != null)
                WriteHeaderLine(contactHeader, htmlBody, maxLength, LanguageConsts.EmailCategoriesLabel,
                    String.Join("; ", categories));

            // Empty line
            WriteHeaderEmptyLine(contactHeader, htmlBody);

            WriteHeaderEnd(contactHeader, htmlBody);

            body = InjectHeader(body, contactHeader.ToString());

            // Write the body to a file
            File.WriteAllText(fileName, body, Encoding.UTF8);

            return files;
        }
        #endregion
    }
}

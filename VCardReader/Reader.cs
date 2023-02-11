﻿//
// Reader.cs
//
// Author: Kees van Spelde <sicos2002@hotmail.com>
//
// Copyright (c) 2014-2023 Magic-Sessions. (www.magic-sessions.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NON INFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using VCardReader.Exceptions;
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
        #endregion

        #region Properties
        /// <summary>
        ///     An unique id that can be used to identify the logging of the converter when
        ///     calling the code from multiple threads and writing all the logging to the same file
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public string InstanceId
        {
            get => Logger.InstanceId;
            set => Logger.InstanceId = value;
        }
        #endregion

        #region Constructor
        /// <summary>
        ///     Creates this object and sets it's needed properties
        /// </summary>
        /// <param name="logStream">When set then logging is written to this stream for all conversions. If
        /// you want a separate log for each conversion then set the logstream on the <see cref="Convert"/> method</param>
        public Reader(Stream logStream = null)
        {
            Logger.LogStream = logStream;
        }
        #endregion

        #region SetCulture
        /// <summary>
        ///     Sets the culture that needs to be used to localize the output of this class.
        ///     Default the current system culture is set. When there is no localization available the
        ///     default will be used. This will be en-US.
        /// </summary>
        /// <param name="name">The name of the culture eg. nl-NL</param>
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
            {
                var message = $"Could not find the input file '{inputFile}'";
                Logger.WriteToLog(message);
                throw new FileNotFoundException(message);
            }

            var directoryInfo = new DirectoryInfo(outputFolder);

            if (!directoryInfo.Exists)
            {
                var message = $"The output folder '{outputFolder}' does not exist";
                Logger.WriteToLog(message);
                throw new DirectoryNotFoundException(message);
            }

            var extension = Path.GetExtension(inputFile);
            if (string.IsNullOrEmpty(extension))
            {
                var message = "Expected .vcf extension on the inputfile";
                Logger.WriteToLog(message);
                throw new VCRFileTypeNotSupported(message);
            }

            extension = extension.ToUpperInvariant();

            switch (extension)
            {
                case ".VCF":
                    return;

                default:
                {
                    var message = "Wrong file extension, expected .vcf";
                    Logger.WriteToLog(message);
                    throw new VCRFileTypeNotSupported(message);
                }
            }
        }
        #endregion

        #region ExtractToFolder
        /// <summary>
        /// This method will read the given <paramref name="inputFile"/> convert it to HTML and write it to the <paramref name="outputFolder"/>
        /// </summary>
        /// <param name="inputFile">The vcf file</param>
        /// <param name="outputFolder">The folder where to save the converted vcf file</param>
        /// <param name="hyperlinks">When true hyperlinks are click able, otherwise they are written as plain text</param>
        /// <returns>String array containing the full path to the converted VCF file</returns>
        /// <param name="logStream">When set then logging is written to this stream</param>
        /// <exception cref="ArgumentNullException">Raised when the <paramref name="inputFile" /> or <paramref name="outputFolder" /> is null or empty</exception>
        /// <exception cref="FileNotFoundException">Raised when the <paramref name="inputFile" /> does not exists</exception>
        /// <exception cref="DirectoryNotFoundException">Raised when the <paramref name="outputFolder" /> does not exist</exception>
        /// <exception cref="VCRFileTypeNotSupported">Raised when the extension is not .vcf</exception>
        public List<string> ExtractToFolder(string inputFile, string outputFolder, bool hyperlinks = false, Stream logStream = null)
        {
            if (logStream != null)
                Logger.LogStream = logStream;

            outputFolder = FileManager.CheckForBackSlash(outputFolder);
            CheckFileNameAndOutputFolder(inputFile, outputFolder);

            Logger.WriteToLog($"Reading inputfile '{inputFile}'");

            using (TextReader textReader = File.OpenText(inputFile))
            {
                var vcardReader = new VCardReader();
                var vCard = new VCard();
                Logger.WriteToLog("Start reading vCard information");
                vcardReader.ReadInto(vCard, textReader);
                Logger.WriteToLog("Done reading vCard information");
                Logger.WriteToLog("Start writing vCard information to file(s)");
                var result = WriteVCard(vCard, outputFolder, true);
                Logger.WriteToLog("Done writing vCard information to file(s)");
                return result;
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
        ///     Writes a row to the table
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
        ///     Writes a row to the table without Html encoding the <paramref name="text" />
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
        ///     Writes a row tot the table and makes <paramref name="text"/> click able (hyperlink) />
        /// </summary>
        /// <param name="header">The <see cref="StringBuilder" /> object that is used to write a table</param>
        /// <param name="label">The label text that needs to be written</param>
        /// <param name="hyperlink">The hyperlink</param>
        /// <param name="text">The text for the hyperlink</param>
        private static void WriteTableRowHyperLink(StringBuilder header,
                                                   string label,
                                                   string hyperlink,
                                                   string text)
        {
            text = text.Replace("\n", "<br/>");

            header.AppendLine(
                "<tr style=\"height: 18px; vertical-align: top; \"><td style=\"font-weight: bold; white-space:nowrap; \">" +
                HttpUtility.HtmlEncode(label) + ":</td><td><a href=\"" + hyperlink + "\">" + text + "<a></td></tr>");

            _emptyLineWritten = false;
        }

        /// <summary>
        ///     Writes a row tot the table and inserts the given <paramref name="imageUrl"/>
        /// </summary>
        /// <param name="header">The <see cref="StringBuilder" /> object that is used to write a table</param>
        /// <param name="label">The label text that needs to be written</param>
        /// <param name="imageUrl">The url to the image</param>
        private static void WriteTableRowImage(StringBuilder header,
                                               string label,
                                               string imageUrl)
        {
            header.AppendLine(
                "<tr style=\"height: 18px; vertical-align: top; \"><td style=\"font-weight: bold; white-space:nowrap; \">" +
                HttpUtility.HtmlEncode(label) + ":</td><td><img src=\"" + imageUrl + "\"></td></tr>");

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

        #region WriteTelephone
        /// <summary>
        ///     Writes the selected <paramref name="phoneTypes"/> to the given <paramref name="output"/> for
        ///     the selected <paramref name="vCard"/>
        /// </summary>
        /// <param name="vCard"></param>
        /// <param name="output"></param>
        /// <param name="phoneTypes"></param>
        private void WriteTelephone(VCard vCard, StringBuilder output, ICollection<PhoneTypes> phoneTypes)
        {

            var phones = vCard.Phones.Where(m => phoneTypes.Contains(m.PhoneType)).ToList();
            var i = 0;
            var count = phones.Count;

            foreach (var phone in phones)
            {
                var labelSuffix = string.Empty;

                if (count > 0)
                {
                    i += 1;
                    if (i > 1)
                        labelSuffix = " " + i;
                }

                switch (phone.PhoneType)
                {
                    case PhoneTypes.Bbs:
                        WriteTableRow(output, LanguageConsts.BBSTelephoneNumberLabel + labelSuffix, phone.FullNumber);
                        break;

                    case PhoneTypes.Car:
                    case PhoneTypes.CarVoice:
                        WriteTableRow(output, LanguageConsts.CarTelephoneNumberLabel + labelSuffix, phone.FullNumber);
                        break;

                    case PhoneTypes.Cellular:
                    case PhoneTypes.CellularVoice:
                        WriteTableRow(output, LanguageConsts.CellularTelephoneNumberLabel + labelSuffix,
                            phone.FullNumber);
                        break;

                    case PhoneTypes.Fax:
                        WriteTableRow(output, LanguageConsts.OtherFaxLabel + labelSuffix, phone.FullNumber);
                        break;

                    case PhoneTypes.HomeFax:
                        WriteTableRow(output, LanguageConsts.HomeFaxNumberLabel + labelSuffix, phone.FullNumber);
                        break;

                    case PhoneTypes.Home:
                    case PhoneTypes.HomeVoice:
                        WriteTableRow(output, LanguageConsts.HomeTelephoneNumberLabel + labelSuffix, phone.FullNumber);
                        break;

                    case PhoneTypes.Isdn:
                        WriteTableRow(output, LanguageConsts.ISDNNumberLabel + labelSuffix, phone.FullNumber);
                        break;

                    case PhoneTypes.MessagingService:
                        WriteTableRow(output, LanguageConsts.BeeperTelephoneNumberLabel + labelSuffix, phone.FullNumber);
                        break;

                    case PhoneTypes.Modem:
                        WriteTableRow(output, LanguageConsts.ModemTelephoneNumberLabel + labelSuffix, phone.FullNumber);
                        break;

                    case PhoneTypes.Pager:
                    case PhoneTypes.VoicePager:
                        WriteTableRow(output, LanguageConsts.BeeperTelephoneNumberLabel + labelSuffix, phone.FullNumber);
                        break;

                    case PhoneTypes.Ttytdd:
                        WriteTableRow(output, LanguageConsts.TextTelephoneLabel + labelSuffix, phone.FullNumber);
                        break;

                    case PhoneTypes.Preferred:
                        WriteTableRow(output, LanguageConsts.PrimaryTelephoneNumberLabel + labelSuffix, phone.FullNumber);
                        break;

                    case PhoneTypes.Video:
                        WriteTableRow(output, LanguageConsts.VideoTelephoneNumberLabel + labelSuffix, phone.FullNumber);
                        break;

                    case PhoneTypes.Voice:
                        WriteTableRow(output, LanguageConsts.VoiceTelephoneNumberLabel + labelSuffix, phone.FullNumber);
                        break;

                    case PhoneTypes.Work:
                    case PhoneTypes.WorkVoice:
                        WriteTableRow(output, LanguageConsts.BusinessTelephoneNumberLabel + labelSuffix,
                            phone.FullNumber);
                        break;

                    case PhoneTypes.Company:
                    case PhoneTypes.VoiceCompany:
                        WriteTableRow(output, LanguageConsts.CompanyMainTelephoneNumberLabel + labelSuffix,
                            phone.FullNumber);
                        break;

                    case PhoneTypes.Callback:
                    case PhoneTypes.VoiceCallback:
                        WriteTableRow(output, LanguageConsts.CallbackTelephoneNumberLabel + labelSuffix,
                            phone.FullNumber);
                        break;

                    case PhoneTypes.Radio:
                    case PhoneTypes.VoiceRadio:
                        WriteTableRow(output, LanguageConsts.RadioTelephoneNumberLabel + labelSuffix, phone.FullNumber);
                        break;

                    case PhoneTypes.Assistant:
                    case PhoneTypes.VoiceAssistant:
                        WriteTableRow(output, LanguageConsts.AssistantLabel + labelSuffix, phone.FullNumber);
                        break;

                    case PhoneTypes.WorkFax:
                        WriteTableRow(output, LanguageConsts.BusinessFaxNumberLabel + labelSuffix, phone.FullNumber);
                        break;
                }
            }
        }
        #endregion

        #region WriteVCard
        /// <summary>
        ///     Writes the body of the MSG Contact to html or text and extracts all the attachments. The
        ///     result is return as a List of strings
        /// </summary>
        /// <param name="vCard">
        ///     <see cref="VCard" />
        /// </param>
        /// <param name="outputFolder">The folder where we need to write the output</param>
        /// <param name="hyperlinks">When <c>true</c> then hyperlinks are generated for the To, CC, BCC and attachments</param>
        /// <returns></returns>
        private List<string> WriteVCard(VCard vCard, string outputFolder, bool hyperlinks)
        {
            var fileName = Path.Combine(outputFolder, "contact.html");
            var files = new List<string> {fileName};
            
            var output = new StringBuilder();

            // Start of table
            WriteTableStart(output);

            var i = 1;
            var count = vCard.Photos.Count;

            foreach (var photo in vCard.Photos)
            {
                string photoLabel;
                if (count > 1)
                    photoLabel = LanguageConsts.PhotoLabel + " " + i;
                else
                    photoLabel = LanguageConsts.PhotoLabel;

                if (photo.IsLoaded)
                {
                    var tempFileName = Path.Combine(outputFolder, Guid.NewGuid() + ".png");
                    var bitmap = photo.GetBitmap();
                    bitmap.Save(tempFileName, ImageFormat.Png);
                    files.Add(tempFileName);
                    WriteTableRowImage(output, photoLabel, tempFileName);
                }
                else
                {
                    if (hyperlinks)
                        WriteTableRowHyperLink(output, photoLabel, photo.Url.ToString(), photo.Url.ToString());
                    else
                        WriteTableRowImage(output, photoLabel, photo.Url.ToString());
                }

                i += 1;
            }

            // Full name
            if (!string.IsNullOrEmpty(vCard.FormattedName))
                WriteTableRow(output, LanguageConsts.DisplayNameLabel, vCard.FormattedName);

            // Last name
            if (!string.IsNullOrEmpty(vCard.FamilyName))
                WriteTableRow(output, LanguageConsts.SurNameLabel, vCard.FamilyName);

            // First name
            if (!string.IsNullOrEmpty(vCard.GivenName))
                WriteTableRow(output, LanguageConsts.GivenNameLabel,
                    vCard.GivenName);

            // Job title
            if (!string.IsNullOrEmpty(vCard.Title))
                WriteTableRow(output, LanguageConsts.FunctionLabel,
                    vCard.Title);

            // Department
            if (!string.IsNullOrEmpty(vCard.Department))
                WriteTableRow(output, LanguageConsts.DepartmentLabel,
                    vCard.Department);

            // Company
            if (!string.IsNullOrEmpty(vCard.Organization))
                WriteTableRow(output, LanguageConsts.CompanyLabel, vCard.Organization);

            // Empty line
            WriteEmptyTableRow(output);

            if (vCard.DeliveryLabels.Count == 0)
                foreach (var deliveryAddress in vCard.DeliveryAddresses)
                {
                    var address = (!string.IsNullOrWhiteSpace(deliveryAddress.Street) ? deliveryAddress.Street : string.Empty) + Environment.NewLine +
                        (!string.IsNullOrWhiteSpace(deliveryAddress.PostalCode) ? deliveryAddress.PostalCode : string.Empty) + " " +
                        (!string.IsNullOrWhiteSpace(deliveryAddress.City) ? deliveryAddress.City : string.Empty) + Environment.NewLine +
                        (!string.IsNullOrWhiteSpace(deliveryAddress.Region) ? deliveryAddress.Region : string.Empty) + Environment.NewLine +
                        (!string.IsNullOrWhiteSpace(deliveryAddress.Country) ? deliveryAddress.Country : string.Empty);

                    // intl, postal, parcel, work
                    if (deliveryAddress.IsWork)
                        WriteTableRow(output, LanguageConsts.WorkAddressLabel, address);
                    else if (deliveryAddress.IsHome)
                        WriteTableRow(output, LanguageConsts.HomeAddressLabel, address);
                    else if (deliveryAddress.IsInternational)
                        WriteTableRow(output, LanguageConsts.InternationalAddressLabel, address);
                    else if (deliveryAddress.IsPostal)
                        WriteTableRow(output, LanguageConsts.PostalAddressLabel, address);
                    else if (deliveryAddress.IsParcel)
                        WriteTableRow(output, LanguageConsts.ParcelAddressLabel, address);
                    else if (deliveryAddress.IsDomestic)
                        WriteTableRow(output, LanguageConsts.DomesticAddressLabel, address);
                }

            // Business address
            foreach (var deliveryLabel in vCard.DeliveryLabels)
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
            if (!string.IsNullOrEmpty(vCard.InstantMessagingAddress))
                WriteTableRow(output, LanguageConsts.InstantMessagingAddressLabel, vCard.InstantMessagingAddress);
            
            // Empty line
            WriteEmptyTableRow(output);

            WriteTelephone(vCard, output, new List<PhoneTypes> {PhoneTypes.Work, PhoneTypes.WorkVoice});
            WriteTelephone(vCard, output, new List<PhoneTypes> {PhoneTypes.Assistant, PhoneTypes.VoiceAssistant});
            WriteTelephone(vCard, output, new List<PhoneTypes> {PhoneTypes.Company, PhoneTypes.VoiceCompany});
            WriteTelephone(vCard, output, new List<PhoneTypes> {PhoneTypes.Home, PhoneTypes.HomeVoice});
            WriteTelephone(vCard, output, new List<PhoneTypes> {PhoneTypes.Cellular, PhoneTypes.CellularVoice});
            WriteTelephone(vCard, output, new List<PhoneTypes> {PhoneTypes.Car, PhoneTypes.CarVoice});
            WriteTelephone(vCard, output, new List<PhoneTypes> {PhoneTypes.Radio, PhoneTypes.VoiceRadio});
            WriteTelephone(vCard, output, new List<PhoneTypes> {PhoneTypes.Pager, PhoneTypes.VoicePager});
            WriteTelephone(vCard, output, new List<PhoneTypes> {PhoneTypes.Callback, PhoneTypes.VoiceCallback});
            WriteTelephone(vCard, output, new List<PhoneTypes> {PhoneTypes.Voice});
            WriteTelephone(vCard, output, new List<PhoneTypes> {PhoneTypes.Preferred});

            // Telex
            foreach (var email in vCard.EmailAddresses)
            {
                switch (email.EmailType)
                {
                    case EmailAddressType.Telex:
                        WriteTableRow(output, LanguageConsts.TelexNumberLabel, email.ToString());
                        break;
                }
            }
            WriteTelephone(vCard, output, new List<PhoneTypes> { PhoneTypes.Ttytdd });
            WriteTelephone(vCard, output, new List<PhoneTypes> {PhoneTypes.Isdn});
            WriteTelephone(vCard, output, new List<PhoneTypes> {PhoneTypes.Fax});
            WriteTelephone(vCard, output, new List<PhoneTypes> {PhoneTypes.WorkFax});
            WriteTelephone(vCard, output, new List<PhoneTypes> {PhoneTypes.HomeFax});

            // Empty line
            WriteEmptyTableRow(output);

            i = 1;

            foreach (var email in vCard.EmailAddresses)
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
                        if (i > 1)
                            WriteEmptyTableRow(output);

                        if (hyperlinks)
                            WriteTableRowHyperLink(output, LanguageConsts.EmailEmailAddressLabel + " " + i, "mailto:" + email, email.ToString());
                        else
                            WriteTableRowNoEncoding(output, LanguageConsts.EmailEmailAddressLabel + " " + i, email.ToString());

                        if (!string.IsNullOrEmpty(vCard.FormattedName))
                            WriteTableRow(output, LanguageConsts.EmailDisplayNameLabel + " " + i,
                                vCard.FormattedName + " (" + email + ")");

                        i += 1;
                        break;

                    case EmailAddressType.Telex:
                        // Ignore
                        break;

                }
            }

            // Empty line
            WriteEmptyTableRow(output);
            
            // Birthday
            if (vCard.BirthDate != null)
                WriteTableRow(output, LanguageConsts.BirthdayLabel,
                    ((DateTime)vCard.BirthDate).ToString(LanguageConsts.DataFormat));

            // Anniversary
            if (vCard.Anniversary != null)
                WriteTableRow(output, LanguageConsts.WeddingAnniversaryLabel,
                    ((DateTime)vCard.Anniversary).ToString(LanguageConsts.DataFormat));
            
            // Spouse/Partner
            if (!string.IsNullOrEmpty(vCard.Spouse))
                WriteTableRow(output, LanguageConsts.SpouseNameLabel,
                    vCard.Spouse);

            // Profession
            if (!string.IsNullOrEmpty(vCard.Role))
                WriteTableRow(output, LanguageConsts.ProfessionLabel,
                    vCard.Role);

            // Assistant
            if (!string.IsNullOrEmpty(vCard.Assistant))
                WriteTableRow(output, LanguageConsts.AssistantTelephoneNumberLabel,
                    vCard.Assistant);

            
            // Web page
            var firstRow = true;
            foreach (var webpage in vCard.Websites)
            {
                if (!string.IsNullOrEmpty(webpage.Url))
                {
                    if (firstRow)
                    {
                        firstRow = false;
                        if (hyperlinks)
                            WriteTableRowHyperLink(output, LanguageConsts.HtmlLabel, webpage.Url, webpage.Url);
                        else
                            WriteTableRow(output, LanguageConsts.HtmlLabel, webpage.Url);
                    }
                    else
                    {
                        if (hyperlinks)
                            WriteTableRowHyperLink(output, string.Empty, webpage.Url, webpage.Url);
                        else
                            WriteTableRow(output, string.Empty, webpage.Url);
                    }
                }
            }

            // Empty line
            WriteEmptyTableRow(output);
            
            // Categories
            var categories = vCard.Categories;
            if (categories != null && categories.Count > 0)
                WriteTableRow(output, LanguageConsts.CategoriesLabel,
                    String.Join("; ", categories));

            // Empty line
            WriteEmptyTableRow(output);

            // Empty line
            firstRow = true;
            if (vCard.Notes != null && vCard.Notes.Count > 0)
            {
                foreach (var note in vCard.Notes)
                {
                    if (!string.IsNullOrWhiteSpace(note.Text))
                    {
                        if (firstRow)
                        {
                            firstRow = false;
                            WriteTableRow(output, LanguageConsts.NotesLabel, note.Text);
                        }
                        else
                            WriteTableRow(output, string.Empty, note.Text);
                    }
                }
            }

            WriteTableEnd(output);

            // Write the body to a file
            File.WriteAllText(fileName, output.ToString(), Encoding.UTF8);

            return files;
        }
        #endregion
    }
}
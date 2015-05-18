using System;
using System.Collections.Specialized;
using System.IO;
using VCardReader.Collections;

namespace VCardReader
{
    #region VCardEncoding
    /// <summary>
    ///     The encoding used to store a vCard property value in text format.
    /// </summary>
    public enum VCardEncoding
    {
        /// <summary>
        ///     Unknown or no encoding.
        /// </summary>
        Unknown = 0,

        /// <summary>
        ///     Standard escaped text.
        /// </summary>
        Escaped,

        /// <summary>
        ///     Binary or BASE64 encoding.
        /// </summary>
        Base64,

        /// <summary>
        ///     Quoted-Printable encoding.
        /// </summary>
        QuotedPrintable
    }
    #endregion

    /// <summary>
    ///     A vCard object for exchanging personal contact information.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A vCard contains personal information, such as postal addresses, public security certificates, email addresses, and
    ///         web sites.  The vCard specification makes it possible for different computer programs to exchange personal contact
    ///         information; for example, a vCard can be attached to an email or sent over a wireless connection.
    ///     </para>
    ///     <para>
    ///         The standard vCard format is a text file with properties in name:value format.  However, there are multiple versions of
    ///         this format as well as compatible alternatives in XML and HTML formats.  This class library aims to accomodate these
    ///         variations but be aware some some formats do not support all possible properties.
    ///     </para>
    /// </remarks>
    [Serializable]
    public class VCard
    {
        #region Fields
        private readonly StringCollection _nicknames;
        [NonSerialized] private readonly NoteCollection _notes;
        [NonSerialized] private readonly PhotoCollection _photos;
        private string _additionalNames;
        private string _department;
        private string _displayName;
        private string _familyName;
        private string _formattedName;
        private string _givenName;
        private float? _latitude;
        private float? _longitude;
        private string _mailer;
        private string _namePrefix;
        private string _nameSuffix;
        private string _office;
        private string _organization;
        private string _productId;
        private DateTime? _revisionDate;
        private string _role;
        private string _timeZone;
        private string _title;
        private string _uniqueId;
        #endregion

        #region AccessClassification
        /// <summary>
        ///     The security access classification of the vCard owner (e.g. private).
        /// </summary>
        public AccessClassification AccessClassification { get; set; }
        #endregion

        #region AdditionalNames
        /// <summary>
        ///     Any additional (e.g. middle) names of the person.
        /// </summary>
        /// <seealso cref="FamilyName" />
        /// <seealso cref="FormattedName" />
        /// <seealso cref="GivenName" />
        /// <seealso cref="Nicknames" />
        public string AdditionalNames
        {
            get { return _additionalNames ?? string.Empty; }
            set { _additionalNames = value; }
        }
        #endregion

        #region Anniversary
        /// <summary>
        ///     The anniversary of the person.
        /// </summary>
        /// <remarks>
        ///     Only available when the contact card has been generated with Microsoft Outlook
        /// </remarks>
        public DateTime? Anniversary { get; set; }
        #endregion

        #region Assistant
        /// <summary>
        ///     The assistant.
        /// </summary>
        /// <remarks>
        ///     Only available when the contact card has been generated with Microsoft Outlook
        /// </remarks>
        public string Assistant { get; set; }
        #endregion

        #region BirthDate
        /// <summary>
        ///     The birthdate of the person.
        /// </summary>
        public DateTime? BirthDate { get; set; }
        #endregion

        #region Categories
        /// <summary>
        ///     Categories of the vCard.
        /// </summary>
        /// <remarks>
        ///     This property is a collection of strings containing keywords or category names.
        /// </remarks>
        public StringCollection Categories { get; private set; }
        #endregion

        #region Certificates
        /// <summary>
        ///     Public key certificates attached to the vCard.
        /// </summary>
        /// <seealso cref="Certificate" />
        public CertificateCollection Certificates { get; private set; }
        #endregion

        #region DeliveryAddresses
        /// <summary>
        ///     Delivery addresses associated with the person.
        /// </summary>
        public DeliveryAddressCollection DeliveryAddresses { get; private set; }
        #endregion

        #region DeliveryLabels
        /// <summary>
        ///     Formatted delivery labels.
        /// </summary>
        public DeliveryLabelCollection DeliveryLabels { get; private set; }
        #endregion

        #region Department
        /// <summary>
        ///     The department of the person in the organization.
        /// </summary>
        /// <seealso cref="Office" />
        /// <seealso cref="Organization" />
        public string Department
        {
            get { return _department ?? string.Empty; }
            set { _department = value; }
        }
        #endregion

        #region DisplayName
        /// <summary>
        ///     The display name of the vCard.
        /// </summary>
        /// <remarks>
        ///     This property is used by vCard applications for titles, headers, and other visual elements.
        /// </remarks>
        public string DisplayName
        {
            get { return _displayName ?? string.Empty; }
            set { _displayName = value; }
        }
        #endregion

        #region EmailAddresses
        /// <summary>
        ///     A collection of <see cref="EmailAddress" /> objects for the person.
        /// </summary>
        /// <seealso cref="EmailAddress" />
        public EmailAddressCollection EmailAddresses { get; private set; }
        #endregion

        #region FamilyName
        /// <summary>
        ///     The family (last) name of the person.
        /// </summary>
        /// <seealso cref="AdditionalNames" />
        /// <seealso cref="FormattedName" />
        /// <seealso cref="GivenName" />
        /// <seealso cref="Nicknames" />
        public string FamilyName
        {
            get { return _familyName ?? string.Empty; }
            set { _familyName = value; }
        }
        #endregion

        #region FormattedName
        /// <summary>
        ///     The formatted name of the person.
        /// </summary>
        /// <remarks>
        ///     This property allows the name of the person to be written in a manner specific to his or her culture.
        ///     The formatted name is not required to strictly correspond with the family name, given name, etc.
        /// </remarks>
        /// <seealso cref="AdditionalNames" />
        /// <seealso cref="FamilyName" />
        /// <seealso cref="GivenName" />
        /// <seealso cref="Nicknames" />
        public string FormattedName
        {
            get { return _formattedName ?? string.Empty; }
            set { _formattedName = value; }
        }
        #endregion

        #region Gender
        /// <summary>
        ///     The gender of the person.
        /// </summary>
        /// <remarks>
        ///     The vCard specification does not define a property to indicate the gender of the contact.  
        ///     Microsoft Outlook implements it as a custom property named X-WAB-GENDER.
        /// </remarks>
        /// <seealso cref="Gender" />
        public Gender Gender { get; set; }
        #endregion

        #region GivenName
        /// <summary>
        ///     The given (first) name of the person.
        /// </summary>
        /// <seealso cref="AdditionalNames" />
        /// <seealso cref="FamilyName" />
        /// <seealso cref="FormattedName" />
        /// <seealso cref="Nicknames" />
        public string GivenName
        {
            get { return _givenName ?? string.Empty; }
            set { _givenName = value; }
        }
        #endregion

        #region InstantMessagingAddress
        /// <summary>
        ///     The instantMessagingAddress of the person.
        /// </summary>
        /// <remarks>
        ///     Only available when the contact card has been generated with Microsoft Outlook
        /// </remarks>
        public string InstantMessagingAddress { get; set; }
        #endregion

        #region Latitude
        /// <summary>
        ///     The latitude of the person in decimal degrees.
        /// </summary>
        /// <seealso cref="Longitude" />
        public float? Latitude
        {
            get { return _latitude; }
            set { _latitude = value; }
        }
        #endregion

        #region Longitude
        /// <summary>
        ///     The longitude of the person in decimal degrees.
        /// </summary>
        /// <seealso cref="Latitude" />
        public float? Longitude
        {
            get { return _longitude; }
            set { _longitude = value; }
        }
        #endregion

        #region Mailer
        /// <summary>
        ///     The mail software used by the person.
        /// </summary>
        public string Mailer
        {
            get { return _mailer ?? string.Empty; }
            set { _mailer = value; }
        }
        #endregion

        #region Manager
        /// <summary>
        ///     The Manager.
        /// </summary>
        /// <remarks>
        ///     Only available when the contact card has been generated with Microsoft Outlook
        /// </remarks>
        public string Manager { get; set; }
        #endregion

        #region NamePrefix
        /// <summary>
        ///     The prefix (e.g. "Mr.") of the person.
        /// </summary>
        /// <seealso cref="NameSuffix" />
        public string NamePrefix
        {
            get { return _namePrefix ?? string.Empty; }
            set { _namePrefix = value; }
        }
        #endregion

        #region NameSuffix
        /// <summary>
        ///     The suffix (e.g. "Jr.") of the person.
        /// </summary>
        /// <seealso cref="NamePrefix" />
        public string NameSuffix
        {
            get { return _nameSuffix ?? string.Empty; }
            set { _nameSuffix = value; }
        }
        #endregion

        #region Nicknames
        /// <summary>
        ///     A collection of nicknames for the person.
        /// </summary>
        /// <seealso cref="AdditionalNames" />
        /// <seealso cref="FamilyName" />
        /// <seealso cref="FormattedName" />
        /// <seealso cref="GivenName" />
        public StringCollection Nicknames
        {
            get { return _nicknames; }
        }
        #endregion

        #region Notes
        /// <summary>
        ///     A collection of notes or comments.
        /// </summary>
        public NoteCollection Notes
        {
            get { return _notes; }
        }
        #endregion

        #region Office
        /// <summary>
        ///     The office of the person at the organization.
        /// </summary>
        /// <seealso cref="Department" />
        /// <seealso cref="Organization" />
        public string Office
        {
            get { return _office ?? string.Empty; }
            set { _office = value; }
        }
        #endregion

        #region Organization
        /// <summary>
        ///     The organization or company of the person.
        /// </summary>
        /// <seealso cref="Office" />
        /// <seealso cref="Role" />
        /// <seealso cref="Title" />
        public string Organization
        {
            get { return _organization ?? string.Empty; }
            set { _organization = value; }
        }
        #endregion

        #region Phones
        /// <summary>
        ///     A collection of telephone numbers.
        /// </summary>
        public PhoneCollection Phones { get; private set; }
        #endregion

        #region Photos
        /// <summary>
        ///     A collection of photographic images embedded or referenced by the vCard.
        /// </summary>
        public PhotoCollection Photos
        {
            get { return _photos; }
        }
        #endregion

        #region ProductId
        /// <summary>
        ///     The name of the product that generated the vCard.
        /// </summary>
        public string ProductId
        {
            get { return _productId ?? string.Empty; }
            set { _productId = value; }
        }
        #endregion

        #region RevisionDate
        /// <summary>
        ///     The revision date of the vCard.
        /// </summary>
        /// <remarks>
        ///     The revision date is not automatically updated by the
        ///     vCard when modifying properties.  It is up to the
        ///     developer to change the revision date as needed.
        /// </remarks>
        public DateTime? RevisionDate
        {
            get { return _revisionDate; }
            set { _revisionDate = value; }
        }
        #endregion

        #region Role
        /// <summary>
        ///     The role of the person (e.g. Executive).
        /// </summary>
        /// <remarks>
        ///     The role is shown as "Profession" in Microsoft Outlook.
        /// </remarks>
        /// <seealso cref="Department" />
        /// <seealso cref="Office" />
        /// <seealso cref="Organization" />
        /// <seealso cref="Title" />
        public string Role
        {
            get { return _role ?? string.Empty; }
            set { _role = value; }
        }
        #endregion

        #region Sources
        /// <summary>
        ///     Directory sources for the vCard information.
        /// </summary>
        /// <remarks>
        ///     A vCard may contain zero or more sources. A source identifies a directory that contains (or provided)
        ///     information found in the vCard. A program can hypothetically connect to the source in order to
        ///     obtain updated information.
        /// </remarks>
        public SourceCollection Sources { get; private set; }
        #endregion

        #region Spouse
        /// <summary>
        ///     The Spouse.
        /// </summary>
        /// <remarks>
        ///     Only available when the contact card has been generated with Microsoft Outlook
        /// </remarks>
        public string Spouse { get; set; }
        #endregion

        #region TimeZone
        /// <summary>
        ///     A string identifying the time zone of the entity
        ///     represented by the vCard.
        /// </summary>
        public string TimeZone
        {
            get { return _timeZone ?? string.Empty; }
            set { _timeZone = value; }
        }
        #endregion

        #region Title
        /// <summary>
        ///     The job title of the person.
        /// </summary>
        /// <seealso cref="Organization" />
        /// <seealso cref="Role" />
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }
        #endregion

        #region ToString
        /// <summary>
        ///     Builds a string that represents the vCard.
        /// </summary>
        /// <returns>
        ///     The formatted name of the contact person, if defined, or the default object.ToString().
        /// </returns>
        public override string ToString()
        {
            return string.IsNullOrEmpty(_formattedName) ? base.ToString() : _formattedName;
        }
        #endregion

        #region UniqueId
        /// <summary>
        ///     A value that uniquely identifies the vCard.
        /// </summary>
        /// <remarks>
        ///     This value is optional. The string must be any string that can be used to uniquely identify the vCard.  The
        ///     usage of the field is determined by the software. Typical possibilities for a unique string include a URL, a GUID,
        ///     or an LDAP directory path.  However, there is no particular standard dictated by the vCard specification.
        /// </remarks>
        public string UniqueId
        {
            get { return _uniqueId ?? string.Empty; }
            set { _uniqueId = value; }
        }
        #endregion

        #region Websites
        /// <summary>
        ///     Web sites associated with the person.
        /// </summary>
        /// <seealso cref="Website" />
        /// <seealso cref="WebsiteCollection" />
        public WebsiteCollection Websites { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="VCard" /> class.
        /// </summary>
        public VCard()
        {
            // Per Microsoft best practices, string properties should
            // never return null.  String properties should always
            // return String.Empty.

            _additionalNames = string.Empty;
            _department = string.Empty;
            _displayName = string.Empty;
            _familyName = string.Empty;
            _formattedName = string.Empty;
            _givenName = string.Empty;
            _mailer = string.Empty;
            _namePrefix = string.Empty;
            _nameSuffix = string.Empty;
            _office = string.Empty;
            _organization = string.Empty;
            _productId = string.Empty;
            _role = string.Empty;
            _timeZone = string.Empty;
            _title = string.Empty;
            _uniqueId = string.Empty;

            Categories = new StringCollection();
            Certificates = new CertificateCollection();
            DeliveryAddresses = new DeliveryAddressCollection();
            DeliveryLabels = new DeliveryLabelCollection();
            EmailAddresses = new EmailAddressCollection();
            _nicknames = new StringCollection();
            _notes = new NoteCollection();
            Phones = new PhoneCollection();
            _photos = new PhotoCollection();
            Sources = new SourceCollection();
            Websites = new WebsiteCollection();
        }

        /// <summary>
        ///     Loads a new instance of the <see cref="VCard" /> class from a text reader.
        /// </summary>
        /// <param name="input">
        ///     An initialized text reader.
        /// </param>
        public VCard(TextReader input) : this()
        {
            var reader = new VCardReader();
            reader.ReadInto(this, input);
        }


        /// <summary>
        ///     Loads a new instance of the <see cref="VCard" /> class from a text file.
        /// </summary>
        /// <param name="path">
        ///     The path to a text file containing vCard data in any recognized vCard format.
        /// </param>
        public VCard(string path) : this()
        {
            using (var streamReader = new StreamReader(path))
            {
                var reader = new VCardReader();
                reader.ReadInto(this, streamReader);
            }
        }
        #endregion
    }
}
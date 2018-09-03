namespace GlitchedPolygons.Services.MailgunEmailSender
{
    /// <summary>
    /// An email attachment's details.
    /// </summary>
    public struct Attachment
    {
        /// <summary>
        /// The attachment's name.
        /// </summary>
        public string name;

        /// <summary>
        /// The attachment's file name.
        /// </summary>
        public string fileName;

        /// <summary>
        /// The actual attachment file's bytes.
        /// </summary>
        public byte[] file;

        /// <summary>
        /// The request's content type.
        /// Can be left null, unlike the other fields here.
        /// </summary>
        public string contentType;
    }
}

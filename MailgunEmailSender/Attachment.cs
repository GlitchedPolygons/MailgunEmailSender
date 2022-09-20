namespace GlitchedPolygons.Services.MailgunEmailSender
{
    /// <summary>
    /// An email attachment's details.
    /// </summary>
    public class Attachment
    {
        /// <summary>
        /// The attachment's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The attachment's file name.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// The actual attachment file's bytes.
        /// </summary>
        public byte[] File { get; set; }

        /// <summary>
        /// [OPTIONAL] The request's content type. <para> </para>
        /// Can be left <c>null</c>, unlike the other fields here.
        /// </summary>
        public string ContentType { get; set; }
    }
}

namespace GlitchedPolygons.Services.MailgunEmailSender
{
    /// <summary>
    /// The base url to use for the mailgun API.<para> </para>
    /// <see cref="US"/> = https://api.mailgun.net/v3 <para> </para>
    /// <see cref="EU"/> = https://api.eu.mailgun.net/v3 <para> </para>
    /// </summary>
    public enum MailgunApiBaseUrl : int
    {
        /// <summary>
        /// https://api.mailgun.net/v3 (default)
        /// </summary>
        US = 0,

        /// <summary>
        /// https://api.eu.mailgun.net/v3
        /// </summary>
        EU = 1
    }
}
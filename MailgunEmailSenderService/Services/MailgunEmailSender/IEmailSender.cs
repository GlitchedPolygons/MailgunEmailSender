using RestSharp;
using System.Threading.Tasks;

namespace GlitchedPolygons.Services.MailgunEmailSender
{
    /// <summary>
    /// Interface for sending emails asynchronosuly.
    /// </summary>
    public interface IEmailSender
    {
        /// <summary>
        /// Sends a plain-text-only email to a single recipient.<para> </para>
        /// Only use for testing or internal use: html+text variant is much more professional.<para> </para>
        /// The "from" parameter shall be set automatically by the implementing class.
        /// </summary>
        /// <param name="subject">The email's subject.</param>
        /// <param name="text">The email's text body.</param>
        /// <param name="to">The recipient's email address. Please ensure this is valid!</param>
        /// <returns>The <see cref="IRestResponse"/> that resulted from sending the email. Contains useful data like <see cref="IRestResponse.IsSuccessful"/>, <see cref="IRestResponse.ErrorMessage"/> in case of an error, etc...</returns>
        Task<IRestResponse> SendEmailAsync(string subject, string text, string to);

        /// <summary>
        /// Sends an email that contains both a text and html variant. This is the most common approach.<para> </para>
        /// NOTE: even though it is possible to add additional recipients (and CC) it is NOT recommended!<para> </para>
        /// Sending an email to multiple addresses allows all the recipients to see each others' full email addresses.<para> </para>
        /// For the sake of privacy it's recommended to send multiple mails out to single recipients instead (or use BCC, yeah...)
        /// </summary>
        /// <param name="from">The sender's email address. This can be a raw email address or in the format "Justin Sider &lt;justin.sider@domain.com&gt;".</param>
        /// <param name="to">The recipient email address.</param>
        /// <param name="subject">The mail's subject.</param>
        /// <param name="text">The text-only version of the mail's body (for the old, crippled browsers).</param>
        /// <param name="html">The html variant of the mail body. Can be omitted (null or empty), but it's HIGHLY recommended to always have both the html and text variants.</param>
        /// <param name="replyTo">A custom reply-to address. Can be omitted.</param>
        /// <param name="additionalRecipients">Any additional, directly addressed recipients. IMPORTANT: read the doc summary to find out why this is a very bad idea. Use for testing or internal use ONLY!!!</param>
        /// <param name="cc">Carbon copy list.</param>
        /// <param name="bcc">Blind carbon copy list.</param>
        /// <returns>The <see cref="IRestResponse"/> that resulted from sending the email. Contains useful data like <see cref="IRestResponse.IsSuccessful"/>, <see cref="IRestResponse.ErrorMessage"/> in case of an error, etc...</returns>
        Task<IRestResponse> SendEmailAsync(string from, string to, string subject, string text, string html, string replyTo = null, string[] additionalRecipients = null, string[] cc = null, string[] bcc = null);
    }
}

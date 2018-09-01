using System;
using System.Threading.Tasks;

using RestSharp;
using RestSharp.Authenticators;

namespace GlitchedPolygons.Services.MailgunEmailSender
{
    /// <summary>
    /// This class is used by the application
    /// to send email via Mailgun and RestSharp.
    /// </summary>
    public class EmailSender : IEmailSender
    {
        private readonly string domain;
        private readonly string defaultFrom;
        private readonly RestClient restClient;

        /// <summary>
        /// The Mailgun API base url.
        /// </summary>
        private const string MAILGUN_API_URL = "https://api.mailgun.net/v3";

        /// <summary>
        /// Creates an <see cref="EmailSender"/> instance using the specified Mailgun API key, domain and defaultFrom address.
        /// </summary>
        /// <param name="mailgunApiKey">Your Mailgun.com API key.</param>
        /// <param name="domain">Your mailgun-registered emailing domain (e.g. mail.yourdomain.com).</param>
        /// <param name="defaultFrom">The default sender's email address for when no "from" parameter is provided (e.g. "Justin Sider &lt;info@yourdomain.com&gt;").</param>
        public EmailSender(string mailgunApiKey, string domain, string defaultFrom)
        {
            if (string.IsNullOrEmpty(mailgunApiKey) || string.IsNullOrWhiteSpace(mailgunApiKey))
            {
                throw new ArgumentException($"{nameof(EmailSender)}::ctor: The passed {nameof(mailgunApiKey)} string is either null or empty!", mailgunApiKey);
            }

            if (string.IsNullOrEmpty(domain) || !domain.Contains("."))
            {
                throw new ArgumentException($"{nameof(EmailSender)}::ctor: The passed {nameof(domain)} string is either null, empty or invalid!", domain);
            }

            if (string.IsNullOrEmpty(defaultFrom) || !defaultFrom.Contains("@"))
            {
                throw new ArgumentException($"{nameof(EmailSender)}::ctor: The passed {nameof(defaultFrom)} string is either null, empty or not a valid email address!", defaultFrom);
            }

            this.domain = domain;
            this.defaultFrom = defaultFrom;

            restClient = new RestClient
            {
                BaseUrl = new Uri(MAILGUN_API_URL),
                Authenticator = new HttpBasicAuthenticator("api", mailgunApiKey)
            };
        }

        /// <summary>
        /// Sends a plain-text-only email to a single recipient.<para> </para>
        /// Only use for testing or internal use: the html+text variant is much more professional.<para> </para>
        /// </summary>
        /// <param name="subject">The email's subject.</param>
        /// <param name="text">The email's text body.</param>
        /// <param name="to">The recipient's email address. Please ensure this is valid!</param>
        /// <returns>The <see cref="IRestResponse"/> that resulted from sending the email. Contains useful data like <see cref="IRestResponse.IsSuccessful"/>, <see cref="IRestResponse.ErrorMessage"/> in case of an error, etc...</returns>
        public async Task<IRestResponse> SendEmailAsync(string subject, string text, string to)
        {
            if (string.IsNullOrEmpty(to) || !to.Contains("@"))
            {
                throw new ArgumentException($"{nameof(EmailSender)}::{nameof(SendEmailAsync)}: The '{nameof(to)}' email address argument is either null, empty or invalid. Please only send email to valid addresses.");
            }

            var request = new RestRequest
            {
                Method = Method.POST,
                Resource = "{domain}/messages"
            };

            request.AddParameter("domain", domain, ParameterType.UrlSegment);
            request.AddParameter("from", defaultFrom);
            request.AddParameter("to", to);
            request.AddParameter("subject", subject ?? string.Empty);
            request.AddParameter("text", text ?? string.Empty);

            return await restClient.ExecuteTaskAsync(request);
        }

        /// <summary>
        /// Sends an email that contains both a text and html variant. This is the most common approach.<para> </para>
        /// NOTE: even though it is possible to add additional recipients (and CC) it is NOT recommended!<para> </para>
        /// Sending an email to multiple addresses allows all of the recipients to see each others' full email addresses.<para> </para>
        /// For the sake of privacy it's recommended to send multiple mails out to single recipients instead (or use BCC, yeah...)
        /// </summary>
        /// <param name="from">The sender's email address. This can be a raw email address or in the format "Justin Sider &lt;justin.sider@yourdomain.com&gt;".</param>
        /// <param name="to">The recipient email address.</param>
        /// <param name="subject">The mail's subject.</param>
        /// <param name="text">The text-only version of the mail's body (for the old, crippled browsers).</param>
        /// <param name="html">The html variant of the mail body. Can be omitted (null or empty), but it's HIGHLY recommended to always have both the html and text variants.</param>
        /// <param name="replyTo">A custom reply-to address. Can be omitted.</param>
        /// <param name="additionalRecipients">Any additional, directly addressed recipients. IMPORTANT: read the doc summary to find out why this is a bad idea. Use for testing or internal use only!!!</param>
        /// <param name="cc">Carbon copy list.</param>
        /// <param name="bcc">Blind carbon copy list.</param>
        /// <returns>The <see cref="IRestResponse"/> that resulted from sending the email. Contains useful data like <see cref="IRestResponse.IsSuccessful"/>, <see cref="IRestResponse.ErrorMessage"/> in case of an error, etc...</returns>
        public async Task<IRestResponse> SendEmailAsync(string from, string to, string subject, string text, string html, string replyTo, string[] additionalRecipients = null, string[] cc = null, string[] bcc = null)
        {
            var request = new RestRequest
            {
                Method = Method.POST,
                Resource = "{domain}/messages"
            };

            request.AddParameter("domain", domain, ParameterType.UrlSegment);
            request.AddParameter("subject", subject ?? string.Empty);
            request.AddParameter("text", text ?? string.Empty);
            request.AddParameter("from", from);
            request.AddParameter("to", to);

            if (!string.IsNullOrEmpty(html))
            {
                request.AddParameter("html", html);
            }

            if (!string.IsNullOrEmpty(replyTo))
            {
                request.AddParameter("h:Reply-To", replyTo);
            }

            if (additionalRecipients != null && additionalRecipients.Length > 0)
            {
                for (int i = additionalRecipients.Length - 1; i >= 0; i--)
                {
                    request.AddParameter("to", additionalRecipients[i]);
                }
            }

            if (cc != null && cc.Length > 0)
            {
                for (int i = cc.Length - 1; i >= 0; i--)
                {
                    request.AddParameter("cc", cc[i]);
                }
            }

            if (bcc != null && bcc.Length > 0)
            {
                for (int i = bcc.Length - 1; i >= 0; i--)
                {
                    request.AddParameter("bcc", bcc[i]);
                }
            }

            return await restClient.ExecuteTaskAsync(request);
        }
    }
}

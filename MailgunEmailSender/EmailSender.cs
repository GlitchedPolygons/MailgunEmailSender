using System;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GlitchedPolygons.Services.MailgunEmailSender
{
    /// <summary>
    /// This class is used by the application
    /// to send email via Mailgun and RestSharp.
    /// </summary>
    public class EmailSender : IEmailSender, IDisposable
    {
        private readonly string endpoint;
        private readonly string defaultFrom;
        private readonly HttpClient httpClient;

        /// <summary>
        /// Creates an <see cref="EmailSender"/> instance using the specified Mailgun API key, domain and defaultFrom address.
        /// </summary>
        /// <param name="mailgunApiKey">Your Mailgun.com API key.</param>
        /// <param name="domain">Your mailgun-registered emailing domain (e.g. mail.yourdomain.com).</param>
        /// <param name="defaultFrom">The default sender's email address for when no "from" parameter is provided (e.g. "Justin Sider &lt;info@yourdomain.com&gt;").</param>
        /// <param name="baseUrl">The Mailgun API Base URL to use for all requests (US or EU).</param>
        /// <seealso cref="MailgunApiBaseUrl"/>
        public EmailSender(string mailgunApiKey, string domain, string defaultFrom, MailgunApiBaseUrl baseUrl = 0)
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

            this.defaultFrom = defaultFrom;
            this.endpoint = $"v3/{domain}/messages";

            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(baseUrl == 0 ? "https://api.mailgun.net" : "https://api.eu.mailgun.net");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"api:{mailgunApiKey}")));
        }

        /// <summary>
        /// Sends a plain-text-only email to a single recipient.<para> </para>
        /// Only use for testing or internal use: html+text variant is much more professional.<para> </para>
        /// The "from" parameter shall be set automatically by the implementing class.
        /// </summary>
        /// <param name="subject">The email's subject.</param>
        /// <param name="text">The email's text body.</param>
        /// <param name="to">The recipient's email address. Please ensure this is valid!</param>
        /// <returns>The <see cref="HttpResponseMessage"/> that resulted from sending the email. Contains useful data like <see cref="HttpResponseMessage.IsSuccessStatusCode"/>, <see cref="HttpResponseMessage.StatusCode"/>, etc...</returns>
        public Task<HttpResponseMessage> SendEmailAsync(string subject, string text, string to)
        {
            if (string.IsNullOrEmpty(to) || !to.Contains("@"))
            {
                throw new ArgumentException($"{nameof(EmailSender)}::{nameof(SendEmailAsync)}: The '{nameof(to)}' email address argument is either null, empty or invalid. Please only send email to valid addresses.");
            }

            return httpClient.PostAsync(endpoint, new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("from", defaultFrom),
                new KeyValuePair<string, string>("to", to),
                new KeyValuePair<string, string>("subject", subject ?? string.Empty),
                new KeyValuePair<string, string>("text", text ?? string.Empty)
            }));
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
        /// <param name="attachments">Any email attachments.</param>
        /// <returns>The <see cref="HttpResponseMessage"/> that resulted from sending the email. Contains useful data like <see cref="HttpResponseMessage.IsSuccessStatusCode"/>, <see cref="HttpResponseMessage.StatusCode"/>, etc...</returns>
        public Task<HttpResponseMessage> SendEmailAsync(string from, string to, string subject, string text, string html, string replyTo, string[] additionalRecipients = null, string[] cc = null, string[] bcc = null, IEnumerable<Attachment> attachments = null)
        {
            var httpContent = new MultipartFormDataContent();

            httpContent.Add(new StringContent(from), "from");
            httpContent.Add(new StringContent(to), "to");
            httpContent.Add(new StringContent(subject ?? string.Empty), "subject");
            httpContent.Add(new StringContent(text ?? string.Empty), "text");

            if (!string.IsNullOrEmpty(html))
            {
                httpContent.Add(new StringContent(html), "html");
            }

            if (!string.IsNullOrEmpty(replyTo))
            {
                httpContent.Add(new StringContent(replyTo), "h:Reply-To");
            }

            if (additionalRecipients != null && additionalRecipients.Length > 0)
            {
                for (int i = additionalRecipients.Length - 1; i >= 0; --i)
                {
                    httpContent.Add(new StringContent(additionalRecipients[i]), "to");
                }
            }

            if (cc != null && cc.Length > 0)
            {
                for (int i = cc.Length - 1; i >= 0; --i)
                {
                    httpContent.Add(new StringContent(cc[i]), "cc");
                }
            }

            if (bcc != null && bcc.Length > 0)
            {
                for (int i = bcc.Length - 1; i >= 0; --i)
                {
                    httpContent.Add(new StringContent(bcc[i]), "bcc");
                }
            }

            if (attachments != null)
            {
                foreach (var attachment in attachments)
                {
                    ByteArrayContent fileContent = new ByteArrayContent(attachment.File);

                    fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        Name = "attachment",
                        FileName = attachment.FileName,
                    };

                    if (!string.IsNullOrEmpty(attachment.ContentType) && MediaTypeHeaderValue.TryParse(attachment.ContentType, out MediaTypeHeaderValue parsedMediaType))
                    {
                        fileContent.Headers.ContentType = parsedMediaType;
                    }

                    httpContent.Add(fileContent);
                }
            }

            return httpClient.PostAsync(endpoint, httpContent);
        }

        /// <summary>
        /// Disposes this <see cref="EmailSender"/> and its underlying <see cref="HttpClient"/>.
        /// </summary>
        public void Dispose()
        {
            httpClient?.Dispose();
        }
    }
}
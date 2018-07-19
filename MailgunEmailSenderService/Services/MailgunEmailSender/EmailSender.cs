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

        private const string MAILGUN_API_URL = "https://api.mailgun.net/v3";

        public EmailSender(string mailgunApiKey, string domain, string defaultFrom)
        {
            if (string.IsNullOrEmpty(mailgunApiKey))
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

        public async Task<IRestResponse> SendEmailAsync(string subject, string text, string to)
        {
            var request = new RestRequest
            {
                Method = Method.POST,
                Resource = "{domain}/messages"
            };

            request.AddParameter("domain", domain, ParameterType.UrlSegment);
            request.AddParameter("from", defaultFrom);
            request.AddParameter("to", to);
            request.AddParameter("subject", subject);
            request.AddParameter("text", text);

            return await restClient.ExecuteTaskAsync(request);
        }

        public async Task<IRestResponse> SendEmailAsync(string from, string to, string subject, string text, string html, string replyTo, string[] additionalRecipients = null, string[] cc = null, string[] bcc = null)
        {
            var request = new RestRequest
            {
                Method = Method.POST,
                Resource = "{domain}/messages"
            };

            request.AddParameter("domain", domain, ParameterType.UrlSegment);
            request.AddParameter("subject", subject);
            request.AddParameter("text", text);
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

// Copyright (C) - Raphael Beck, 2018

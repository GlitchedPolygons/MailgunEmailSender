using System;
using Xunit;

namespace GlitchedPolygons.Services.MailgunEmailSender.Tests
{
    public class EmailSenderTests
    {
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Ctor_PassNullOrEmptyApiKey_ThrowArgumentException(string apiKey)
        {
            Assert.Throws<ArgumentException>(() =>
            {
                IEmailSender emailSender = new EmailSender(apiKey, "domain", "info@yourdomain.com");
            });
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Ctor_PassNullOrEmptyDomain_ThrowArgumentException(string domain)
        {
            Assert.Throws<ArgumentException>(() =>
            {
                IEmailSender emailSender = new EmailSender("api_key", domain, "info@yourdomain.com");
            });
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Ctor_PassNullOrEmptyDefaultFrom_ThrowArgumentException(string defaultFrom)
        {
            Assert.Throws<ArgumentException>(() =>
            {
                IEmailSender emailSender = new EmailSender("api_key", "domain", defaultFrom);
            });
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async void SendEmailAsync_PassNullOrEmptyTo_ThrowArgumentException(string to)
        {
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                IEmailSender emailSender = new EmailSender("api_key", "domain", "from@domain.com");
                await emailSender.SendEmailAsync("subject", "text", to);
            });
        }
    }
}

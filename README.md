[![NuGet](https://img.shields.io/nuget/v/GlitchedPolygons.Services.MailgunEmailSender.svg)](https://www.nuget.org/packages/GlitchedPolygons.Services.MailgunEmailSender) [![Build Status](https://travis-ci.org/GlitchedPolygons/MailgunEmailSender.svg?branch=master)](https://travis-ci.org/GlitchedPolygons/MailgunEmailSender)

# Mailgun Email Sender API Wrapper for ASP.NET Core apps

With this useful service you can easily send email through Mailgun. 
The Mailgun API Key (and other params) are set in the _EmailSender_'s class ctor. 

Intended usage is within ASP.NET Core apps using MVC; inject the service into the DI container 
(inside Startup.cs use `services.AddTransient` or `services.AddSingleton`) and then use it in your code.

Example:

`
services.AddSingleton<IEmailSender, EmailSender>(
                s => new EmailSender(
                    domain: "mail.yourdomain.com",
                    mailgunApiKey: "your_api_key_which_is_NOT_in_your_repo",
                    defaultFrom: "Info Team <info@yourdomain.com>"
                )
            );
` 

### Dependencies:

* [RestSharp (>= 106.3.1)](https://github.com/restsharp/RestSharp)

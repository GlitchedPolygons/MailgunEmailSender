[![CircleCI](https://circleci.com/gh/GlitchedPolygons/MailgunEmailSender.svg?style=shield)](https://circleci.com/gh/GlitchedPolygons/MailgunEmailSender)

# Mailgun Email Sender API Wrapper for ASP.NET Core apps

With this useful service you can easily send email through Mailgun. 
The Mailgun API Key (and other params) are set in the _EmailSender_'s class ctor. 

Intended usage is within ASP.NET Core apps using MVC; inject the service into the DI container 
(inside Startup.cs use `services.AddTransient` or `services.AddSingleton`) and then use it in your code.

### Dependencies:

* [RestSharp (>= 106.3.1)](https://github.com/restsharp/RestSharp)

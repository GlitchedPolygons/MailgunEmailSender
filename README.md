# Mailgun Email Sender API Wraper for ASP.NET Core apps

With this useful service you can send email through Mailgun. 
API Key and other params are set in the sender class ctor. 

Intended use is within ASP.NET Core apps using MVC; inject the service into the DI container 
(inside Startup.cs use services.AddTransient) and then use it in your code.

### Dependencies:

* RestSharp

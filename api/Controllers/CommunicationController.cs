using api.models.client;
using api.repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using Swashbuckle.AspNetCore.Annotations;

namespace api.Controllers;

[ApiController]
[Route("api/v1/communication")]
public class CommunicationController(IConfiguration configuration) : ControllerBase
{
    private IConfiguration _configuration { get; set; } = configuration;

    [HttpPost("sendEmail")]
    [SwaggerOperation(Summary = "Sends an email")]
    public async Task<bool> SendEmail()
    {
        var client = new SendGridClient(_configuration.GetSection("SendGrid.ApiKey").Value);
        var from = new EmailAddress("contact@parker-stovall.com", "#yournewstalker");
        var subject = "hey bb";
        var to = new EmailAddress("test");
        var plainTextContent = "text";
        var htmlContent = "html";
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        var resp = await client.SendEmailAsync(msg);
        return resp.IsSuccessStatusCode;
    }
}


// TODO: gather other addresses and add them to config
// TODO: Write basic email html
//txt.att.net

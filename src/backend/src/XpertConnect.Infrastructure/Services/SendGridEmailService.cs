using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using XpertConnect.Application.Common.Interfaces;

namespace XpertConnect.Infrastructure.Services;

/// <summary>
/// SendGrid email service implementation
/// </summary>
public class SendGridEmailService : IEmailService
{
    private readonly ILogger<SendGridEmailService> _logger;
    private readonly SendGridClient _client;
    private readonly string _fromEmail;
    private readonly string _fromName;

    public SendGridEmailService(IConfiguration configuration, ILogger<SendGridEmailService> logger)
    {
        _logger = logger;

        var apiKey = configuration["SendGrid:ApiKey"]
            ?? throw new InvalidOperationException("SendGrid:ApiKey not configured");

        _fromEmail = configuration["SendGrid:FromEmail"] ?? "noreply@xpertconnect.com";
        _fromName = configuration["SendGrid:FromName"] ?? "XpertConnect";

        _client = new SendGridClient(apiKey);
    }

    public async Task SendEmailAsync(
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var from = new EmailAddress(_fromEmail, _fromName);
            var toAddress = new EmailAddress(to);
            var msg = MailHelper.CreateSingleEmail(from, toAddress, subject, body, body);

            var response = await _client.SendEmailAsync(msg, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Email sent successfully to {To} with subject '{Subject}'", to, subject);
            }
            else
            {
                var responseBody = await response.Body.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Failed to send email to {To}. Status: {StatusCode}, Body: {Body}",
                    to, response.StatusCode, responseBody);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception sending email to {To}", to);
            throw;
        }
    }

    public async Task SendTemplateEmailAsync(
        string to,
        string templateId,
        object templateData,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var from = new EmailAddress(_fromEmail, _fromName);
            var toAddress = new EmailAddress(to);

            var msg = new SendGridMessage
            {
                From = from,
                TemplateId = templateId
            };
            msg.AddTo(toAddress);
            msg.SetTemplateData(templateData);

            var response = await _client.SendEmailAsync(msg, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Template email sent successfully to {To} using template {TemplateId}",
                    to, templateId);
            }
            else
            {
                var responseBody = await response.Body.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Failed to send template email to {To}. Status: {StatusCode}, Body: {Body}",
                    to, response.StatusCode, responseBody);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception sending template email to {To}", to);
            throw;
        }
    }
}

/// <summary>
/// Mock email service for development/testing
/// </summary>
public class MockEmailService : IEmailService
{
    private readonly ILogger<MockEmailService> _logger;

    public MockEmailService(ILogger<MockEmailService> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[MOCK EMAIL] To: {To}, Subject: {Subject}, Body: {Body}",
            to, subject, body.Length > 100 ? body[..100] + "..." : body);

        return Task.CompletedTask;
    }

    public Task SendTemplateEmailAsync(
        string to,
        string templateId,
        object templateData,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[MOCK EMAIL] To: {To}, TemplateId: {TemplateId}, Data: {Data}",
            to, templateId, System.Text.Json.JsonSerializer.Serialize(templateData));

        return Task.CompletedTask;
    }
}

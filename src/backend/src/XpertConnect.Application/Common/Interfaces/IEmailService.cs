namespace XpertConnect.Application.Common.Interfaces;

/// <summary>
/// Email service interface
/// </summary>
public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
    Task SendTemplateEmailAsync(string to, string templateId, object templateData, CancellationToken cancellationToken = default);
}

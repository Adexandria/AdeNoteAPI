﻿using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Services.Blob;
using AdeNote.Infrastructure.Services.EmailSettings;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Infrastructure.Utilities.EmailSettings;

namespace AdeNote.Infrastructure.Services.Notification
{
    /// <summary>
    /// Manages the email notification using template
    /// </summary>
    public class NotificationService : INotificationService
    {
        /// <summary>
        /// A constructor
        /// </summary>
        /// <param name="blobService">Handles the cloud storage</param>
        /// <param name="emailService">Handles email management</param>
        public NotificationService(IBlobService blobService, IEmailService emailService)
        {
            _blobService = blobService;
            _emailService = emailService;
        }
        /// <summary>
        /// Send notification based on template type
        /// </summary>
        /// <typeparam name="T">Email</typeparam>
        /// <param name="email">Hold email details</param>
        /// <param name="substitutions">User's details</param>
        /// <param name="template">Email template</param>
        /// <param name="contentType">Content type of message</param> 
        public void SendNotification<T>(T email, EmailTemplate template, ContentType contentType, Dictionary<string, string> substitutions) where T : Email
        {
            var contentTemplate = GenerateContentTemplate(template);

            if (string.IsNullOrEmpty(contentTemplate))
            {
                return;
            }

            var content = substitutions == null ? contentTemplate : CreatePersonalisedMessage(substitutions, contentTemplate);
            if (contentType == ContentType.html)
            {
                email.SetHtmlContent(content);
            }
            else
            {
                email.SetPlainContent(content);
            }

            _emailService.SendMessage(email);
        }

        public void SendNotification<T>(List<T> emails, EmailTemplate template, ContentType contentType, List<Dictionary<string,string>> substitutions) where T: Email
        {
            var contentTemplate = GenerateContentTemplate(template);

            if (string.IsNullOrEmpty(contentTemplate))
            {
                return;
            }

            for (int i = 0; i < emails.Count; i++)
            {
                var content = substitutions == null ? contentTemplate : CreatePersonalisedMessage(substitutions[i],contentTemplate);
                if (contentType == ContentType.html)
                {
                    emails[i].SetHtmlContent(content);
                }
                else
                {
                    emails[i].SetPlainContent(content);
                }
            }

            _emailService.SendMessages(emails);
        }

        /// <summary>
        /// Generates content template using template type
        /// </summary>
        /// <param name="template">template type</param>
        /// <returns>Content</returns>
        private string GenerateContentTemplate(EmailTemplate template)
        {
            var templateName = template.GetDescription();
            var contentTemplate = _blobService.DownloadImage(templateName).Result;
            return contentTemplate;
        }

        /// <summary>
        /// Generates Personalised message using content template
        /// </summary>
        /// <param name="substitutions">User details</param>
        /// <param name="contentTemplate">Content template</param>
        /// <returns>Personalised content</returns>
        private string CreatePersonalisedMessage(Dictionary<string, string> substitutions, string contentTemplate)
        {
            foreach (var substitution in substitutions)
            {
                contentTemplate = contentTemplate.Replace(substitution.Key, substitution.Value);
            }
            return contentTemplate;
        }
        /// <summary>
        ///Handles the cloud storage
        /// </summary>
        public IBlobService _blobService;
        /// <summary>
        /// Handles email management
        /// </summary>
        public IEmailService _emailService;
    }
}

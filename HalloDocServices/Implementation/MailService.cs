using HalloDocServices.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.Implementation
{
    public class MailService : IMailService
    {
        private readonly IConfiguration _configuration;
        public MailService(IConfiguration configuration) 
        { 
            _configuration = configuration;
        }

        public async Task<bool> SendMail(List<string> receiver, string subject, string body, bool isHtml, List<string>? filesToSend = null)
        {
            try
            {
                var mail = _configuration["Smtp:FromEmail"];
                var password = _configuration["Smtp:Password"];
                var port = _configuration["Smpt:Port"];
                var server = _configuration["Smtp:Server"];

                var client = new SmtpClient(server)
                {
                    Port = int.Parse(port ?? "587"),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(mail, password)
                };

                string senderDisplayName = "HalloDoc Platform";
                //string receiverDisplayName = receiver.First();

                //MailAddress senderMailAddress = new MailAddress(mail ?? "", senderDisplayName);
                //MailAddress receiverMailAddress = new MailAddress(receiver.First(), receiverDisplayName);
                MailAddressCollection receiverMailAddresses = new MailAddressCollection();
                foreach(var recipient in receiver)
                {
                    receiverMailAddresses.Add(new MailAddress(recipient));
                }

                using (var mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(mail ?? "", senderDisplayName);
                    foreach(var recipient in receiverMailAddresses)
                    {
                        mailMessage.To.Add(recipient);
                    }
                    mailMessage.Subject = subject;
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = isHtml;

                    // Validate file path and existence
                    if (filesToSend != null)
                    {
                        foreach (var file in filesToSend)
                        {
                            string FilePath = "wwwroot\\" + file;
                            string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);

                            if (!System.IO.File.Exists(path))
                            {
                                throw new ArgumentException("Invalid file path: " + path);
                            }

                            // Attach the file
                            var attachment = new Attachment(path);
                            mailMessage.Attachments.Add(attachment);
                        }
                    }

                    await client.SendMailAsync(mailMessage);
                }

                return true;
            }
            catch 
            {
                return false;
            }
        }
    }
}

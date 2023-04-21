using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mime;
using CopyRightPDF.Core.Models;

namespace CopyRightPDF.Core
{
    public class MailProvider
    {
        public MailProvider() { }

        public void SendApproveMail(string smtpServer,
                                    int port,
                                    string userName,
                                    string displayName,
                                    string password,
                                    string subject,
                                    string mailBody,
                                    LicenseModel license)
        {
            var smtpClient = new SmtpClient(smtpServer)
            {
                Port = port,
                Credentials = new NetworkCredential(userName, password),
                EnableSsl = true,
            };

            var html = System.IO.File.ReadAllText(mailBody);
            html = html.Replace("[[CustomerName]]", license.RegisteredCustomerName);
            html = html.Replace("[[PhoneNumber]]", license.RegisteredPhoneNumber);

            var mailMessage = new MailMessage
            {
                From = new MailAddress(userName, displayName),
                Subject = subject,
                Body = html,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(license.RegisteredEmail);

            smtpClient.Send(mailMessage);
        }
    }
}

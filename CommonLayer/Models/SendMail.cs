using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace CommonLayer.Models
{
    public class SendMail
    {

        public string SendEmail(string ToEmail,string Token)
        {
            string FromEmail = "vaishnavigkuber@gmail.com";
            MailMessage message = new MailMessage(FromEmail,ToEmail);
            string MailBody = "The token for the rest password"+Token;
            message.Subject = "Token generated for resetting password";
            message.Body = MailBody.ToString();
            message.BodyEncoding= Encoding.UTF8;
            message.IsBodyHtml= true;
                                                  // client        port number(.gmail.com can be replaced by different platforms like yahoo etc
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com",587);
            NetworkCredential credential = new NetworkCredential("vaishnavikuber2712@gmail.com", "mbhd kqtq rywg nrxb");

            smtpClient.EnableSsl= true;
            smtpClient.UseDefaultCredentials = true;
            smtpClient.Credentials= credential;

            smtpClient.Send(message);
            return ToEmail;
        }

    }
}

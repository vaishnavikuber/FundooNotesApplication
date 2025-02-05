using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace CommonLayer.Models
{
    public class SendCollaboratorMail
    {
        public string SendEmail(string ToEmail, int NotesId,string collaborator)
        {
            string FromEmail = "vaishnavigkuber@gmail.com";
            MailMessage message = new MailMessage(FromEmail, ToEmail);
            string MailBody = "Hi "+ToEmail+"  You are collaborated in notes with notesId: " + NotesId;
            message.Subject = "collaborated by person: "+collaborator;
            message.Body = MailBody.ToString();
            message.BodyEncoding = Encoding.UTF8;
            message.IsBodyHtml = true;
            // client        port number(.gmail.com can be replaced by different platforms like yahoo etc
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
            NetworkCredential credential = new NetworkCredential("vaishnavikuber2712@gmail.com", "mbhd kqtq rywg nrxb");

            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = true;
            smtpClient.Credentials = credential;

            smtpClient.Send(message);
            return ToEmail;
        }
    }
}

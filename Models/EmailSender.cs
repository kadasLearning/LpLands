using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.IO;

namespace LandsDepartment.Models
{
    public class EmailSender
    {
        public void SendEmail(string To, string Subject, string Body)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["EmailID"].ToString());
                mail.To.Add(To.Trim());
                mail.Subject = Subject.Trim();
                mail.Body = Body;
                mail.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                System.Net.NetworkCredential BasicAuthenticationInfo = new System.Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["EmailID"].ToString(), System.Configuration.ConfigurationManager.AppSettings["EmailPass"].ToString());
                smtp.Timeout = 600000;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = BasicAuthenticationInfo;
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void SendEmail(string From, string Subject, string Body,string To)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["EmailID"].ToString());
                mail.To.Add(To.Trim());
                mail.Subject = Subject.Trim();
                mail.Body = Body;
                mail.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                System.Net.NetworkCredential BasicAuthenticationInfo = new System.Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["EmailID"].ToString(), System.Configuration.ConfigurationManager.AppSettings["EmailPass"].ToString());
                smtp.Timeout = 600000;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = BasicAuthenticationInfo;
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SendEmail(string From, string To, string Subject, string Body, string fs)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["EmailID"].ToString());
                mail.To.Add(To.Trim());
                mail.Subject = Subject.Trim();
                mail.Attachments.Add(new Attachment(fs));
                mail.Body = Body;
                mail.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                System.Net.NetworkCredential BasicAuthenticationInfo = new System.Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["EmailID"].ToString(), System.Configuration.ConfigurationManager.AppSettings["EmailPass"].ToString());
                smtp.Timeout = 600000;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = BasicAuthenticationInfo;
                smtp.EnableSsl = true;
                smtp.Send(mail);
                mail.Attachments.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
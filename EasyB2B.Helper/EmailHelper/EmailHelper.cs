using EasyB2B.Helper.Email;
using EasyB2B.Models.Helper.EmailHelper;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace EasyB2B.Helper
{
    
    public class EmailHelper : IEmailHelper
    {
        #region Global Declaration
        private  MailMessage Message = null;
        private  SmtpClient smtpClient = null;
        #endregion

        #region Constrctor
        public EmailHelper(IOptions<EmailSetting> options)
        {
            smtpClient = new SmtpClient();
            smtpClient.Host = options.Value.SMTPHostName;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.EnableSsl = true;
            //smtpClient.EnableSsl = true;//comment if you don't need SSL 
            smtpClient.Credentials = new NetworkCredential(options.Value.UserName, options.Value.Password);
            smtpClient.Port = options.Value.Port;
            Message = new MailMessage();
        }
        #endregion

        #region public methods
        public async  Task SendEmail(EmailData email)
        {
            try
            {
                if (!string.IsNullOrEmpty(email.FromEmails))
                    AddFromAddress(email.FromEmails);

                if (!string.IsNullOrEmpty(email.ToEmails))
                    AddToAddress(email.ToEmails);

                if (!string.IsNullOrEmpty(email.CCEmails))
                    AddCcAddress(email.CCEmails);

                if (!string.IsNullOrEmpty(email.BCCEmails))
                    AddBccAddress(email.BCCEmails);

                if (email.EmailAttachments != null && email.EmailAttachments.Count > 0)
                {
                    for (int i = 0; i < email.EmailAttachments.Count; i++)
                    {
                        AddAttachment(email.EmailAttachments[i].FileStream, email.EmailAttachments[i].FileName, email.EmailAttachments[i].MIMEType);
                    }
                }
                Message.Subject = email.Subject;
                Message.IsBodyHtml = email.IsHTML;
                Message.Body = email.Body;
                await smtpClient.SendMailAsync(Message);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }
        #endregion
        

        #region Private Methods
        private  void AddFromAddress(string email)
        {
            Message.From = new MailAddress(email);
        }

        private  void AddToAddress(string email, string name = null)
        {
            if (!string.IsNullOrEmpty(email))
            {
                email = email.Replace(",", ";");
                string[] emailList = email.Split(';');
                for (int i = 0; i < emailList.Length; i++)
                {
                    if (!string.IsNullOrEmpty(emailList[i]))
                        Message.To.Add(new MailAddress(emailList[i], name));
                }
            }
        }
        private  void AddCcAddress(string email, string name = null)
        {
            if (!string.IsNullOrEmpty(email))
            {   
                email = email.Replace(",", ";");
                string[] emailList = email.Split(';');
                for (int i = 0; i < emailList.Length; i++)
                {
                    if (!string.IsNullOrEmpty(emailList[i]))
                        Message.CC.Add(new MailAddress(emailList[i], name));
                }
            }
        }
        private  void AddBccAddress(string email, string name = null)
        {
            if (!string.IsNullOrEmpty(email))
            {
                email = email.Replace(",", ";");
                string[] emailList = email.Split(';');
                for (int i = 0; i < emailList.Length; i++)
                {
                    if (!string.IsNullOrEmpty(emailList[i]))
                        Message.Bcc.Add(new MailAddress(emailList[i], name));
                }
            }
        }
        private  void AddAttachment(Stream stream, string fileName, string mimeType)
        {
            Attachment attachment = new Attachment(stream,fileName, mimeType);
            Message.Attachments.Add(attachment);
        }
        private  void AddAttachment(Attachment objAttachment)
        {
            Message.Attachments.Add(objAttachment);
        }

        private  string GetFileMimeType(string fileName)
        {
            string fileExt = Path.GetExtension(fileName.ToLower());
            string mimeType = string.Empty;
            switch (fileExt)
            {
                case ".htm":
                case ".html":
                    mimeType = "text/html";
                    break;
                case ".xml":
                    mimeType = "text/xml";
                    break;
                case ".jpg":
                case ".jpeg":
                    mimeType = "image/jpeg";
                    break;
                case ".gif":
                    mimeType = "image/gif";
                    break;
                case ".png":
                    mimeType = "image/png";
                    break;
                case ".bmp":
                    mimeType = "image/bmp";
                    break;
                case ".pdf":
                    mimeType = "application/pdf";
                    break;
                case ".doc":
                    mimeType = "application/msword";
                    break;
                case ".docx":
                    mimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    break;
                case ".xls":
                    mimeType = "application/x-msexcel";
                    break;
                case ".xlsx":
                    mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    break;
                case ".csv":
                    mimeType = "application/csv";
                    break;
                case ".ppt":
                    mimeType = "application/vnd.ms-powerpoint";
                    break;
                case ".pptx":
                    mimeType = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                    break;
                case ".rar":
                    mimeType = "application/x-rar-compressed";
                    break;
                case ".zip":
                    mimeType = "application/x-zip-compressed";
                    break;
                default:
                    mimeType = "text/plain";
                    break;
            }
            return mimeType;
        }

        #endregion
    }
}



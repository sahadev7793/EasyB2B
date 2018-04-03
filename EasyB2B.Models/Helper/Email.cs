using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EasyB2B.Models.Helper.EmailHelper
{
    #region Models
    public class EmailSetting
    {
        public string SMTPHostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
    }


    public class EmailData
    {
        public string FromEmails { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string ToEmails { get; set; }
        public string CCEmails { get; set; }
        public string BCCEmails { get; set; }
        public bool IsHTML { get; set; }
        public List<EmailAttachment> EmailAttachments { get; set; }
    }


    public class EmailAttachment
    {
        public Stream FileStream { get; set; }
        public string FileName { get; set; }
        public string MIMEType { get; set; }
    }

    #endregion

}

using EasyB2B.Models.Helper.EmailHelper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EasyB2B.Helper.Email
{
    public interface IEmailHelper
    {
        Task SendEmail(EmailData email);
    }
}   

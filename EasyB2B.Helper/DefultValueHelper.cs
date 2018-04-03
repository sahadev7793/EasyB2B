using System;
using System.Collections.Generic;
using System.Text;

namespace EasyB2B.Helper
{
    public class DefultValueHelper
    {
        public static readonly Guid DEFAULT_SYSTEMUSER = new Guid("A5C2D187-1A1F-274E-A100-08541C959350"); // system id

        public static readonly DateTime DEFAULT_OTP_EXPRIRE_TIME = DateTime.Now.ToUniversalTime().AddMinutes(5).ToUniversalTime();

        public static readonly DateTime DEFAULT_TOKEN_EXPRIRE_TIME = DateTime.Now.AddMinutes(30);

        public static readonly string DEFAULT_FROM_EMAIL = "sahadev.stridely@gmail.com";

    }
}

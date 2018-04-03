using System;
using System.Collections.Generic;
using System.Text;

namespace EasyB2B.Models.API
{
    public class AccessCode
    {
        public string EmailOrMobile { get; set; }
        public bool IsMobileNumber { get; set; }
    }

    public class CheckAccessCode : AccessCode
    {
        public string AccessCode { get; set; }
    }
}

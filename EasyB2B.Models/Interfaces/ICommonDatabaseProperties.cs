using System;
using System.Collections.Generic;
using System.Text;

namespace EasyB2B.Models.Interfaces
{
    internal interface ICommonDatabaseProperties
    {
         Guid CreatedBy { get; set; }
         DateTime CreatedOn { get; set; }
         Guid UpdatedBy { get; set; }
         DateTime UpdatedOn { get; set; }
    }
}

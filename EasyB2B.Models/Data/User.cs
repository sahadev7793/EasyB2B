using EasyB2B.Models.DataContext;
using EasyB2B.Models.Interfaces;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyB2B.Models.Data
{
    public class User : ICommonDatabaseProperties
    {
        [BsonId(IdGenerator = typeof(CustomIdGenerator))]
        public Guid Id { get; set; }
        public string  MobileNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool? IsActive { get; set; }
        public bool IsPhoneVarified { get; set; }
        public bool IsEmailVarified { get; set; }
        public Guid CreatedBy { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedOn { get ; set ; }
        public Guid UpdatedBy { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UpdatedOn { get; set; }
    }
}

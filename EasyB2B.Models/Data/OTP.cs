using EasyB2B.Models.Interfaces;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyB2B.Models.Data
{
    public class OTP : ICommonDatabaseProperties
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public bool IsUsed { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime ExpiryDateTime { get; set; }
        public string Type { get; set; }
        public Guid UserId { get; set; }
        public Guid CreatedBy { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedOn { get; set; }
        public Guid UpdatedBy { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UpdatedOn { get; set; }
    }
}

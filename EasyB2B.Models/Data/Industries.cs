using EasyB2B.Models.DataContext;
using EasyB2B.Models.Interfaces;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyB2B.Models
{
    public class Industries : ICommonDatabaseProperties
    {
        [BsonId(IdGenerator = typeof(CustomIdGenerator))]
        public Guid Id { get; set; }
        public string  Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public Guid UpdatedBy { get; set; }
    }
}

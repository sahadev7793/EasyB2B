using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyB2B.Models.DataContext
{
    public class CustomIdGenerator : IIdGenerator
    {
        public object GenerateId(object container, object document)
        {
            return Guid.NewGuid();
        }

        public bool IsEmpty(object id)
        {
            return id == null || (Guid)id == Guid.Empty;
        }
    }
}

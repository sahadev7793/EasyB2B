using System;
using System.Collections.Generic;
using System.Text;
using EasyB2B.Models.DataContext;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Threading.Tasks;
using EasyB2B.Models.Data;
using EasyB2B.Models;

namespace EasyB2B.DataAccess.DataContext
{
    public class EasyB2BContext
    {
        private readonly IMongoDatabase _database = null;

        public EasyB2BContext(IOptions<DbSettings> options)
        {
            var mongoClient = new MongoClient(options.Value.ConnectionString);
            if(mongoClient != null)
            {
                _database = mongoClient.GetDatabase(options.Value.Database);
            }
       }


        public IMongoCollection<User> Users
        {
            get
            {
                return _database.GetCollection<User>("Users");
            }

        }

        public IMongoCollection<Industries> Industries
        {
            get
            {
                return _database.GetCollection<Industries>("Industries");
            }

        }


        public IMongoCollection<OTP> OTPs        {
            get
            {
                return _database.GetCollection<OTP>("OTPs");
            }

        }

        public async Task CreateTablesAsync()
        {
            if(_database != null)
            {
              await  _database.CreateCollectionAsync("Users");
              await _database.CreateCollectionAsync("Industries");
            }
        }
    }
}

using EasyB2B.DataAccess.DataContext;
using EasyB2B.DataAccess.Interfaces;
using EasyB2B.Models.Data;
using EasyB2B.Models.DataContext;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EasyB2B.DataAccess.Providers
{
    public class UserProvider : IUserProvider
    {
        #region Global Declaration
        private readonly EasyB2BContext _context = null;
        #endregion

        #region Constructor
        public UserProvider(IOptions<DbSettings> settings)
        {
            _context = new EasyB2BContext(settings);
        }
        #endregion

        #region CRUD Methods
        public async Task AddAsync(User user)
        {
            try
            {
                user.CreatedOn = DateTime.Now.ToUniversalTime();
                user.UpdatedOn = DateTime.Now.ToUniversalTime();
                await _context.Users.InsertOneAsync(user);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            try
            {
                return await _context.Users.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            try
            {
                var filter = Builders<User>.Filter.Eq("Id", id);
                return await _context.Users.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public async Task<bool> RemoveAsync(Guid id)
        {
            try
            {
                DeleteResult deleteResult = await _context.Users.DeleteOneAsync(Builders<User>.Filter.Eq("Id", id));
                return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<bool> RemoveAllAsync()
        {
            try
            {
                DeleteResult deleteResult = await _context.Users.DeleteManyAsync(new BsonDocument());
                return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
            }
            catch (Exception)
            {

                throw;
            }
        }



        public async Task<bool> UpdateAsync(Guid id, User user)
        {

            try
            {
                var result = await GetByIdAsync(id);

                if (!string.IsNullOrEmpty(user.MobileNumber))
                    result.MobileNumber = user.MobileNumber;
                if (!string.IsNullOrEmpty(user.Email))
                    result.Email = user.Email;
                if (user.IsActive != null)
                    result.IsActive = user.IsActive;
                if (user.UpdatedBy != Guid.Empty)
                    result.UpdatedBy = user.UpdatedBy;

                result.UpdatedOn = DateTime.Now.ToUniversalTime();
                ReplaceOneResult replaceOneResult = await _context.Users.ReplaceOneAsync(n => n.Id.Equals(id), result, new UpdateOptions { IsUpsert = true });
                return replaceOneResult.IsAcknowledged && replaceOneResult.ModifiedCount > 0;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region OtherMethods
        public async Task<User> GetByEmailAsync(string  email)
        {
            try
            {
                var filter = Builders<User>.Filter.Eq("Email", email);
                return await _context.Users.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        public async Task<User> GetByMobileNumberAsync(string mobileNumber)
        {
            try
            {
                var filter = Builders<User>.Filter.Eq("MobileNumber", mobileNumber);
                return await _context.Users.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        #endregion

    }
}

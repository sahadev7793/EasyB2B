using EasyB2B.DataAccess.DataContext;
using EasyB2B.DataAccess.Interfaces;
using EasyB2B.Models.Data;
using EasyB2B.Models.DataContext;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyB2B.DataAccess.Providers
{
    public class OTPProvider : IOTPProvider
    {
        #region Global Declaration
        private readonly EasyB2BContext _context = null;
        #endregion

        #region Constructor
        public OTPProvider(IOptions<DbSettings> settings)
        {
            _context = new EasyB2BContext(settings);
        }
        #endregion


        #region CRUD Methods
        public async Task AddAsync(OTP otp)
        {
            try
            {
                otp.CreatedOn = DateTime.Now.ToUniversalTime();
                otp.UpdatedOn = DateTime.Now.ToUniversalTime();
                await _context.OTPs.InsertOneAsync(otp);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<IEnumerable<OTP>> GetAllAsync()
        {
            try
            {
                return await _context.OTPs.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<OTP> GetByIdAsync(Guid id)
        {
            try
            {
                var filter = Builders<OTP>.Filter.Eq("Id", id);
                return await _context.OTPs.Find(filter).FirstOrDefaultAsync();
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
                DeleteResult deleteResult = await _context.OTPs.DeleteManyAsync(new BsonDocument());
                return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> RemoveAsync(Guid id)
        {
            try
            {
                DeleteResult deleteResult = await _context.OTPs.DeleteOneAsync(Builders<OTP>.Filter.Eq("Id", id));
                return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<bool> UpdateAsync(Guid id, OTP otp)
        {
            try
            {
                ReplaceOneResult replaceOneResult = await _context.OTPs.ReplaceOneAsync(n => n.Id.Equals(id), otp, new UpdateOptions { IsUpsert = true });
                return replaceOneResult.IsAcknowledged && replaceOneResult.ModifiedCount > 0;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion


        #region Other Methods
        public async Task<OTP> GetOTPDetailByUserIdAndAccessCodeAsync(Guid userId, string code)
        {
            try
            {
                var builder = Builders<OTP>.Filter;
                var filter = builder.Eq("UserId", userId) & builder.Eq("Code",code);
                return await _context.OTPs.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        #endregion

    }
}

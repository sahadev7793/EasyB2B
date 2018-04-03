using EasyB2B.Models.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EasyB2B.DataAccess.Interfaces
{
    public interface IOTPProvider
    {
        Task<IEnumerable<OTP>> GetAllAsync();

        Task<OTP> GetByIdAsync(Guid id);

        Task AddAsync(OTP otp);

        Task<bool> RemoveAsync(Guid id);

        Task<bool> RemoveAllAsync();

        Task<bool> UpdateAsync(Guid id, OTP otp);

        Task<OTP> GetOTPDetailByUserIdAndAccessCodeAsync(Guid userId, string code);
    }
}

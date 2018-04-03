using EasyB2B.Models.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EasyB2B.DataAccess.Interfaces
{
    public interface IUserProvider
    {
        Task<IEnumerable<User>> GetAllAsync();

        Task<User> GetByIdAsync(Guid id);

        Task AddAsync(User user);

        Task<bool> RemoveAsync(Guid id);

        Task<bool> RemoveAllAsync ();

        Task<bool> UpdateAsync(Guid id, User user);

        Task<User> GetByEmailAsync(string email);

        Task<User> GetByMobileNumberAsync(string mobileNumber);
    }
}

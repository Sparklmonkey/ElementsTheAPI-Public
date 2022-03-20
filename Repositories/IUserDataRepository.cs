using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElementsTheAPI.Models;

namespace ElementsTheAPI.Repositories
{
    public interface IUserDataRepository
    {
        Task<AccountResponse> SaveUserData(AccountRequest accountRequest);
        Task<AccountResponse> ResetUserData(AccountRequest accountRequest);
        Task<AccountResponse> UpdateUserDetails(AccountRequest accountRequest);
        Task<string> RefreshToken(string token);
    }
}

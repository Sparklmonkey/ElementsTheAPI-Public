using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElementsTheAPI.Entities;
using ElementsTheAPI.Models;

namespace ElementsTheAPI.Repositories
{
    public interface ILoginRepository
    {
        Task<LoginResponse> RegisterUser(LoginRequest loginRequest);
        Task<LoginResponse> LoginUser(LoginRequest loginRequest);
        Task<UserData> GetById(string id);
    }
}

using Airline.Services.AuthAPI.Models.DTOs;
using static Airline.Services.AuthAPI.Responses.ServiceResponses;

namespace Airline.Services.AuthAPI.Services
{
    public interface IAccountService
    {
        Task<GeneralResponse> RegisterAccount(RegisterDTO users, bool isAdmin);
        Task<LoginResponse> LoginAccount(LoginDTO loginUser);
        Task <AccountResponse> GetAdmin(string id);
        Task<AccountResponse> GetUser(string id);
    }
}

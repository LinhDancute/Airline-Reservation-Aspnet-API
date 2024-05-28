using Airline_API.Models.DTOs;
using static Airline_API.Responses.ServiceResponses;

namespace Airline_API.Services
{
    public interface IAccountService
    {
        Task<GeneralResponse> RegisterAccount(RegisterDTO users, bool isAdmin);
        Task<LoginResponse> LoginAccount(LoginDTO loginUser);
        Task <AccountResponse> GetAdmin(string id);
        Task<AccountResponse> GetUser(string id);
    }
}

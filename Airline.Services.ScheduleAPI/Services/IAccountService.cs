using Airline.Services.ScheduleAPI.Models.DTOs;
using static Airline.Services.ScheduleAPI.Responses.ServiceResponses;

namespace Airline.Services.ScheduleAPI.Services
{
    public interface IAccountService
    {
        Task<LoginResponse> LoginAccount(LoginDTO loginUser);
    }
}

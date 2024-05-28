using Airline_API.Models.DTOs;

namespace Airline_API.Responses
{
    public class ServiceResponses
    {
        public record class GeneralResponse(bool flag, string Message);
        public record class LoginResponse(bool flag, string Token, string Message);
        public record class AccountResponse(bool flag, string Message, List<AccountDTO> AccountDTO); 
    }
}

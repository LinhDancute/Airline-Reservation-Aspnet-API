using Airline.Services.ScheduleAPI.Data;
using Microsoft.AspNetCore.Identity;
using Airline.Services.ScheduleAPI.Services;
using Airline.Services.ScheduleAPI.Models.DTOs;
using Airline.Services.ScheduleAPI.Responses;
using static Airline.Services.ScheduleAPI.Responses.ServiceResponses;
using Airline.Services.ScheduleAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Airline.Services.ScheduleAPI.Repositories
{
    public class AccountRepository : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        public AccountRepository(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<ServiceResponses.LoginResponse> LoginAccount(LoginDTO loginDTO)
        {
            if (loginDTO == null)
            {
                return new LoginResponse(false, null!, "Login container is empty");
            }

            var getUser = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (getUser == null)
            {
                return new LoginResponse(false, null!, "User not found");
            }

            bool checkUserPasswords = await _userManager.CheckPasswordAsync(getUser, loginDTO.Password);
            if (!checkUserPasswords)
            {
                return new LoginResponse(false, null!, "Invalid email/password");
            }

            var getUserRole = await _userManager.GetRolesAsync(getUser);
            var userSession = new AccountSession(getUser.Id, getUser.UserName, getUser.Email, getUserRole.First());
            string token = GenerateToken(userSession);

            return new LoginResponse(true, token!, "Login success");
        }

        private string GenerateToken(AccountSession userSession)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userSession.Id),
                new Claim(ClaimTypes.Name, userSession.UserName),
                new Claim(ClaimTypes.Email, userSession.Email),
                new Claim(ClaimTypes.Role, userSession.Role)
            };
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: userClaims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

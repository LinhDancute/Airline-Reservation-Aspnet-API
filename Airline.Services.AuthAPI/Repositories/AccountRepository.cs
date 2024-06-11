using Airline.Services.AuthAPI.Data;
using Microsoft.AspNetCore.Identity;
using Airline.Services.AuthAPI.Services;
using Airline.Services.AuthAPI.Models.DTOs;
using Airline.Services.AuthAPI.Responses;
using static Airline.Services.AuthAPI.Responses.ServiceResponses;
using Airline.Services.AuthAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Airline.Services.AuthAPI.Repositories
{
    public class AccountRepository : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        public AccountRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
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

        public async Task<GeneralResponse> RegisterAccount(RegisterDTO users, bool isAdmin = false)
        {
            if (users == null)
            {
                return new GeneralResponse(false, "Model is empty");
            }

            var newUser = new ApplicationUser
            {
                UserName = users.UserName,
                Email = users.Email,
            };

            var user = await _userManager.FindByEmailAsync(newUser.Email);
            if (user != null)
            {
                return new GeneralResponse(false, "User already registered");
            }

            var registerUser = await _userManager.CreateAsync(newUser, users.Password);
            if (!registerUser.Succeeded)
            {
                var errorMessage = string.Join(", ", registerUser.Errors.Select(e => e.Description));
                return new GeneralResponse(false, $"Error creating user: {errorMessage}");
            }

            // Role assignment
            var role = isAdmin ? "Admin" : "User";
            var roleExists = await _roleManager.RoleExistsAsync(role);
            if (!roleExists)
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole(role));
                if (!roleResult.Succeeded)
                {
                    var roleErrorMessage = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    return new GeneralResponse(false, $"Error creating role {role}: {roleErrorMessage}");
                }
            }

            var addToRoleResult = await _userManager.AddToRoleAsync(newUser, role);
            if (!addToRoleResult.Succeeded)
            {
                var addToRoleErrorMessage = string.Join(", ", addToRoleResult.Errors.Select(e => e.Description));
                return new GeneralResponse(false, $"Error adding user to role: {addToRoleErrorMessage}");
            }

            return new GeneralResponse(true, $"User registered as {role}");
        }

        public async Task<AccountResponse> GetUser(string id)
        {
            var users = new List<AccountDTO>();

            var aacounts = await _userManager.GetUsersInRoleAsync("User");
            foreach (var aacount in aacounts)
            {
                var roles = await _userManager.GetRolesAsync(aacount);
                var accountDTO = new AccountDTO
                {
                    Id = aacount.Id,
                    UserName = aacount.UserName,
                    Email = aacount.Email,
                    HomeAddress = aacount.HomeAddress,
                    BirthDate = aacount.BirthDate,
                    CMND = aacount.CMND,
                };

                users.Add(accountDTO);
            }

            if (users.Any())
            {
                return new AccountResponse(true, "users found.", users);
            }
            else
            {
                return new AccountResponse(false, "No users found.", null);
            }
        }

        public async Task<AccountResponse> GetAdmin(string id)
        {
            var admins = new List<AccountDTO>();

            var aacounts = await _userManager.GetUsersInRoleAsync("Admin");
            foreach (var aacount in aacounts)
            {
                var roles = await _userManager.GetRolesAsync(aacount);
                var accountDTO = new AccountDTO
                {
                    Id = aacount.Id,
                    UserName = aacount.UserName,
                    Email = aacount.Email,
                    HomeAddress = aacount.HomeAddress,
                    BirthDate = aacount.BirthDate,
                    CMND = aacount.CMND,
                };

                admins.Add(accountDTO);
            }

            if (admins.Any())
            {
                return new AccountResponse(true, "Admin users found.", admins);
            }
            else
            {
                return new AccountResponse(false, "No admin users found.", null);
            }
        }
    }
}

using Airline_API.Data;
using Azure.Identity;
using Microsoft.AspNetCore.Identity;
using Serilog;
using SharedClassLibrary.Contracts;
using SharedClassLibrary.Models;
using static SharedClassLibrary.Models.ServiceResponses;

namespace Airline_API.Responsitories
{
    public class UserResponsitory(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration) : IUserAccount
    {
        public async Task<ServiceResponses.LoginResponse> LoginUser(LoginUser loginUser)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponses.GeneralResponse> RegisterUser(Users users)
        {
            if (users == null)
            {
                Log.Error("Model is empty");
                return new GeneralResponse(false, "Model is empty");
            }

            var newUser = new ApplicationUser()
            {
                UserName = users.Name,
                NormalizedUserName = users.Name,
                Email = users.EmailAddress,
                NormalizedEmail = users.EmailAddress,
                EmailConfirmed = true,
                PasswordHash = users.Password,
                HomeAddress = users.HomeAddress,
                BirthDate = users.BirthDate,
                CMND = users.CMND,
            };

            var user = await userManager.FindByEmailAsync(newUser.Email);
            if (user is not null)
            {
                Log.Warning("User registered already");
                return new GeneralResponse(false, "User registered already");
            }

            var registerUser = await userManager.CreateAsync(newUser!, users.Password);
            if (!registerUser.Succeeded)
            {
                Log.Error("Error creating user: {Errors}", string.Join(", ", registerUser.Errors));
                return new GeneralResponse(false, "Error..try again");
            }

            var checkAdmin = await roleManager.FindByNameAsync("Admin");
            if (checkAdmin is null)
            {
                await roleManager.CreateAsync(new IdentityRole() { Name = "Admin" });
                await userManager.AddToRoleAsync(newUser, "Admin");
                Log.Information("User registered as Admin");
                return new GeneralResponse(true, "User registered as Admin");
            }
            else
            {
                var checkUser = await roleManager.FindByNameAsync("User");
                if (checkUser is null)
                {
                    await roleManager.CreateAsync(new IdentityRole() { Name = "User" });
                    await userManager.AddToRoleAsync(newUser, "User");
                    Log.Information("User registered as User");
                    return new GeneralResponse(true, "User registered as User");
                }
            }

            Log.Error("Error creating user: No role found");
            return new GeneralResponse(false, "Error..please try again");
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using SharedClassLibrary.Contracts;
using SharedClassLibrary.Models;

namespace Airline_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IUserAccount userAccount) : Controller
    {
        [HttpPost("Register")]
        public async Task<IActionResult> Register (Users users)
        {
            var response = await userAccount.RegisterUser (users);
            return Ok(response);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Airline.Services.ScheduleAPI.Services;
using Airline.Services.ScheduleAPI.Models.DTOs;

namespace Airline.Services.ScheduleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _userAccount;

        public AccountController(IAccountService userAccount)
        {
            _userAccount = userAccount;
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var response = await _userAccount.LoginAccount(loginDTO);
            if (!response.flag)
            {
                return BadRequest(response.Message);
            }
            return Ok(response);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airline.Services.AuthAPI.Models.DTOs
{
    public class AccountDTO
    {
        public string? Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? HomeAddress { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; }
        public string? CMND { get; set; } = string.Empty;
        public List<string> Roles { get; set; }
    }
}

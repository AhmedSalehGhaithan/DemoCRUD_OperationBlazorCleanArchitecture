using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLibrary.DTOs.Request.Account
{
    public class CreateAccountDTO :LoginDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required, Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string Roles { get; set; } = string.Empty;
    }
}

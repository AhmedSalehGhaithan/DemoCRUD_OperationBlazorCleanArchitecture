using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLibrary.DTOs.Request.Account
{
    public class LoginDTO
    {
        [EmailAddress, Required, DataType(DataType.EmailAddress)]
        [RegularExpression("[^@ \\t\\r\\n]+@[^@ \\t\\r\\n]+\\.[^@ \\t\\r\\n]+",
            ErrorMessage = "Your email is not valid , provide valid email such as ...@gmail, @hotmail,etc...")]
        [Display(Name ="Email Address")]
        public string EmailAddress { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$ %^&*-]).{8,}$",
            ErrorMessage = "Your Password must be a mix of Alphanumeric and special characters")]
        public string Password { get; set; } = string.Empty;
    }
}

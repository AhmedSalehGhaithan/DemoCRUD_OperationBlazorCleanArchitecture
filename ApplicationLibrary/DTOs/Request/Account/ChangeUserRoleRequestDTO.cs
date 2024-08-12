using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLibrary.DTOs.Request.Account
{
    public record ChangeUserRoleRequestDTO(string UserEmail,string RoleName);
}

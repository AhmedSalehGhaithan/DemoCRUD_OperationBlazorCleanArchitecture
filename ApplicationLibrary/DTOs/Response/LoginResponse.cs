using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLibrary.DTOs.Response
{
    public record LoginResponse(bool Flag = false, string Message = null!,string Token = null!,string RefreshToken = null!);
}

using ApplicationLibrary.DTOs.Request.Account;
using ApplicationLibrary.DTOs.Response.Account;
using ApplicationLibrary.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLibrary.Services
{
    public interface IAccountService
    {
        Task CreateAdmin();
        Task<GeneralResponse> CreateAccountAsync(CreateAccountDTO model);
        Task<LoginResponse> LoginAccountAsync(LoginDTO model);
        Task<GeneralResponse> CreateRoleAsync(CreateRoleDTO model);
        Task<LoginResponse> RefreshTokenAsync(RefreshTokenDTO model);
        Task<IEnumerable<GetRoleDTO>> GetRolesAsync();
        Task<IEnumerable<GetUsersWithRolesResponseDTO>> GetUsersWithRolesAsync();
        Task<GeneralResponse> ChanageUserRoleAsync(ChangeUserRoleRequestDTO model);
    }
}

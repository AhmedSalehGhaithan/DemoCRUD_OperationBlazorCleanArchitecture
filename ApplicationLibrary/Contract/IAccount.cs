
using ApplicationLibrary.DTOs.Request.Account;
using ApplicationLibrary.DTOs.Response;
using ApplicationLibrary.DTOs.Response.Account;

namespace ApplicationLibrary.Contract
{
    public interface IAccount
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

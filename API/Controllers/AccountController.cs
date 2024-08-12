using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ApplicationLibrary.Contract;
using ApplicationLibrary.DTOs.Response;
using ApplicationLibrary.DTOs.Request.Account;
using ApplicationLibrary.DTOs.Response.Account;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(IAccount account) : ControllerBase
    {
        [HttpPost("identity/create")]
        public async Task<ActionResult<GeneralResponse>> CreateAccount(CreateAccountDTO model)
        {
            if(!ModelState.IsValid)
                return BadRequest("Model can not be null");

            return Ok(await account.CreateAccountAsync(model));
        }

        [HttpPost("identity/Login")]
        public async Task<ActionResult<GeneralResponse>> LoginAccount(LoginDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Model can not be null");

            return Ok(await account.LoginAccountAsync(model));
        }

        [HttpPost("identity/refresh-token")]
        public async Task<ActionResult<GeneralResponse>> RefreshToken(RefreshTokenDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Model can not be null");

            return Ok(await account.RefreshTokenAsync(model));
        }

        [HttpPost("identity/role/create")]
        public async Task<ActionResult<GeneralResponse>> CreateRole(CreateRoleDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Model can not be null");

            return Ok(await account.CreateRoleAsync(model));
        }

        [HttpGet("identity/role/list")]
        public async Task<ActionResult<IEnumerable<GetRoleDTO>>> GetRoles()
             => Ok(await account.GetRolesAsync());

        [HttpPost("/setting")]
        public async Task<IActionResult> CreateAdmin()
        {
            await account.CreateAdmin();
            return Ok();
        }

        [HttpGet("identity/user-with-roles")]
        public async Task<ActionResult<IEnumerable<GetUsersWithRolesResponseDTO>>> GetUSerWithRoles()
            => Ok(await account.GetUsersWithRolesAsync());

        [HttpPost("identity/change-role")]
        public async Task<ActionResult<GeneralResponse>> ChangeUserRole(ChangeUserRoleRequestDTO model)
            => Ok(await account.ChanageUserRoleAsync(model));
    }
}

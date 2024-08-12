using Domain.Entity.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using ApplicationLibrary.Contract;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationLibrary.DTOs.Request.Account;
using ApplicationLibrary.DTOs.Response.Account;
using ApplicationLibrary.DTOs.Response;
using System;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Mapster;
using ApplicationLibrary.Extensions;
using Infrastructre.Data;
using Microsoft.EntityFrameworkCore;


namespace Infrastructre.Repoes
{
    public class AccountRepository(RoleManager<IdentityRole> _roleManager,
             UserManager<ApplicationUser> _userManager,AppDbContext _context,
             IConfiguration _config, SignInManager<ApplicationUser> _signInManager) : ApplicationLibrary.Contract.IAccount
    {
        private async Task<ApplicationUser> FindUserByEmailAsync(string email)
            => await _userManager.FindByEmailAsync(email);

        private async Task<IdentityRole> FindRoleByNameAsync(string name)
            => await _roleManager.FindByNameAsync(name);

        private static string GenerateRefreshToken() 
            => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        private async Task<string> GenerateToken(ApplicationUser user) 
        {
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:key"]!));
                //(SigningCredentials) This property is used to specify the algorithm and the secret key that will be used to 
                //sign the token
                var credentials = new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256);
                var roles = await _userManager.GetRolesAsync(user);
                var userClaims = new[]
                {
                    new Claim(ClaimTypes.Name,user.Name!),
                    new Claim(ClaimTypes.Email,user.Email!),
                    new Claim(ClaimTypes.Role,roles.FirstOrDefault()!.ToString()),
                    new Claim("FullName",user.Name!)
                };

                var token = new JwtSecurityToken(
                    issuer: _config["JWT:Issuer"],
                    audience: _config["JWT:Audience"],
                    claims:userClaims,
                    expires:DateTime.Now.AddMinutes(30),
                    signingCredentials:credentials
                    );
                return new JwtSecurityTokenHandler().WriteToken(token); 
            }
            catch
            {
                return null!;
            }
        }

        private async Task<GeneralResponse> AssignUserToRole(ApplicationUser user,IdentityRole role)

        {
            if (user is null || role is null) return new GeneralResponse(false, "Model state can not by empty");

            if (await FindRoleByNameAsync(role.Name!) == null)
                await CreateRoleAsync(role.Adapt(new CreateRoleDTO()));

            IdentityResult result = await _userManager.AddToRoleAsync(user, role.Name!);

            string error = CheckResponse(result);

            if (!string.IsNullOrEmpty(error))
                return new GeneralResponse(false, error);
            else
                return new GeneralResponse(true, $"{user.Name} assign to role to {role.Name}");
        }

        private static string CheckResponse(IdentityResult result)
        {
            if(!result.Succeeded)
            {
                var erros = result.Errors.Select(_e_ => _e_.Description);
                return string.Join(Environment.NewLine, erros);
            }
            return null!;
        }
        
        public async Task<GeneralResponse> ChanageUserRoleAsync(ChangeUserRoleRequestDTO model)
        {
            if (await FindRoleByNameAsync(model.RoleName) is null) return new GeneralResponse(false, "Role not found");
            if (await FindUserByEmailAsync(model.UserEmail) is null) return new GeneralResponse(false, "User not found");

            var user = await FindUserByEmailAsync(model.UserEmail);
            var previousRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            var removeOldRole = (await _userManager.RemoveFromRoleAsync(user, previousRole!));
            var errors = CheckResponse(removeOldRole);
            if(!string.IsNullOrEmpty(errors)) return new GeneralResponse(false, errors);

            var result = await _userManager.AddToRoleAsync(user,model.RoleName);
            var response = CheckResponse(result);
            if (!string.IsNullOrEmpty(response)) return new GeneralResponse(false, response);
            else return new GeneralResponse(true, "Role Changed");
        }

        public async Task<GeneralResponse> CreateAccountAsync(CreateAccountDTO model)
        {
            try
            {
                if (await FindUserByEmailAsync(model.EmailAddress) != null)
                    return new GeneralResponse(false, "Sorry, user already created");

                var user = new ApplicationUser()
                {
                    Name = model.Name,
                    UserName = model.EmailAddress,
                    PasswordHash = model.Password,
                    Email = model.EmailAddress
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                string errors= CheckResponse(result);
                if(!string.IsNullOrEmpty(errors)) return new GeneralResponse(false,errors);

                var (flag, message) = await AssignUserToRole(user, new IdentityRole() { Name = model.Roles });

                return new GeneralResponse(flag, message);
            }
            catch(Exception ex) 
            {
                return new GeneralResponse(false, ex.Message);
            }
        }

        public async Task CreateAdmin()
        {
            try
            {
                if ((await FindRoleByNameAsync(Constant.Role.Admin)) != null) return;

                var admin = new CreateAccountDTO()
                {
                    Name = "Admin",
                    Password = "Admin@123",
                    EmailAddress = "admin@admin.com",
                    Roles = Constant.Role.Admin
                };
                await CreateAccountAsync(admin);
            }

            catch { }
        }

        public async Task<IEnumerable<GetRoleDTO>> GetRolesAsync()
            => (await _roleManager.Roles.ToListAsync()).Adapt<IEnumerable<GetRoleDTO>>();

        public async Task<IEnumerable<GetUsersWithRolesResponseDTO>> GetUsersWithRolesAsync()
        {
            var allUsers = await _userManager.Users.ToListAsync();
            if (allUsers is null)
                return null!;

            var list = new List<GetUsersWithRolesResponseDTO>();
            foreach(var user in allUsers)
            {
                var getUserRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
                var getRoleInfo = await _roleManager.Roles.FirstOrDefaultAsync(_t_ => _t_.Name!.ToLower() == getUserRole!.ToLower());
                list.Add(new GetUsersWithRolesResponseDTO()
                {
                    Name = user.Name,
                    Email = user.Email,
                    RoleId = getRoleInfo!.Id,
                    RoleName = getRoleInfo.Name
                });
            }
            return list;
        }

        public async Task<LoginResponse> LoginAccountAsync(LoginDTO model)
        {
            try
            { 
                var user = await _userManager.FindByEmailAsync(model.EmailAddress);
                if (user is null)
                    return new LoginResponse(false, "User not found");

                SignInResult result;
                try
                {
                    // Parameters:
                    //   user:
                    //     The user to sign in.
                    //
                    //   password:
                    //     The password to attempt to sign in with.
                    //
                    //   lockoutOnFailure:
                    //     Flag indicating if the user account should be locked if the sign in fails.
                    result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                }
                catch
                {
                    return new LoginResponse(false, "Invalid credential");
                }

                if (!result.Succeeded)
                    return new LoginResponse(false, "Invalid credential");

                var jwtToken = await GenerateToken(user);
                string refreshToken = GenerateRefreshToken();

                if (string.IsNullOrEmpty(jwtToken) || string.IsNullOrEmpty(refreshToken))
                    return new LoginResponse(false, "Error occurred while logging in account ,please contact administration");
                else
                {
                    var saveResult = await SaveRefreshToken(user.Id, refreshToken);
                    if (saveResult.Flag)
                        return new LoginResponse(true, $"{user.Name} successfully logged in", jwtToken, refreshToken);
                    else
                        return new LoginResponse();
                }
            }
            catch(Exception ex)
            {
                return new LoginResponse(false, ex.Message);
            }
        }

        public async Task<GeneralResponse> CreateRoleAsync(CreateRoleDTO model)
        {
            try
            {
                if((await FindRoleByNameAsync(model.Name!)) == null)
                {
                    var response = await _roleManager.CreateAsync(new IdentityRole(model.Name!));
                    var error = CheckResponse(response);

                    if (!string.IsNullOrEmpty(error)) 
                        throw new Exception(error);
                    else 
                        return new GeneralResponse(true, $"{model.Name} created");
                }
                return new GeneralResponse(false, $"{model.Name} already created");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<LoginResponse> RefreshTokenAsync(RefreshTokenDTO model)
        {
            var Token = await _context.RefreshTokens.FirstOrDefaultAsync(_t_ => _t_.Token == model.Token);
            if (Token is null) return new LoginResponse();

            var user = await _userManager.FindByIdAsync(Token.UserId!);
            string newToken = await GenerateToken(user!);
            string newRefreshToken = GenerateRefreshToken();
            var saveResult = await SaveRefreshToken(user!.Id, newRefreshToken);

            if (saveResult.Flag)
                return new LoginResponse(true, $"{user.Name} successfully re-logged in", newToken, newRefreshToken);
            else
                return new LoginResponse();
        }

        private async Task<GeneralResponse> SaveRefreshToken(string userId,string token)
        {
            try
            {
                var user = await _context.RefreshTokens.FirstOrDefaultAsync(_t_ => _t_.UserId == userId);
                if (user is null)
                    _context.RefreshTokens.Add(new RefreshToken() { UserId = userId, Token = token });
                else
                    user.Token = token;

                await _context.SaveChangesAsync();
                return new GeneralResponse(true, null!);
            }
            catch(Exception ex)
            {
                return new GeneralResponse(false, ex.Message);
            }
        }
    }
}

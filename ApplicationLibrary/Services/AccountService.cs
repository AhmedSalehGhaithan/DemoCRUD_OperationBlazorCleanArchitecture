using ApplicationLibrary.DTOs.Request.Account;
using ApplicationLibrary.DTOs.Response;
using ApplicationLibrary.DTOs.Response.Account;
using ApplicationLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLibrary.Services
{
    public class AccountService(HttpClientService _httpClientService) : IAccountService
    {
        public async Task<LoginResponse> LoginAccountAsync(LoginDTO model)
        {
            try
            {
                var publicClient = _httpClientService.GetPublicClient();
                var response = await publicClient.PostAsJsonAsync(Constant.LoginRoute, model);
                string error = CheckResponseStatus(response);
                if (!string.IsNullOrEmpty(error))
                    return new LoginResponse(Flag: false, Message: error);

                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                return result!;
            }
            catch (Exception ex) { return new LoginResponse(Flag: false, Message: ex.Message); }
        }

        public async Task<GeneralResponse> CreateAccountAsync(CreateAccountDTO model)
        {
            try
            {
                var publicClient = _httpClientService.GetPublicClient();
                var response = await publicClient.PostAsJsonAsync(Constant.RegisterRoute, model);
                string error = CheckResponseStatus(response);
                if (!string.IsNullOrEmpty(error))
                    return new GeneralResponse(Flag: false, Message: error);

                var result = await response.Content.ReadFromJsonAsync<GeneralResponse>();
                return result!;

            }
            catch (Exception ex) { return new GeneralResponse(Flag: false, Message: ex.Message); }
        }

        public async Task CreateAdmin()
        {
            try
            {
                var client = _httpClientService.GetPublicClient();
                await client.PostAsync(Constant.CreateAdminRoute, null);
            }
            catch { }
        }

        public async Task<IEnumerable<GetRoleDTO>> GetRolesAsync()
        {
            try
            {
                var privateClient = await _httpClientService.GetPrivateClient();
                var response = await privateClient.GetAsync(Constant.GetRolesRoute);
                string error = CheckResponseStatus(response);
                if (!string.IsNullOrEmpty(error))
                    throw new Exception(error);

                var result = await response.Content.ReadFromJsonAsync<IEnumerable<GetRoleDTO>>();
                return result!;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
        
        //public IEnumerable<GetRoleDTO> GetDefaultRoles()
        //{
        //    var list = new List<GetRoleDTO>();
        //    list?.Clear();
        //    list?.Add(new GetRoleDTO(1.ToString(),Constant.Role.Admin));
        //    list?.Add(new GetRoleDTO(2.ToString(),Constant.Role.User));

        //    return list!;
        //}

        public async Task<IEnumerable<GetUsersWithRolesResponseDTO>> GetUsersWithRolesAsync()
        {
            try
            {
                var privateClient = await _httpClientService.GetPrivateClient();
                var response = await privateClient.GetAsync(Constant.GetUserWithRoleRoute);
                string error = CheckResponseStatus(response);
                if (!string.IsNullOrEmpty(error))
                    throw new Exception(error);

                var result = await response.Content.ReadFromJsonAsync<IEnumerable<GetUsersWithRolesResponseDTO>>();
                return result!;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
        
        public async Task<GeneralResponse> ChanageUserRoleAsync(ChangeUserRoleRequestDTO model)
        {
            try
            {
                var publicClient = _httpClientService.GetPublicClient();
                var response = await publicClient.PostAsJsonAsync(Constant.ChangeUserRoleRoute, model);
                string error = CheckResponseStatus(response);
                if (!string.IsNullOrEmpty(error))
                    return new GeneralResponse(false,error);

                var result = await response.Content.ReadFromJsonAsync<GeneralResponse>();
                return result!;
            }
            catch (Exception ex) { return new GeneralResponse(false,ex.Message); }
        }

        public async Task<GeneralResponse> CreateRoleAsync(CreateRoleDTO model)
        {
            throw new NotImplementedException();
        }
        
        public async Task<LoginResponse> RefreshTokenAsync(RefreshTokenDTO model)
        {
            try
            {
                var publicClient = _httpClientService.GetPublicClient();
                var response = await publicClient.PostAsJsonAsync(Constant.RefreshTokenRoute, model);
                string error = CheckResponseStatus(response);
                if (!string.IsNullOrEmpty(error))
                    return new LoginResponse(false, error);

                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                return result!;
            }
            catch (Exception ex) { return new LoginResponse(false, ex.Message); }
        }

        private static string CheckResponseStatus(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
                return $"Sorry unknown error occurred." +
                    $"{Environment.NewLine}Error Description:" +
                    $"{Environment.NewLine}Status Code:{response.StatusCode}" +
                    $"{Environment.NewLine}Response phrase:{response.ReasonPhrase}";
            else
                return null!;
        }
    }
}

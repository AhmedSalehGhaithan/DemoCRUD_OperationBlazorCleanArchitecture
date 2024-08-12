using Microsoft.IdentityModel.Tokens;
using NetcodeHub.Packages.Extensions.LocalStorage;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLibrary.Extensions
{
    public class HttpClientService(IHttpClientFactory httpClientFactory,LocalStorageService localStorageService)
    {
        private HttpClient CreateClient() => httpClientFactory.CreateClient(Constant.HttpClientName);
        public HttpClient GetPublicClient() => CreateClient();

        public async Task<HttpClient> GetPrivateClient()
        {
            try
            {
                var client = CreateClient();
                var localStorageDTO = await localStorageService.GetModelFromToken();
                if(string.IsNullOrEmpty(localStorageDTO.Token))
                    return client;

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constant.HttpClientHeaderScheme, localStorageDTO.Token);
                return client;

            }
            catch { return new HttpClient(); }
        }

    }
}

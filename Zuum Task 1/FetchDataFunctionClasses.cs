using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace Zuum_Task_1
{

    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient) 
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse> FetchDataAsync()
        {
            var responce = new ApiResponse();

            try
            {
                var result = await _httpClient.GetStringAsync("https://api.publicapis.org/random?auth=null");
                responce.IsSuccess = true;
                responce.Payload = result;
            }
            catch (Exception ex) 
            { 
                responce.IsSuccess = false;
                responce.ErrorMessage = ex.Message;
            }

            return responce;
        }
    }
}

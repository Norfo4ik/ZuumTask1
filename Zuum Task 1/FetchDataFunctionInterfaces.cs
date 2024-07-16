using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zuum_Task_1
{
    interface IApiService
    {
        Task<ApiResponse> FetchDataAsync();
    }

    public class ApiResponse
    {
        public bool IsSuccess { get; set; }
        public string Payload { get; set; }
        public string ErrorMessage { get; set; }

    }

    interface ILoggingService
    {
        Task LogSuccessAsync(ApiResponse response);
        Task LogFailureAsynk(string errorMessage);
        Task <IEnumerable<LogEntry>> GetLogsAsync(string from,  string to);
    }

    public class LogEntry
    {
        public string Id { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
    }

    interface IBlobStorageService
    {
        Task StorePayloadAsync(string payload);
        Task<string> GetPayloadAsync(string logId);
    }


}

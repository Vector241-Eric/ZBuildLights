using System;

namespace ZBuildLights.Core.Models.Requests
{
    public class NetworkResponse<T>
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public Exception Exception { get; set; }
    }

    public static class NetworkResponse
    {
        public static NetworkResponse<T> Success<T>(T entity)
        {
            return new NetworkResponse<T> {IsSuccessful = true, Data = entity};
        }

        public static NetworkResponse<T> Fail<T>(string message, Exception exception = null)
        {
            return new NetworkResponse<T> {IsSuccessful = false, Message = message, Exception = exception};
        }
    }
}
using System;

namespace ZBuildLights.Core.Models.Requests
{
    public class NetworkRequest<T>
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public Exception Exception { get; set; }
    }

    public static class NetworkRequest
    {
        public static NetworkRequest<T> Success<T>(T entity)
        {
            return new NetworkRequest<T> {IsSuccessful = true, Data = entity};
        }

        public static NetworkRequest<T> Fail<T>(string message, Exception exception = null)
        {
            return new NetworkRequest<T> {IsSuccessful = false, Message = message, Exception = exception};
        }
    }
}
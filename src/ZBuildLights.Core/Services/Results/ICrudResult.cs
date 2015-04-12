namespace ZBuildLights.Core.Services.Results
{
    public interface ICrudResult<T>
    {
        bool IsSuccessful { get; set; }
        string Message { get; set; }
        T Entity { get; set; }
    }
}
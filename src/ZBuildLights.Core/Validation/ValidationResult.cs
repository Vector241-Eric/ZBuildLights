namespace ZBuildLights.Core.Validation
{
    public class CreationResult<T>
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public T Entity { get; set; }
    }

    public static class CreationResult
    {
        public static CreationResult<T> Success<T>(T entity)
        {
            return new CreationResult<T> {IsSuccessful = true, Entity = entity};
        }

        public static CreationResult<T> Fail<T>(string message)
        {
            return new CreationResult<T> {IsSuccessful = false, Message = message};
        }
    }
}
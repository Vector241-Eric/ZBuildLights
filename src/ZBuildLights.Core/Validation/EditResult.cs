namespace ZBuildLights.Core.Validation
{
    public class EditResult<T>
    {
        public bool WasSuccessful { get; set; }
        public string Message { get; set; }
        public T Entity { get; set; }
    }

    public static class EditResult
    {
        public static EditResult<T> Success<T>(T entity)
        {
            return new EditResult<T> {WasSuccessful = true, Entity = entity};
        }

        public static EditResult<T> Fail<T>(string message)
        {
            return new EditResult<T> {WasSuccessful = false, Message = message};
        }
    }
}
namespace ZBuildLights.Core.Services.Results
{
    public class ZWaveOperationResult
    {
        private ZWaveOperationResult()
        {
        }

        public bool IsSuccessful { get; set; }
        public string Message { get; set; }

        public static ZWaveOperationResult Success
        {
            get { return new ZWaveOperationResult {IsSuccessful = true}; }
        }

        public static ZWaveOperationResult Fail(string message)
        {
            return new ZWaveOperationResult {IsSuccessful = false, Message = message};
        }
    }
}
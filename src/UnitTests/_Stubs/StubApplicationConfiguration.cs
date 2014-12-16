using ZBuildLights.Core.Configuration;

namespace UnitTests._Stubs
{
    public class StubApplicationConfiguration : IApplicationConfiguration
    {
        public string StorageFilePath { get; set; }
    }
}
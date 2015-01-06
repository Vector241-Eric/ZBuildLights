namespace ZBuildLights.Core.CruiseControl
{
    public interface ICcReader
    {
        Projects GetStatus(string url);
    }
}
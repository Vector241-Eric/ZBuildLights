namespace ZBuildLights.Core.Models
{
    public interface IHasZWaveIdentity
    {
        ZWaveValueIdentity ZWaveIdentity { get; }
    }
}
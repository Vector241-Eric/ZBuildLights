namespace ZBuildLights.Core.Models
{
    public class SystemStatusModel {
        public MasterModel MasterModel { get; set; }
        public LightGroup UnassignedLights { get; set; }
    }
}
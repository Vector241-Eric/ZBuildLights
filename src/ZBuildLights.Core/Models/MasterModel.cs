using System;

namespace ZBuildLights.Core.Models
{
    public class MasterModel
    {
        public Project[] Projects { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }
}
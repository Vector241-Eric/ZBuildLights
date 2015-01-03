using System.Collections.Generic;
using ZBuildLights.Core.Models;

namespace ZBuildLights.Core.Services
{
    public interface ILightStatusSetter
    {
        void SetLightStatus(IEnumerable<Light> lights);
    }
}
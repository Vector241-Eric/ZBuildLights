using System.Collections.Generic;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Services;

namespace UnitTests._Stubs
{
    public class StubLightStatusSetter : ILightStatusSetter
    {
        private List<Light> _lights = new List<Light>();
        private SwitchState _stubState;

        public StubLightStatusSetter StubStatus(SwitchState stubState)
        {
            _stubState = stubState;
            return this;
        }

        public void SetLightStatus(IEnumerable<Light> lights)
        {
            foreach (var light in lights)
            {
                _lights.Add(light);
                light.SwitchState = _stubState;
            }
        }

        public Light[] LightsThatHadStatusSet { get { return _lights.ToArray(); } }
    }
}
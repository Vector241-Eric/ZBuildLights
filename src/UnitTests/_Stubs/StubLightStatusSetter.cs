using System.Collections.Generic;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Services;

namespace UnitTests._Stubs
{
    public class StubLightStatusSetter : ILightStatusSetter
    {
        private readonly List<Light> _lights = new List<Light>();
        private SwitchState _defaultState;

        private readonly Dictionary<string, SwitchState> _stubStates = new Dictionary<string, SwitchState>();

        public StubLightStatusSetter DefaultStatus(SwitchState stubState)
        {
            _defaultState = stubState;
            return this;
        }

        public void SetLightStatus(IEnumerable<Light> lights)
        {
            foreach (var light in lights)
            {
                _lights.Add(light);
                var key = MakeKey(light.ZWaveIdentity);
                if (_stubStates.ContainsKey(key))
                    light.SwitchState = _stubStates[key];
                else
                    light.SwitchState = _defaultState;
            }
        }

        public Light[] LightsThatHadStatusSet
        {
            get { return _lights.ToArray(); }
        }

        public StubLightStatusSetter StubStatus(ZWaveIdentity identity, SwitchState switchState)
        {
            var key = MakeKey(identity);
            _stubStates[key] = switchState;
            return this;
        }

        private static string MakeKey(ZWaveIdentity identity)
        {
            return identity.ToString();
        }
    }
}
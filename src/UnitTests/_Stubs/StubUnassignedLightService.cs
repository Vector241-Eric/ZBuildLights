using System;
using System.Collections.Generic;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Services;

namespace UnitTests._Stubs
{
    public class StubUnassignedLightService : IUnassignedLightService
    {
        private bool _initialized;
        private readonly List<Light> _stubbed = new List<Light>();

        public void SetUnassignedLights(MasterModel masterModel)
        {
            if (!_initialized)
                throw new InvalidOperationException("The stub is not initialized.");
            masterModel.AddUnassignedLights(_stubbed);
        }

        public StubUnassignedLightService WithZeroUnassignedLights()
        {
            _initialized = true;
            return this;
        }

        public StubUnassignedLightService WithUnassignedLight(Light light)
        {
            _initialized = true;
            _stubbed.Add(light);
            return this;
        }
    }
}
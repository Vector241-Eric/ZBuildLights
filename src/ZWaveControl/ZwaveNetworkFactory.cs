using System;
using System.IO.Ports;
using System.Linq;
using ZBuildLights.Core.Services;

namespace ZWaveControl
{
    public class ZwaveNetworkFactory
    {
        private readonly IZWaveSettings _zWaveSettings;

        public ZwaveNetworkFactory(IZWaveSettings zWaveSettings)
        {
            _zWaveSettings = zWaveSettings;
        }

        public IZWaveNetwork GetNetwork()
        {
            var configuredComPort = _zWaveSettings.ControllerComPort;
            if (SerialPort.GetPortNames().Any(x => String.Compare(x, configuredComPort, StringComparison.OrdinalIgnoreCase) == 0))
                return new ZWaveNetwork();
            return new EmptyNetwork();
        }
    }
}
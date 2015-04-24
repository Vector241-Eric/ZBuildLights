using System;
using System.IO.Ports;
using System.Linq;
using NLog;
using ZBuildLights.Core.Services;

namespace ZWaveControl
{
    public class ZwaveNetworkFactory
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly IZWaveSettings _zWaveSettings;

        public ZwaveNetworkFactory(IZWaveSettings zWaveSettings)
        {
            _zWaveSettings = zWaveSettings;
        }

        public IZWaveNetwork GetNetwork()
        {
            var configuredComPort = _zWaveSettings.ControllerComPort;
            var portNames = SerialPort.GetPortNames();
            if (
                portNames
                    .Any(x => String.Compare(x, configuredComPort, StringComparison.OrdinalIgnoreCase) == 0))
                return new ZWaveNetwork();

            Log.Error(
                "Could not find any COM devices at {0}. Check to make sure the setting '{1}' matches the Z-Wave controller's COM port.",
                configuredComPort,
                ZWaveSettings.ZWaveComPortSettingKey);
            return new EmptyNetwork();
        }
    }
}
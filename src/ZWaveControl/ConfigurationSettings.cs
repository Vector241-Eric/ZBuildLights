using System.Configuration;

namespace BuildLightControl.ZWave
{
    public class ConfigurationSettings
    {
        public string ZWaveConfigurationPath
        {
            get { return ConfigurationManager.AppSettings["ConfigurationPath"]; }
        }
        public string ZWavePort
        {
            get { return @"\\.\COM" + ConfigurationManager.AppSettings["ControllerPortNumber"]; }
        }
    }
}
using System.Configuration;

namespace ZWaveControl
{
    public class ZWaveSettings
    {
        public string ConfigurationPath
        {
            get { return GetRequiredSetting("ZWaveConfigurationPath"); }
        }

        public string ControllerPortNumber
        {
            get { return @"\\.\COM" + GetRequiredSetting("ZWaveControllerPortNumber"); }
        }

        private string GetRequiredSetting(string key)
        {
            var value = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrEmpty(value))
                throw new ConfigurationErrorsException(string.Format("Configuration [{0}] is required.", key));
            return value;
        }
    }
}
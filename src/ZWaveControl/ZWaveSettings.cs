using System.Configuration;

namespace ZWaveControl
{
    public class ZWaveSettings
    {
        public string LogDirectory
        {
            get { return ConfigurationManager.AppSettings["LogDirectory"]; }
        }

        public string ConfigurationPath
        {
            get { return ConfigurationManager.AppSettings["ConfigurationPath"]; }
        }

        public string ControllerPortNumber
        {
            get { return @"\\.\COM" + ConfigurationManager.AppSettings["ControllerPortNumber"]; }
        }
    }
}
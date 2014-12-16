using System.Configuration;

namespace ZBuildLights.Core.Configuration
{
    public class ApplicationConfiguration : IApplicationConfiguration
    {
        public string StorageFilePath
        {
            get { return Required("StorageFilePath"); }
        }

        private string Required(string key)
        {
            var value = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrEmpty(value))
                throw new ConfigurationErrorsException(string.Format("AppSetting {0} is required.", key));
            return value;
        }
    }
}
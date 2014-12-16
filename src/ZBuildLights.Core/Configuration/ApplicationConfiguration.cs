using System.Configuration;
using System.IO;

namespace ZBuildLights.Core.Configuration
{
    public class ApplicationConfiguration : IApplicationConfiguration
    {
        public string StorageFilePath
        {
            get
            {
                var value = Required("StorageFilePath");
                var fullPath = Path.GetFullPath(value);
                return fullPath;
            }
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
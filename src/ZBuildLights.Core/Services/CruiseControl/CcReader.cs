using System;
using System.IO;
using System.Net;
using System.Xml.Serialization;
using NLog;
using ZBuildLights.Core.Models.CruiseControl;
using ZBuildLights.Core.Models.Requests;

namespace ZBuildLights.Core.Services.CruiseControl
{
    public class CcReader : ICcReader
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        public NetworkResponse<Projects> GetStatus(string url)
        {
            try
            {
                var request = WebRequest.Create(url);

                Projects projects;
                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var serializer = new XmlSerializer(typeof (Projects));
                    projects = (Projects) serializer.Deserialize(reader);
                }

                return NetworkResponse.Success(projects);
            }
            catch (Exception e)
            {
                Log.ErrorException("Exception when trying to get status from Cruise server.", e);
                return NetworkResponse.Fail<Projects>(string.Format("{0}: {1}", e.GetType().Name, e.Message));
            }
        }
    }
}
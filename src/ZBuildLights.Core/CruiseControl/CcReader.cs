using System.IO;
using System.Net;
using System.Xml.Serialization;

namespace ZBuildLights.Core.CruiseControl
{
    public class CcReader : ICcReader
    {
        public Projects GetStatus(string url)
        {
            var request = WebRequest.Create(url);

            Projects projects;
            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                var serializer = new XmlSerializer(typeof(Projects));
                projects = (Projects)serializer.Deserialize(reader);
            }

            return projects;
        }
    }
}
﻿using System;
using System.IO;
using System.Net;
using System.Xml.Serialization;
using ZBuildLights.Core.Models.Requests;

namespace ZBuildLights.Core.CruiseControl
{
    public class CcReader : ICcReader
    {
        public NetworkRequest<Projects> GetStatus(string url)
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

                return NetworkRequest.Success(projects);
            }
            catch (Exception e)
            {
                return NetworkRequest.Fail<Projects>(string.Format("{0}: {1}", e.GetType().Name, e.Message));
            }
        }
    }
}
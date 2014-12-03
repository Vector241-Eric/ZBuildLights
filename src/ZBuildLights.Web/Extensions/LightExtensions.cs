using System;
using BuildLightControl;

namespace ZBuildLights.Web.Extensions
{
    public static class LightExtensions
    {
        public static string DisplayClass(this Light light)
        {
            if (light.Status == LightStatus.Off)
                return string.Empty;

            if (light.Color.Equals(LightColor.Green))
                return "success";

            if (light.Color.Equals(LightColor.Yellow))
                return "warning";

            if (light.Color.Equals(LightColor.Red))
                return "danger";

            throw new Exception(string.Format("light color {0} is not handled.", light.Color.DisplayName));
        }
    }
}
using System;
using BuildLightControl;
using ZBuildLights.Core.Models;

namespace ZBuildLights.Web.Extensions
{
    public static class LightExtensions
    {
        public static string DisplayClass(this Light light)
        {
            if (light.SwitchState == SwitchState.Off)
                return string.Empty;
            return light.Color.DisplayClass();
        }

        public static string DisplayClass(this LightColor color)
        {
            if (color.Equals(LightColor.Green))
                return "success";

            if (color.Equals(LightColor.Yellow))
                return "warning";

            if (color.Equals(LightColor.Red))
                return "danger";

            if (color.Equals(LightColor.Unknown))
                return string.Empty;

            throw new Exception(string.Format("light color {0} is not handled.", color.DisplayName));
        }
    }
}
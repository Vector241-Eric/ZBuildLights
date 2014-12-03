using System;
using BuildLightControl;

namespace ZBuildLights.Web.Extensions
{
    public static class LightExtensions
    {
        public static string DisplayClass(this SwitchableLight switchableLight)
        {
            if (switchableLight.SwitchState == SwitchState.Off)
                return string.Empty;

            if (switchableLight.Color.Equals(LightColor.Green))
                return "success";

            if (switchableLight.Color.Equals(LightColor.Yellow))
                return "warning";

            if (switchableLight.Color.Equals(LightColor.Red))
                return "danger";

            throw new Exception(string.Format("light color {0} is not handled.", switchableLight.Color.DisplayName));
        }
    }
}
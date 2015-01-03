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
            return string.Format("light-display-{0}", color.DisplayName.ToLowerInvariant());
        }
    }
}
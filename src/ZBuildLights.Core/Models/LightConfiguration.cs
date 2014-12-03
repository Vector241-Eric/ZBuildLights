using System.Linq;

namespace BuildLightControl
{
    public class LightConfiguration
    {
        public LightStatus Green { get; set; }
        public LightStatus Yellow { get; set; }
        public LightStatus Red { get; set; }

        public Light[] Lights
        {
            get
            {
                return new[]
                {
                    new Light(LightColor.Green, Green),
                    new Light(LightColor.Yellow, Yellow),
                    new Light(LightColor.Red, Red)
                };
            }
        }

        public override string ToString()
        {
            return string.Join(", ", Lights.Select(x => x.ToString()));
        }
    }
}
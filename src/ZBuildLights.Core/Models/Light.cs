using System.ComponentModel;
using BuildLightControl;

namespace ZBuildLights.Core.Models
{
    public class Light
    {
        public Project Project { get; private set; }
        public LightColor Color { get; private set; }
        public LightSwitch LightSwitch { get; private set; }
        public string Description { get; set; }
        

        public Light(LightSwitch lightSwitch, Project project)
        {
            LightSwitch = lightSwitch;
            Project = project;
        }

        public Light SetColor(LightColor color)
        {
            Color = color;
            return this;
        }

        public Light SetDescription(string description)
        {
            Description = description;
            return this;
        }
    }
}
namespace BuildLightControl
{
    public class Light
    {
        public LightColor Color { get; private set; }
        public LightStatus Status { get; private set; }
        public string Description { get; set; }

        public Light(LightColor color, LightStatus status)
        {
            Color = color;
            Status = status;
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", Color, Status);
        }
    }
}
using Headspring;

namespace ZBuildLights.Core.Models
{
    public class LightColor : Enumeration<LightColor>
    {
        public static LightColor Unknown = new LightColor(0, "<Unknown>", 0);
        public static LightColor Green = new LightColor(1, "Green", 1);
        public static LightColor Yellow = new LightColor(2, "Yellow", 2);
        public static LightColor Red = new LightColor(3, "Red", 3);

        public int DisplayOrder { get; private set; }

        public LightColor(int value, string displayName, int displayOrder) : base(value, displayName)
        {
            DisplayOrder = displayOrder;
        }
    }
}
using ZWaveControl;

namespace ZBuildLightsConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                new ConsoleCommandWatcher().Run();
            }
            finally
            {
                ZWaveManagerFactory.Destroy();
            }
        }
    }
}
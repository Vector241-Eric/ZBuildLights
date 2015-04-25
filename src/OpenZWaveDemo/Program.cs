using System.Linq;

namespace OpenZWaveDemo
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var lowerArgs = args.Select(x => x.ToLowerInvariant()).ToArray();
            if (lowerArgs.Contains("--nodes"))
                DumpZWaveNodes();
        }

        private static void DumpZWaveNodes()
        {
            throw new System.NotImplementedException();
        }
    }
}
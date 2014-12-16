using System;

namespace ZBuildLights.Core.Services
{
    public static class SystemClock
    {
        public static Func<DateTime> Now =
            () => { throw new Exception(string.Format("Type {0} is not initialized.", typeof (SystemClock).FullName)); };

        public static void UseCurrentTime()
        {
            Now = () => DateTime.Now;
        }
    }
}
using System;
using System.IO;
using System.ServiceProcess;
using System.Timers;
using NLog;

namespace ZBuildLightsUpdater
{
    public partial class ZBuildLightsUpdaterService : ServiceBase
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private Timer _timer;

        public ZBuildLightsUpdaterService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            File.AppendAllLines(@"c:\var\ZBuildLights\_logs\servicelog.txt", new[]{"Service starting..."});
            Log.Info("ZBuildLights Updater Service Starting...");
            _timer = new Timer();
            _timer.Interval = TimeSpan.FromSeconds(5).TotalMilliseconds;
            _timer.Elapsed += OnTimerElapsed;
            Log.Info("ZBuildLights Updater Service Startup Complete.");
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Log.Debug("Initiating status refresh...");
        }

        protected override void OnStop()
        {
            Log.Info("ZBuildLights Updater Service Stopping.");
        }
    }
}
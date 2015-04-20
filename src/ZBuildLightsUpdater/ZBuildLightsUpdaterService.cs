using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.ServiceProcess;
using System.Timers;
using NLog;

namespace ZBuildLightsUpdater
{
    public partial class ZBuildLightsUpdaterService : ServiceBase
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private Timer _timer;
        private string _triggerUrl;

        public ZBuildLightsUpdaterService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            File.AppendAllLines(@"c:\var\ZBuildLights\_logs\servicelog.txt", new[] {"Service starting..."});
            Log.Info("ZBuildLights Updater Service Starting...");
            _timer = new Timer();
            var updateSeconds = Int32.Parse(ConfigurationManager.AppSettings["UpdateIntervalSeconds"]);
            _timer.Interval = TimeSpan.FromSeconds(updateSeconds).TotalMilliseconds;
            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();

            _triggerUrl = ConfigurationManager.AppSettings["TriggerUrl"];

            Log.Info("ZBuildLights Updater Service Startup Complete.");
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Log.Debug("Initiating status refresh...");
            var request = WebRequest.Create(_triggerUrl);
            request.GetResponse();
            Log.Debug("Status refresh complete.");
        }

        protected override void OnStop()
        {
            Log.Info("ZBuildLights Updater Service Stopping.");
        }
    }
}
using System;
using System.Diagnostics;
using OpenZWaveDotNet;

namespace BuildLightControl.ZWave
{
    public static class ZWaveManagerFactory
    {
        private static ZWManager _instance;

        private static readonly ConfigurationSettings _config = new ConfigurationSettings();

        private static readonly ZWOptions _options = new ZWOptions();
        private static readonly object _lock = new object();

        public static ZWManager GetInstance()
        {
            lock (_lock)
            {
                if (_instance == null)
                    _instance = Create();
            }
            return _instance;
        }

        private static ZWManager Create(Action<ZWNotification> notificationHandler = null)
        {
            SetOptions();

            var manager = new ZWManager();
            // create the OpenZWave Manager
            manager.Create();
            manager.OnNotification += notification => ZWaveNotificationHandler.HandleNotification(notification, manager);
            manager.OnControllerStateChanged += state => { Debug.WriteLine(state); };
            // once the driver is added it takes some time for the device to get ready
            manager.AddDriver(_config.ZWavePort);
            return manager;
        }

        private static void SetOptions()
        {
            // the directory the config files are copied to in the post build
            _options.Create(_config.ZWaveConfigurationPath, @"", @"");

            // logging options
            _options.AddOptionInt("SaveLogLevel", (int) ZWLogLevel.Debug);
            _options.AddOptionInt("QueueLogLevel", (int) ZWLogLevel.Debug);
            _options.AddOptionInt("DumpTriggerLevel", (int) ZWLogLevel.Error);

            // lock the options
            _options.Lock();
        }
    }
}
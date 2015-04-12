using System.Diagnostics;
using OpenZWaveDotNet;

namespace ZWaveControl
{
    public static class ZWaveManagerFactory
    {
        private static ZWManager _instance;

        private static readonly ZWaveSettings ZWaveSettings = new ZWaveSettings();

        private static readonly ZWOptions _options = new ZWOptions();
        private static readonly object _lock = new object();

        public static DisposableManager GetInstance()
        {
            lock (_lock)
            {
                if (_instance == null)
                    _instance = Create();
            }
            return new DisposableManager(_instance);
        }

        private static ZWManager Create()
        {
            SetOptions();

            var manager = new ZWManager();
            // create the OpenZWave Manager
            manager.Create();
            manager.OnNotification += notification => ZWaveNotificationHandler.HandleNotification(notification, manager);
            manager.OnControllerStateChanged += state => { Debug.WriteLine(state); };
            // once the driver is added it takes some time for the device to get ready
            manager.AddDriver(ZWaveSettings.ControllerPortNumber);
            return manager;
        }

        private static void SetOptions()
        {
            // the directory the config files are copied to in the post build
            _options.Create(ZWaveSettings.ConfigurationPath, @"", @"");

            // logging options
            _options.AddOptionInt("SaveLogLevel", (int)ZWLogLevel.Debug);
            _options.AddOptionInt("QueueLogLevel", (int)ZWLogLevel.Debug);
            _options.AddOptionInt("DumpTriggerLevel", (int)ZWLogLevel.Error);

            // lock the options
            _options.Lock();
        }
    }
}
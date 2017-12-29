using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Timers;

namespace DirectoryMonitor.Services
{
    partial class WatcherService : ServiceBase
    {
//        private FileSystemWatcher _watcher;
        private EventLog _eventLog;
        private int eventId = 1;

        public WatcherService()
        {
            InitializeComponent();
//            _watcher = new FileSystemWatcher();
            _eventLog = new EventLog();

            if (!EventLog.SourceExists("WatcherSource"))
            {
                EventLog.CreateEventSource("WatcherSource", "WatcherLog");
            }

            _eventLog.Source = "WatcherSource";
            _eventLog.Log = "WatcherLog";
        }

        protected override void OnStart(string[] args)
        {
            // TODO: Add code here to start your service.
            _eventLog.WriteEntry("Hello from OnStart");
            var timer = new Timer {Interval = 10000};
            timer.Elapsed += OnTimer;
        }

        public void OnTimer(object sender, ElapsedEventArgs args)
        {
            _eventLog.WriteEntry("Monitoring the System", EventLogEntryType.Information, eventId++);
        }
        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
        }
    }
}
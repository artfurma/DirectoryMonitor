using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Timers;
using DirectoryMonitor.Libs;

namespace DirectoryMonitor.Services
{
	partial class WatcherService : ServiceBase
	{
		private Timer _timer = null;

		public WatcherService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			_timer = new Timer {Interval = 1000};
			_timer.Elapsed += OnTimer;
			_timer.Enabled = true;
			WatcherLib.StartMonitoring();
		}

		public void OnTimer(object sender, ElapsedEventArgs args)
		{
		}

		protected override void OnStop()
		{
			WatcherLib.StopMonitoring();
		}
	}
}
using System;
using System.Configuration;
using System.ServiceProcess;
using System.Threading;
using System.Timers;
using WatcherLibs;

namespace WatcherService
{
	public partial class FileSystemWatcher : ServiceBase
	{
		private Thread _thread;
		private readonly ManualResetEvent _shutdownEvent = new ManualResetEvent(false);

		public FileSystemWatcher()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			_thread = new Thread(WorkerThreadFunc)
			{
				Name = "FileWatcherService Worker",
				IsBackground = true
			};

			_thread.Start();
		}

		private void WorkerThreadFunc()
		{
			FileWatcherLogic.StartMonitoring();
			GetSettings();
			FileWatcherLogic.ConfigureWatcher();

			while (!_shutdownEvent.WaitOne(0))
			{
				ConfigurationManager.RefreshSection("appSettings");
				GetSettings();
				FileWatcherLogic.UpdateWatcherSettings();
				Thread.Sleep(1000);
			}
		}

		private void GetSettings()
		{
			FileWatcherLogic.Path = AppSettingsHelper.GetSetting("Path");
			FileWatcherLogic.Filter = AppSettingsHelper.GetSetting("Filter");
			FileWatcherLogic.Subdirs = bool.Parse(AppSettingsHelper.GetSetting("SubDirs"));
			FileWatcherLogic.Create = bool.Parse(AppSettingsHelper.GetSetting("Create"));
			FileWatcherLogic.Edit = bool.Parse(AppSettingsHelper.GetSetting("Edit"));
			FileWatcherLogic.Delete = bool.Parse(AppSettingsHelper.GetSetting("Delete"));
			FileWatcherLogic.Rename = bool.Parse(AppSettingsHelper.GetSetting("Rename"));
			FileWatcherLogic.Error = bool.Parse(AppSettingsHelper.GetSetting("Error"));
		}

		protected override void OnStop()
		{
			FileWatcherLogic.StopMonitoring();
			_shutdownEvent.Set();

			if (!_thread.Join(3000))
			{
				_thread.Abort();
			}
		}
	}
}
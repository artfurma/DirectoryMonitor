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
			ConfigureWatcher();

			while (!_shutdownEvent.WaitOne(0))
			{
			}
		}

		private void ConfigureWatcher()
		{
			var path = AppSettingsHelper.GetSetting("Path");
			var filter = AppSettingsHelper.GetSetting("Filter");
			var subdirs = AppSettingsHelper.GetSetting("SubDirs");
			var create = AppSettingsHelper.GetSetting("Create");
			var edit = AppSettingsHelper.GetSetting("Edit");
			var delete = AppSettingsHelper.GetSetting("Delete");
			var rename = AppSettingsHelper.GetSetting("Rename");
			var error = AppSettingsHelper.GetSetting("Error");


			if (path == "Setting not found")
			{
				AppSettingsHelper.AddUpdateSetting("Path", AppDomain.CurrentDomain.BaseDirectory);
				path = AppSettingsHelper.GetSetting("Path");
			}

			if (filter == "Setting not found")
			{
				AppSettingsHelper.AddUpdateSetting("Filter", "*.*");
				filter = AppSettingsHelper.GetSetting("Filter");
			}

			if (subdirs == "Setting not found")
			{
				AppSettingsHelper.AddUpdateSetting("SubDirs", "False");
				subdirs = AppSettingsHelper.GetSetting("SubDirs");
			}

			if (create == "Setting not found")
			{
				AppSettingsHelper.AddUpdateSetting("Create", "True");
				subdirs = AppSettingsHelper.GetSetting("Create");
			}

			if (edit == "Setting not found")
			{
				AppSettingsHelper.AddUpdateSetting("Edit", "True");
				subdirs = AppSettingsHelper.GetSetting("Edit");
			}

			if (delete == "Setting not found")
			{
				AppSettingsHelper.AddUpdateSetting("Delete", "True");
				subdirs = AppSettingsHelper.GetSetting("Delete");
			}

			if (rename == "Setting not found")
			{
				AppSettingsHelper.AddUpdateSetting("Rename", "True");
				subdirs = AppSettingsHelper.GetSetting("Rename");
			}

			if (error == "Setting not found")
			{
				AppSettingsHelper.AddUpdateSetting("Error", "True");
				subdirs = AppSettingsHelper.GetSetting("Error");
			}

			FileWatcherLogic.ConfigureWatcher(path, filter, subdirs, bool.Parse(create), bool.Parse(edit), bool.Parse(delete),
				bool.Parse(rename));
		}

		public void OnTimer(object sender, ElapsedEventArgs args)
		{
			ConfigurationManager.RefreshSection("appSettings");
			ConfigureWatcher();
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
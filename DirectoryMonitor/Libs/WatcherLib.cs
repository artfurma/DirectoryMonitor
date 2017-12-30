using System;
using System.IO;

namespace DirectoryMonitor.Libs
{
	public static class WatcherLib
	{
		private static readonly log4net.ILog Log =
			log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public static void StartMonitoring()
		{
			ConfigureWatcher();
			Log.Info("Rozpoczęto monitorowanie");
		}

		public static void StopMonitoring()
		{
			Log.Info("Zakończono monitorowanie");
		}

		private static void ConfigureWatcher()
		{
			var watcher = new FileSystemWatcher
			{
				Path = AppSettingsHelper.GetSetting("Path"),
				Filter = AppSettingsHelper.GetSetting("Filter")
			};

			if (watcher.Path == "Setting not found")
			{
				AppSettingsHelper.AddUpdateSetting("Path", AppDomain.CurrentDomain.BaseDirectory);
			}


			if (watcher.Filter == "Setting not found")
			{
				AppSettingsHelper.AddUpdateSetting("Filter", "*.*");
			}

			watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
			                       | NotifyFilters.FileName | NotifyFilters.DirectoryName;

			// Event handlers
			watcher.Changed += OnChanged;
			watcher.Created += OnCreated;
			watcher.Deleted += OnDeleted;
			watcher.Renamed += OnRenamed;

			// Begin watching
			watcher.EnableRaisingEvents = true;
		}

		private static void OnRenamed(object sender, RenamedEventArgs renamedEventArgs)
		{
			Log.Warn(
				$"{DateTime.Now:MMM ddd d HH:mm yyyy} Zmieniono nazwę pliku z {renamedEventArgs.OldFullPath} na {renamedEventArgs.FullPath}");
		}

		private static void OnDeleted(object sender, FileSystemEventArgs fileSystemEventArgs)
		{
			Log.Error($"{DateTime.Now:MMM ddd d HH:mm yyyy} Usunięto plik: {fileSystemEventArgs.FullPath}");
		}

		private static void OnCreated(object sender, FileSystemEventArgs fileSystemEventArgs)
		{
			Log.Warn($"{DateTime.Now:MMM ddd d HH:mm yyyy} Utworzono plik: {fileSystemEventArgs.FullPath}");
		}

		private static void OnChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
		{
			Log.Warn($"{DateTime.Now:MMM ddd d HH:mm yyyy} Zmeiniono plik!");
		}
	}
}
using System;
using System.Dynamic;
using System.IO;

namespace WatcherLibs
{
	public static class FileWatcherLogic
	{
		private static readonly log4net.ILog Log =
			log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public static void StartMonitoring()
		{
			log4net.Config.XmlConfigurator.Configure();
			Log.Info("Rozpoczęto monitorowanie");
		}

		public static void StopMonitoring()
		{
			Log.Info("Zakończono monitorowanie");
		}

		public static void ConfigureWatcher(string path, string filter, string subdirs, bool create, bool edit, bool delete,
			bool rename, bool error)
		{
			var watcher = new FileSystemWatcher
			{
				Path = path,
				Filter = filter,
				NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
				               | NotifyFilters.FileName | NotifyFilters.DirectoryName,
				IncludeSubdirectories = bool.Parse(subdirs)
			};

			// Event handlers
			if (edit)
				watcher.Changed += OnChanged;
			if (create)
				watcher.Created += OnCreated;
			if (delete)
				watcher.Deleted += OnDeleted;
			if (rename)
				watcher.Renamed += OnRenamed;
			if (error)
				watcher.Error += OnError;

			// Begin watching
			watcher.EnableRaisingEvents = true;
		}

		private static void OnError(object sender, ErrorEventArgs e)
		{
			Log.Error($"{DateTime.Now:MMM ddd d HH:mm yyyy} Wystąpił błąd: {e.GetException()}");
		}

		private static void OnRenamed(object sender, RenamedEventArgs renamedEventArgs)
		{
			Log.Warn(
				$"{DateTime.Now:MMM ddd d HH:mm yyyy} Zmieniono nazwę pliku z {renamedEventArgs.OldFullPath} na {renamedEventArgs.FullPath}");
		}

		private static void OnDeleted(object sender, FileSystemEventArgs fileSystemEventArgs)
		{
			Log.Warn($"{DateTime.Now:MMM ddd d HH:mm yyyy} Usunięto plik: {fileSystemEventArgs.FullPath}");
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
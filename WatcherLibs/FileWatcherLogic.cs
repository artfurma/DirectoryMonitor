using System;
using System.IO;

namespace WatcherLibs
{
	public static class FileWatcherLogic
	{
		private static readonly log4net.ILog Log =
			log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private static FileSystemWatcher _watcher;
		public static string Path { get; set; }
		public static string Filter { get; set; }
		public static bool Subdirs { get; set; }
		public static bool Create { get; set; }
		public static bool Edit { get; set; }
		public static bool Delete { get; set; }
		public static bool Rename { get; set; }
		public static bool Error { get; set; }

		private static bool _createSubscribed;
		private static bool _editSubscribed;
		private static bool _deleteSubscribed;
		private static bool _renameSubscribed;
		private static bool _errorSubscribed;

		public static void StartMonitoring()
		{
			log4net.Config.XmlConfigurator.Configure();
			Log.Info("Rozpoczęto monitorowanie");
		}

		public static void StopMonitoring()
		{
			Log.Info("Zakończono monitorowanie");
		}

		public static void ConfigureWatcher()
		{
			_watcher = new FileSystemWatcher
			{
				Path = Path,
				Filter = Filter,
				NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
				               | NotifyFilters.FileName | NotifyFilters.DirectoryName,
				IncludeSubdirectories = Subdirs
			};

			// Event handlers
			if (Edit)
			{
				_watcher.Changed += OnChanged;
				_editSubscribed = true;
			}
			if (Create)
			{
				_watcher.Created += OnCreated;
				_createSubscribed = true;
			}
			if (Delete)
			{
				_watcher.Deleted += OnDeleted;
				_deleteSubscribed = true;
			}
			if (Rename)
			{
				_watcher.Renamed += OnRenamed;
				_renameSubscribed = true;
			}
			if (Error)
			{
				_watcher.Error += OnError;
				_errorSubscribed = true;
			}

			// Begin watching
			_watcher.EnableRaisingEvents = true;
		}

		public static void UpdateWatcherSettings()
		{
			_watcher.Path = Path;
			_watcher.Filter = Filter;
			_watcher.IncludeSubdirectories = Subdirs;

			if (!_editSubscribed && Edit)
			{
				_watcher.Changed += OnChanged;
				_editSubscribed = true;
			}
			else if (_editSubscribed && !Edit)
			{
				_watcher.Changed -= OnChanged;
				_editSubscribed = false;
			}

			if (!_deleteSubscribed && Delete)
			{
				_watcher.Deleted += OnDeleted;
				_deleteSubscribed = false;
			}
			else if (_deleteSubscribed && !Delete)
			{
				_watcher.Deleted -= OnDeleted;
				_deleteSubscribed = true;
			}

			if (!_createSubscribed && Create)
			{
				_watcher.Created += OnCreated;
				_createSubscribed = true;
			}
			else if (_createSubscribed && !Create)
			{
				_watcher.Created -= OnCreated;
				_createSubscribed = false;
			}

			if (!_renameSubscribed && Rename)
			{
				_watcher.Renamed += OnRenamed;
				_renameSubscribed = true;
			}
			else if (_renameSubscribed && !Rename)
			{
				_watcher.Renamed -= OnRenamed;
				_renameSubscribed = false;
			}

			if (!_errorSubscribed && Error)
			{
				_watcher.Error += OnError;
				_renameSubscribed = true;
			}
			else if (_editSubscribed && !Error)
			{
				_watcher.Error -= OnError;
				_renameSubscribed = false;
			}
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
			Log.Warn(
				$"{DateTime.Now:MMM ddd d HH:mm yyyy} Zmodyfikowano plik {fileSystemEventArgs.Name}. Rodzaj zmiany: {fileSystemEventArgs.ChangeType}");
		}
	}
}
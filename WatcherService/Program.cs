using System.ServiceProcess;

namespace WatcherService
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			var servicesToRun = new ServiceBase[]
			{
				new FileSystemWatcher()
			};
			ServiceBase.Run(servicesToRun);
		}
	}
}
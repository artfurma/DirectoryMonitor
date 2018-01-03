using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using WatcherLibs;
using MessageBox = System.Windows.MessageBox;
using ServiceControllerStatus = System.ServiceProcess.ServiceControllerStatus;

namespace DirectoryMonitor
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private ServiceController _serviceController;
		private string _configPath;

		public MainWindow()
		{
			InitializeComponent();
			_serviceController = new ServiceController {ServiceName = "PLANET File Watcher Service"};
			Init();
		}

		private void Init()
		{
			PathTextBox.Text = "Wybierz ścieżkę...";

			switch (_serviceController.Status)
			{
				case ServiceControllerStatus.Running:
					ServiceStatusLabel.Content = "Usługa uruchomiona";
					ServiceStatusLabel.Foreground = new SolidColorBrush(Colors.Green);
					break;
				case ServiceControllerStatus.Paused:
					ServiceStatusLabel.Content = "Usługa wstrzymana";
					ServiceStatusLabel.Foreground = new SolidColorBrush(Colors.Blue);
					break;
				case ServiceControllerStatus.Stopped:
					ServiceStatusLabel.Content = "Usługa zatrzymana";
					ServiceStatusLabel.Foreground = new SolidColorBrush(Colors.Red);
					break;
			}
			GetSettingsFromConfig();
		}

		private void GetSettingsFromConfig()
		{
			var projectDir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent?.FullName;
			var serviceExePath = Directory.GetParent(projectDir).FullName + "\\WatcherService\\bin\\Debug\\WatcherService.exe";
			_configPath = serviceExePath;

			var path = AppSettingsHelper.GetExternalSetting(serviceExePath, "Path");
			var filter = AppSettingsHelper.GetExternalSetting(serviceExePath, "Filter");
			var subdirs = AppSettingsHelper.GetExternalSetting(serviceExePath, "SubDirs");
			var create = AppSettingsHelper.GetExternalSetting(serviceExePath, "Create");
			var edit = AppSettingsHelper.GetExternalSetting(serviceExePath, "Edit");
			var delete = AppSettingsHelper.GetExternalSetting(serviceExePath, "Delete");
			var rename = AppSettingsHelper.GetExternalSetting(serviceExePath, "Rename");
			var error = AppSettingsHelper.GetExternalSetting(serviceExePath, "Error");

			if (path != "Setting not found")
			{
				PathTextBox.Text = path;
			}

			if (filter != "Setting not found")
			{
				FilterTextBox.Text = filter;
			}

			if (subdirs != "Setting not found")
			{
				IncludeSubdirsCheckBox.IsChecked = bool.Parse(subdirs);
			}

			if (create != "Setting not found")
			{
				CreateCheckBox.IsChecked = bool.Parse(create);
			}

			if (edit != "Setting not found")
			{
				EditCheckBox.IsChecked = bool.Parse(edit);
			}

			if (delete != "Setting not found")
			{
				DeleteCheckBox.IsChecked = bool.Parse(delete);
			}

			if (rename != "Setting not found")
			{
				RenameCheckBox.IsChecked = bool.Parse(rename);
			}

			if (error != "Setting not found")
			{
				ErrorCheckBox.IsChecked = bool.Parse(error);
			}
		}

		private void StartServiceButton_Click(object sender, RoutedEventArgs e)
		{
			if (_serviceController.Status == ServiceControllerStatus.Running ||
			    _serviceController.Status == ServiceControllerStatus.StartPending)
			{
				MessageBox.Show("Serwis już działa");
				return;
			}
			try
			{
				_serviceController.Start();
				_serviceController.WaitForStatus(ServiceControllerStatus.Running);

				if (_serviceController.Status == ServiceControllerStatus.Running)
				{
					ServiceStatusLabel.Content = "Usługa uruchomiona";
					ServiceStatusLabel.Foreground = new SolidColorBrush(Colors.Green);
				}
			}
			catch (Exception exception)
			{
				MessageBox.Show(exception.ToString());
				throw;
			}
		}

		private void PauseServiceButton_Click(object sender, RoutedEventArgs e)
		{
			if (_serviceController.Status == ServiceControllerStatus.Paused ||
			    _serviceController.Status == ServiceControllerStatus.PausePending)
			{
				MessageBox.Show("Usługa jest już zatrzymana");
				return;
			}
			try
			{
				_serviceController.Pause();
				_serviceController.WaitForStatus(ServiceControllerStatus.Paused);
				if (_serviceController.Status == ServiceControllerStatus.Paused)
				{
					ServiceStatusLabel.Content = "Usługa wstrzymana";
					ServiceStatusLabel.Foreground = new SolidColorBrush(Colors.Blue);
				}
			}
			catch (Exception exception)
			{
				MessageBox.Show(exception.ToString());
				throw;
			}
		}

		private void StopServiceButton_Click(object sender, RoutedEventArgs e)
		{
			if (_serviceController.Status == ServiceControllerStatus.Stopped ||
			    _serviceController.Status == ServiceControllerStatus.StopPending)
			{
				MessageBox.Show("Usługa jest już wyłączona");
				return;
			}

			try
			{
				_serviceController.Stop();
				_serviceController.WaitForStatus(ServiceControllerStatus.Stopped);

				if (_serviceController.Status == ServiceControllerStatus.Stopped)
				{
					ServiceStatusLabel.Content = "Usługa zatrzymana";
					ServiceStatusLabel.Foreground = new SolidColorBrush(Colors.Red);
				}
			}
			catch (Exception exception)
			{
				MessageBox.Show(exception.ToString());
				throw;
			}
		}

		private void BrowseDirectoryButton_Click(object sender, RoutedEventArgs e)
		{
			using (var folderDialog = new FolderBrowserDialog())
			{
				if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					PathTextBox.Text = folderDialog.SelectedPath;
				}
			}
		}

		private void SaveConfigButton_Click(object sender, RoutedEventArgs e)
		{
			AppSettingsHelper.AddUpdateExternalSetting(_configPath, "Path", PathTextBox.Text);
			AppSettingsHelper.AddUpdateExternalSetting(_configPath, "Filter", FilterTextBox.Text);

			AppSettingsHelper.AddUpdateExternalSetting(_configPath, "Create",
				CreateCheckBox.IsChecked == true ? "True" : "False");

			AppSettingsHelper.AddUpdateExternalSetting(_configPath, "Edit",
				EditCheckBox.IsChecked == true ? "True" : "False");

			AppSettingsHelper.AddUpdateExternalSetting(_configPath, "Delete",
				DeleteCheckBox.IsChecked == true ? "True" : "False");

			AppSettingsHelper.AddUpdateExternalSetting(_configPath, "Rename",
				RenameCheckBox.IsChecked == true ? "True" : "False");

			AppSettingsHelper.AddUpdateExternalSetting(_configPath, "SubDirs",
				IncludeSubdirsCheckBox.IsChecked == true ? "True" : "False");

			AppSettingsHelper.AddUpdateExternalSetting(_configPath, "Error",
				ErrorCheckBox.IsChecked == true ? "True" : "False");

			MessageBox.Show("Zmiany zostały zapisane");
		}
	}
}
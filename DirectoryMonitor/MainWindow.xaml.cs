using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DirectoryMonitor.Services;

namespace DirectoryMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ServiceBase _serviceToRun;
        private ServiceController _serviceController;

        public MainWindow()
        {
            InitializeComponent();
            ReadAllSettings(); // tylko w celu sprawdzenia ustawień
            _serviceToRun = new WatcherService {ServiceName = "planetwatcherservice"};
            _serviceController = new ServiceController {ServiceName = "planetwatcherservice"};
            PathTextBox.Text = "Wybierz ścieżkę...";
        }

        private void ReadAllSettings()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;

                if (appSettings.Count == 0)
                {
                    Console.WriteLine(@"Brak ustawień");
                }
                else
                {
                    foreach (var key in appSettings.AllKeys)
                    {
                        Debug.WriteLine($"Key: {key}, Value: {appSettings[key]}");
                    }
                }
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine(@"Wystąpił błąd podczas odczytywania ustawień");
            }
        }

        private string GetSetting(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                var result = appSettings[key] ?? "Nie znaleziono";
                return result;
            }
            catch (ConfigurationErrorsException e)
            {
                Debug.WriteLine(e);
                throw;
            }
        }

        private void AddUpdateSetting(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }

                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException e)
            {
                Debug.WriteLine(e);
                throw;
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
//                ServiceBase.Run(_serviceToRun);

            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                throw;
            }
        }

        private void PauseServiceButton_Click(object sender, RoutedEventArgs e)
        {
            if (_serviceController.Status == ServiceControllerStatus.Paused ||
                _serviceController.Status == ServiceControllerStatus.PausePending)
            {
                MessageBox.Show("Serwis już spauzowany");
                return;
            }

            try
            {
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                throw;
            }
        }

        private void StopServiceButton_Click(object sender, RoutedEventArgs e)
        {
            if (_serviceController.Status == ServiceControllerStatus.Stopped ||
                _serviceController.Status == ServiceControllerStatus.StopPending)
            {
                MessageBox.Show("Serwis już jest zatrzymany");
                return;
            }

            try
            {
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                throw;
            }
        }

        private void ExportLogButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void BrowseDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
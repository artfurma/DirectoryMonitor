﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryMonitor.Libs
{
	public static class AppSettingsHelper
	{
		public static string GetSetting(string key)
		{
			try
			{
				var appSettings = ConfigurationManager.AppSettings;
				var result = appSettings[key] ?? "Setting not found";
				return result;
			}
			catch (ConfigurationErrorsException e)
			{
				Debug.WriteLine(e);
				throw;
			}
		}

		public static void AddUpdateSetting(string key, string value)
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
	}
}
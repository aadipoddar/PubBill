using System.Diagnostics;
using System.Text.Json;

namespace PubBillLibrary.Data;

public static class LocalSettingsData
{
	private static readonly string _appDataFolder;
	private const string _settingsFolderName = "DeviceSettings";

	static LocalSettingsData()
	{
		// Initialize the application's local settings directory
		_appDataFolder = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
			"PubBill",
			_settingsFolderName);

		// Ensure the directory exists
		if (!Directory.Exists(_appDataFolder))
		{
			try
			{
				Directory.CreateDirectory(_appDataFolder);
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error creating local settings directory: {ex.Message}");
			}
		}
	}

	/// <summary>
	/// Gets the value of a local device setting
	/// </summary>
	/// <typeparam name="T">The type of the setting value</typeparam>
	/// <param name="key">The setting key</param>
	/// <param name="defaultValue">Default value to return if setting doesn't exist</param>
	/// <returns>The setting value or defaultValue if not found</returns>
	public static T GetSetting<T>(string key, T defaultValue)
	{
		string filePath = Path.Combine(_appDataFolder, $"{key}.json");

		if (!File.Exists(filePath))
			return defaultValue;

		try
		{
			string json = File.ReadAllText(filePath);
			return JsonSerializer.Deserialize<T>(json) ?? defaultValue;
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Error reading local setting {key}: {ex.Message}");
			return defaultValue;
		}
	}

	/// <summary>
	/// Saves a local device setting
	/// </summary>
	/// <typeparam name="T">The type of the setting value</typeparam>
	/// <param name="key">The setting key</param>
	/// <param name="value">The setting value to save</param>
	/// <returns>True if successful, otherwise false</returns>
	public static bool SaveSetting<T>(string key, T value)
	{
		string filePath = Path.Combine(_appDataFolder, $"{key}.json");

		try
		{
			string json = JsonSerializer.Serialize(value, new JsonSerializerOptions
			{
				WriteIndented = true
			});

			File.WriteAllText(filePath, json);
			return true;
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Error saving local setting {key}: {ex.Message}");
			return false;
		}
	}

	/// <summary>
	/// Deletes a local device setting
	/// </summary>
	/// <param name="key">The setting key to delete</param>
	/// <returns>True if successful, otherwise false</returns>
	public static bool DeleteSetting(string key)
	{
		string filePath = Path.Combine(_appDataFolder, $"{key}.json");

		if (!File.Exists(filePath))
			return true;

		try
		{
			File.Delete(filePath);
			return true;
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Error deleting local setting {key}: {ex.Message}");
			return false;
		}
	}
}

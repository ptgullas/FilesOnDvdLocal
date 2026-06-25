using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace MediaFilesOnDvd.Services
{
    public class UserSettings
    {
        public int ScreensaverTimeoutSeconds { get; set; } = 300;
        public string Theme { get; set; } = "Dark";
        public string AccentColor { get; set; } = "#fa4f74";
        public string BackgroundColor { get; set; } = "#121212";
    }

    public class SettingsService
    {
        private readonly string _settingsFilePath;
        private UserSettings _currentSettings = new();

        public event Action? OnSettingsChanged;

        public SettingsService()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appFolder = Path.Combine(appDataPath, "MediaFilesOnDvd");
            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
            }
            _settingsFilePath = Path.Combine(appFolder, "user-settings.json");
            LoadSettings();
        }

        public UserSettings Get()
        {
            return _currentSettings;
        }

        public void LoadSettings()
        {
            if (File.Exists(_settingsFilePath))
            {
                try
                {
                    var json = File.ReadAllText(_settingsFilePath);
                    var settings = JsonSerializer.Deserialize<UserSettings>(json);
                    if (settings != null)
                    {
                        _currentSettings = settings;
                    }
                }
                catch
                {
                    // If error reading, use default
                    _currentSettings = new UserSettings();
                }
            }
            else
            {
                _currentSettings = new UserSettings();
                SaveSettings(_currentSettings);
            }
        }

        public void SaveSettings(UserSettings settings)
        {
            _currentSettings = settings;
            try
            {
                var json = JsonSerializer.Serialize(_currentSettings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_settingsFilePath, json);
                OnSettingsChanged?.Invoke();
            }
            catch
            {
                // Ignore save errors
            }
        }
    }
}

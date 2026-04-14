using System;
using System.IO;
using System.Text.Json;

namespace FotoFlow.Core
{
    public enum FotoFlowMode
    {
        Basic,
        Advance
    }

    public sealed class AppUserSettings
    {
        public string? LastPathBasic { get; set; }
        public string? LastPathAdvance { get; set; }
        public FotoFlowMode? LastMode { get; set; }

        public static string SettingsFilePath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FotoFlow", "settings.json");

        public static AppUserSettings Load()
        {
            try
            {
                string path = SettingsFilePath;
                if (!File.Exists(path))
                    return new AppUserSettings();

                string json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<AppUserSettings>(json, SerializerOptions) ?? new AppUserSettings();
            }
            catch
            {
                return new AppUserSettings();
            }
        }

        public void Save()
        {
            string path = SettingsFilePath;
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            string json = JsonSerializer.Serialize(this, SerializerOptions);
            File.WriteAllText(path, json);
        }

        public static void Update(Action<AppUserSettings> mutator)
        {
            if (mutator == null) throw new ArgumentNullException(nameof(mutator));
            lock (Sync)
            {
                var current = Load();
                mutator(current);
                current.Save();
            }
        }

        private static readonly object Sync = new object();

        private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };
    }
}


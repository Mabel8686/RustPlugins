using System;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Steamworks;

namespace Oxide.Plugins
{
    [Info("Map Name & Timers", "Mabel", "1.0.0")]
    [Description("Displays map name, wipe timer and purge timers in the map name field.")]
    public class MapNameTimers : RustPlugin
    {
        private Configuration _config;
        protected override void SaveConfig() => Config.WriteObject(_config);
        protected override void LoadDefaultConfig() => _config = new Configuration();
        private string[] _mapNameCycle;
        private int _currentMapNameIndex;
        private string _cachedMapName;

        private class Configuration
        {
            [JsonProperty(PropertyName = "Map & Timer Names")]
            public string[] MapNameDisplayedCycle = { "Your Map Name", "Map Wipe In:", "The Purge in:" };

            [JsonProperty(PropertyName = "Display Wipe Timer")]
            public bool DisplayWipeTimer = true;

            [JsonProperty(PropertyName = "Wipe Timer Epoch Time")]
            public long WipeTimerEpochTime = 0;

            [JsonProperty(PropertyName = "Display Purge Timer")]
            public bool DisplayPurgeTimer = true;

            [JsonProperty(PropertyName = "Purge Timer Epoch Time")]
            public long PurgeTimerEpochTime = 0;

            [JsonProperty(PropertyName = "Timer Interval")]
            public float TimerInterval = 5.0f;
        }

        protected override void LoadConfig()
        {
            base.LoadConfig();
            try
            {
                _config = Config.ReadObject<Configuration>();
                if (_config == null) throw new Exception();
                SaveConfig();
            }
            catch
            {
                PrintError("Your configuration file contains an error. Using default configuration values.");
                LoadDefaultConfig();
            }
        }

        private void Init()
        {
            _mapNameCycle = _config.MapNameDisplayedCycle;
            _currentMapNameIndex = 0;
            timer.Every(_config.TimerInterval, UpdateMapName);
        }

        private void UpdateMapName()
        {
            string nameToSet = _mapNameCycle[_currentMapNameIndex];

            if (_currentMapNameIndex == 1 && _config.DisplayWipeTimer && _config.WipeTimerEpochTime > 0)
            {
                TimeSpan wipeTimeSpan = TimeSpan.FromSeconds(_config.WipeTimerEpochTime - DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                nameToSet += $" {wipeTimeSpan.Days}d {wipeTimeSpan.Hours}h";
            }
            else if (_currentMapNameIndex == 2 && _config.DisplayPurgeTimer && _config.PurgeTimerEpochTime > 0)
            {
                TimeSpan purgeTimeSpan = TimeSpan.FromSeconds(_config.PurgeTimerEpochTime - DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                nameToSet += $" {purgeTimeSpan.Days}d {purgeTimeSpan.Hours}h";
            }

            _cachedMapName = nameToSet;
            SteamServer.MapName = _cachedMapName;

            _currentMapNameIndex = (_currentMapNameIndex + 1) % _mapNameCycle.Length;
        }
    }
}
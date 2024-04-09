using System;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Steamworks;

namespace Oxide.Plugins
{
    [Info("Map Name Timers", "Mabel", "1.1.1")]
    [Description("Displays map name, map wipe, purge, blueprint & skilltree timers in the map name field.")]
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
            public string[] MapNameDisplayedCycle = { "Your Map Name", "Map Wipe In:", "The Purge in:", "Blueprint Wipe In:", "SkillTree Wipe In:" };

            [JsonProperty(PropertyName = "Display Wipe Timer")]
            public bool DisplayWipeTimer = true;

            [JsonProperty(PropertyName = "Wipe Timer Epoch Time")]
            public long WipeTimerEpochTime = 0;

            [JsonProperty(PropertyName = "Display Purge Timer")]
            public bool DisplayPurgeTimer = true;

            [JsonProperty(PropertyName = "Purge Timer Epoch Time")]
            public long PurgeTimerEpochTime = 0;

            [JsonProperty(PropertyName = "Display Blueprint Timer")]
            public bool DisplayBPTimer = true;

            [JsonProperty(PropertyName = "Blueprint Timer Epoch Time")]
            public long BPTimerEpochTime = 0;

            [JsonProperty(PropertyName = "Display SkillTree Timer")]
            public bool DisplaySTTimer = true;

            [JsonProperty(PropertyName = "SkillTree Timer Epoch Time")]
            public long STTimerEpochTime = 0;

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
                nameToSet += $" {wipeTimeSpan.Days}d {wipeTimeSpan.Hours}h {wipeTimeSpan.Minutes}m";
            }
            else if (_currentMapNameIndex == 2 && _config.DisplayPurgeTimer && _config.PurgeTimerEpochTime > 0)
            {
                TimeSpan purgeTimeSpan = TimeSpan.FromSeconds(_config.PurgeTimerEpochTime - DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                nameToSet += $" {purgeTimeSpan.Days}d {purgeTimeSpan.Hours}h {purgeTimeSpan.Minutes}m";
            }
			else if (_currentMapNameIndex == 3 && _config.DisplayBPTimer && _config.BPTimerEpochTime > 0)
            {
                TimeSpan bpTimeSpan = TimeSpan.FromSeconds(_config.BPTimerEpochTime - DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                nameToSet += $" {bpTimeSpan.Days}d {bpTimeSpan.Hours}h {bpTimeSpan.Minutes}m";
            }
			else if (_currentMapNameIndex == 4 && _config.DisplaySTTimer && _config.STTimerEpochTime > 0)
            {
                TimeSpan stTimeSpan = TimeSpan.FromSeconds(_config.STTimerEpochTime - DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                nameToSet += $" {stTimeSpan.Days}d {stTimeSpan.Hours}h {stTimeSpan.Minutes}m";
            }
			
			_cachedMapName = nameToSet;  
			
			timer.Once(0.1f, () =>
			{
				SteamServer.MapName = _cachedMapName;
			});
			
            _currentMapNameIndex = (_currentMapNameIndex + 1) % _mapNameCycle.Length;
        }

        private string GetPurgeTime()
        {
            if (!_config.DisplayPurgeTimer || _config.PurgeTimerEpochTime <= 0)
                return "0";

            TimeSpan purgeTimeSpan = TimeSpan.FromSeconds(_config.PurgeTimerEpochTime - DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            if (purgeTimeSpan.TotalSeconds <= 0)
                return "0";

            return $"{purgeTimeSpan.Days}D {purgeTimeSpan.Hours}H {purgeTimeSpan.Minutes}M";
        }
		
	private string GetWipeTime()
        {
            if (!_config.DisplayWipeTimer || _config.WipeTimerEpochTime <= 0)
                return "0";

            TimeSpan wipeTimeSpan = TimeSpan.FromSeconds(_config.WipeTimerEpochTime - DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            if (wipeTimeSpan.TotalSeconds <= 0)
                return "0";

            return $"{wipeTimeSpan.Days}D {wipeTimeSpan.Hours}H {wipeTimeSpan.Minutes}M";
        }
		
	private string GetBPTime()
        {
            if (!_config.DisplayBPTimer || _config.BPTimerEpochTime <= 0)
                return "0";

            TimeSpan bpTimeSpan = TimeSpan.FromSeconds(_config.BPTimerEpochTime - DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            if (bpTimeSpan.TotalSeconds <= 0)
                return "0";

            return $"{bpTimeSpan.Days}D {bpTimeSpan.Hours}H {bpTimeSpan.Minutes}M";
        }
		
	private string GetSTTime()
        {
            if (!_config.DisplaySTTimer || _config.STTimerEpochTime <= 0)
                return "0";

            TimeSpan stTimeSpan = TimeSpan.FromSeconds(_config.STTimerEpochTime - DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            if (stTimeSpan.TotalSeconds <= 0)
                return "0";

            return $"{stTimeSpan.Days}D {stTimeSpan.Hours}H {stTimeSpan.Minutes}M";
        }
    }
}

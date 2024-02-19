using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Event Messages", "Mabel", "1.0.1")]
    [Description("Sends Game Tip When Events Are Manually Triggered")]
    public class EventMessages : RustPlugin
    {
        static Configuration config;
        public class Configuration
        {
            [JsonProperty(PropertyName = "CargoPlane")]
            public bool CargoPlane;

            [JsonProperty(PropertyName = "CH47Helicopter")]
            public bool CH47Helicopter;

            [JsonProperty(PropertyName = "PatrolHelicopter")]
            public bool PatrolHelicopter;

            [JsonProperty(PropertyName = "CargoShip")]
            public bool CargoShip;

            [JsonProperty(PropertyName = "BradleyAPC")]
            public bool BradleyAPC;

            [JsonProperty(PropertyName = "OilRig Crates")]
            public bool HackableLockedCrate;

            [JsonProperty(PropertyName = "Excavator Activated")]
            public bool OnDieselEngineToggle;

            public static Configuration DefaultConfig()
            {
                return new Configuration
                {
                    CargoPlane = true,
                    CH47Helicopter = true,
                    PatrolHelicopter = true,
                    CargoShip = true,
                    BradleyAPC = true,
                    HackableLockedCrate = true,
                    OnDieselEngineToggle = true
                };
            }
        }

        protected override void LoadConfig()
        {
            base.LoadConfig();
            try
            {
                config = Config.ReadObject<Configuration>();
                if (config == null) LoadDefaultConfig();
                SaveConfig();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                PrintWarning("Creating new configuration file.....");
                LoadDefaultConfig();
            }
        }

        protected override void LoadDefaultConfig() => config = Configuration.DefaultConfig();
        protected override void SaveConfig() => Config.WriteObject(config);

        protected override void LoadDefaultMessages()
        {
            lang.RegisterMessages(new Dictionary<string, string>
            {
                ["BradleySpawn"] = "Bradley has spawned",
                ["CargoShipSpawn"] = "Cargo Ship has spawned",
                ["PatrolHeliSpawn"] = "Patrol Helicopter has spawned",
                ["CH47Spawn"] = "CH47 Helicopter has spawned",
                ["CargoPlaneSpawn"] = "Cargo Plane has spawned",
                ["ExcavatorActivated"] = "Excavator has been activated",
                ["LargeOilRigCrateSpawn"] = "Large Oil Rig is online",
                ["SmallOilRigCrateSpawn"] = "Small Oil Rig is online"
            }, this);
        }
		
        void OnEntitySpawned(BaseEntity entity)
        {
            if (entity == null || entity.IsDestroyed) return;

            if (entity is CargoPlane && config.CargoPlane)
            {
                SendToastToActivePlayers(lang.GetMessage("CargoPlaneSpawn", this));
            }
            else if (entity is CH47Helicopter && config.CH47Helicopter)
            {
                SendToastToActivePlayers(lang.GetMessage("CH47Spawn", this));
            }
            else if (entity is PatrolHelicopter && config.PatrolHelicopter)
            {
                SendToastToActivePlayers(lang.GetMessage("PatrolHeliSpawn", this));
            }
            else if (entity is CargoShip && config.CargoShip)
            {
                SendToastToActivePlayers(lang.GetMessage("CargoShipSpawn", this));
            }
            else if (entity is BradleyAPC && config.BradleyAPC)
            {
                SendToastToActivePlayers(lang.GetMessage("BradleySpawn", this));
            }
            else if (entity is HackableLockedCrate && config.HackableLockedCrate)
            {
                HackableLockedCrate crate = entity as HackableLockedCrate;
                if (crate != null && crate.ShortPrefabName == "codelockedhackablecrate_oilrig")
                {
                    if (IsLargeOilRigCrate(crate))
                    {
                        SendToastToActivePlayers(lang.GetMessage("LargeOilRigCrateSpawn", this));
                    }
                    else if (IsSmallOilRigCrate(crate))
                    {
                        SendToastToActivePlayers(lang.GetMessage("SmallOilRigCrateSpawn", this));
                    }
                }
            }
        }

        bool IsLargeOilRigCrate(HackableLockedCrate crate)
        {
            foreach (var monument in TerrainMeta.Path.Monuments)
            {
                if (monument == null) continue;
                if (Vector3.Distance(crate.transform.position, monument.transform.position) < 100)
                {
                    if (monument.displayPhrase.english.Contains("Large Oil Rig"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        bool IsSmallOilRigCrate(HackableLockedCrate crate)
        {
            foreach (var monument in TerrainMeta.Path.Monuments)
            {
                if (monument == null) continue;
                if (Vector3.Distance(crate.transform.position, monument.transform.position) < 100)
                {
                    if (monument.displayPhrase.english.Contains("Oil Rig") && !monument.displayPhrase.english.Contains("Large"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        void OnDieselEngineToggle(DieselEngine engine, BasePlayer player)
        {
            if (config.OnDieselEngineToggle)
            {
                SendToastToActivePlayers(lang.GetMessage("ExcavatorActivated", this));
            }
        }

        void SendToastToActivePlayers(string messageKey)
        {
            foreach (BasePlayer player in BasePlayer.activePlayerList)
            {
                if (player != null)
                {
                    player.ShowToast(GameTip.Styles.Red_Normal, lang.GetMessage(messageKey, this, player.UserIDString));
                }
            }
        }
    }
}

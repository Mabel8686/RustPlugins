/*
Copyright © 2024 Mabel

All rights reserved. This plugin is protected by copyright law.

You may not modify, redistribute, or resell this software without explicit written permission from the copyright owner.

For any support plaese message me directly via Discord `mabel8686`
*/
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
using Oxide.Core;
using Oxide.Core.Plugins;

namespace Oxide.Plugins
{
    [Info("NPC Gifts", "Mabel", "1.0.5")]
    [Description("Set a chance to spawn a gift for players for killing NPC")]
	
    class NPCGifts : RustPlugin
    {
        [PluginReference] private readonly Plugin SimpleLootTable;

        private List<Container> containerList;
        private List<BaseEntity> spawnedContainers = new List<BaseEntity>();
        private float cooldownDurationMinutes;
        private System.Random random;
        
        static Configuration config;
        public class Configuration
        {
            [JsonProperty(PropertyName = "Cooldown Settings")]
            public CooldownSettings CooldownSettings { get; set; }

            [JsonProperty(PropertyName = "Message Settings")]
            public MessageSettings MessageSettings { get; set; }

            [JsonProperty(PropertyName = "Container Settings")]
            public List<Container> Container { get; set; }

            public Core.VersionNumber Version { get; set; }

        }

        public class CooldownSettings
        {
            [JsonProperty(PropertyName = "Cooldown Duration Minutes")]
            public float cooldownDurationMinutes { get; set; }
        }

        public class MessageSettings
        {
            [JsonProperty(PropertyName = "Enable Chat Message")]
            public bool chat;

            [JsonProperty(PropertyName = "Enable Game Tip Message")]
            public bool toast;
        }

        public class Container
        {
            [JsonProperty("Enabled")]
            public bool Enabled { get; set; }

            [JsonProperty("Container Prefab")]
            public string ContainerType { get; set; }

            [JsonProperty("Spawn Chance")]
            public float SpawnChance { get; set; }

            [JsonProperty("Permission")]
            public string Permission { get; set; }

            [JsonProperty("Loot Table Name")]
            public string tableName { get; set; }

            [JsonProperty("Min Items")]
            public int minAmount { get; set; }

            [JsonProperty("Max Items")]
            public int maxAmount { get; set; }
        }

        public class VersionNumber
        {
            Core.VersionNumber Version { get; set; }
        }

        public static Configuration DefaultConfig()
        {
            return new Configuration()
            {
                CooldownSettings = new CooldownSettings
                {
                    cooldownDurationMinutes = 60f,
                },

                MessageSettings = new MessageSettings
                {
                    chat = false,
                    toast = true
                },

                Container = new List<Container>
                {
                    new Container
                    {
                        Enabled = true,
                        ContainerType = "assets/prefabs/misc/xmas/sleigh/presentdrop.prefab",
                        SpawnChance = 0.5f,
                        Permission = "npcgifts.example1",
                        tableName = "",
                        minAmount = 0,
                        maxAmount = 0,
                    },
                    new Container
                    {
                        Enabled = true,
                        ContainerType = "assets/prefabs/missions/portal/proceduraldungeon/xmastunnels/loot/xmastunnellootbox.prefab",
                        SpawnChance = 0.5f,
                        Permission = "npcgifts.example2",
                        tableName = "",
                        minAmount = 0,
                        maxAmount = 0,
                    },
                    new Container
                    {
                        Enabled = true,
                        ContainerType = "assets/prefabs/misc/xmas/giftbox/giftbox_loot.prefab",
                        SpawnChance = 0.5f,
                        Permission = "npcgifts.example3",
                        tableName = "",
                        minAmount = 0,
                        maxAmount = 0,
                    }
                },
                Version = new Core.VersionNumber()
            };
        }

        protected override void LoadConfig()
        {
            base.LoadConfig();
            try
            {
                config = Config.ReadObject<Configuration>();
                if (config == null) LoadDefaultConfig();
                SaveConfig();

                if (config.Version < Version)
                    UpdateConfig();

                Config.WriteObject(config, true);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                PrintWarning("Creating new configuration file....");
                LoadDefaultConfig();
            }
        }

        protected override void LoadDefaultConfig()
        {
            config = DefaultConfig();
            PrintWarning("Default configuration has been loaded....");
        }

        protected override void SaveConfig()
        {
            Config.WriteObject(config);
        }

        private void UpdateConfig()
        {
            PrintWarning("Config update detected! Updating config values...");

            Configuration baseConfig = DefaultConfig();

            if (config.Version < new Core.VersionNumber(1, 0, 1))
                config = baseConfig;

            config.Version = Version;

            PrintWarning("Config update completed!");
        }

        protected override void LoadDefaultMessages()
        {
            lang.RegisterMessages(new Dictionary<string, string>
            {
                ["ChatMessage"] = " :dance: Congratulations <color=#abf229>{0}</color> You Received A Gift From <color=#abf229>{1}</color> :dance:",
                ["GameTipMessage"] = "Congratulations <color=#abf229>{0}</color> You Received A Gift From <color=#abf229>{1}</color>"
            }, this);
        }

        void Init()
        {
            random = new System.Random();

            if (config?.CooldownSettings != null)
            {
                cooldownDurationMinutes = config.CooldownSettings.cooldownDurationMinutes;
            }

            containerList = config?.Container ?? new List<Container>();

            LoadCooldownData();
            CheckAndCreatePermissions();

            timer.Once(300f, () => CleanupContainers());
        }

        void Unload()
        {
            SaveCooldownData();
            CleanupContainers();
        }

        private void LoadCooldownData()
        {
            var data = Interface.Oxide.DataFileSystem.ReadObject<Dictionary<string, object>>("NPCGiftsData");
            if (data.TryGetValue("LastSpawnTimes", out var spawnTimes))
            {
                lastSpawnTimes = spawnTimes as Dictionary<ulong, DateTime> ?? new Dictionary<ulong, DateTime>();
            }
            else
            {
                lastSpawnTimes = new Dictionary<ulong, DateTime>();
            }
        }
        private void SaveCooldownData()
        {
            var data = new Dictionary<string, object>
            {
                ["LastSpawnTimes"] = lastSpawnTimes
            };
            Interface.Oxide.DataFileSystem.WriteObject("NPCGiftsData", data);
        }

        private void CheckAndCreatePermissions()
        {
            foreach (var containerData in containerList)
            {
                if (!string.IsNullOrEmpty(containerData.Permission))
                {
                    if (!permission.PermissionExists(containerData.Permission, this))
                    {
                        permission.RegisterPermission(containerData.Permission, this);
                    }
                }
            }
        }

        private void OnEntityDeath(BaseCombatEntity entity, HitInfo hitInfo)
        {
            if (entity == null || hitInfo == null) return;

            BaseEntity killer = hitInfo.Initiator;
            BasePlayer player = killer as BasePlayer;

            if (player != null && entity.GetComponent<HumanNPC>() != null)
            {
                if (!HasCooldown(player.userID))
                {
                    SpawnRandomContainer(entity.transform.position, (ulong)player.userID);
                    SetCooldown(player.userID);
                }
            }
        }

        private void SpawnRandomContainer(Vector3 position, ulong playerID)
        {
            if (containerList == null || containerList.Count == 0) return;

            random = new System.Random();
            BasePlayer player = BasePlayer.FindByID(playerID);

            foreach (var containerData in containerList.Where(c => c.Enabled))
            {
                if (random.NextDouble() <= containerData.SpawnChance)
                {
                    if (string.IsNullOrEmpty(containerData.Permission) || permission.UserHasPermission(playerID.ToString(), containerData.Permission))
                    {
                        BaseEntity container = GameManager.server.CreateEntity(containerData.ContainerType, position);

                        if (container != null)
                        {
                            container.Spawn();
                            spawnedContainers.Add(container);
                            GetSetItems(container, containerData.tableName, containerData.minAmount, containerData.maxAmount, 1f);

                            if (player != null)
                            {
                                if (config.MessageSettings.chat)
                                {
                                    var containerReceivedMessage = lang.GetMessage("ChatMessage", this);
                                    SendChatMessage(player, ReplacePlaceholders(player, containerReceivedMessage));
                                }
                                else if (config.MessageSettings.toast)
                                {
                                    var containerReceivedMessage = lang.GetMessage("GameTipMessage", this);
                                    player.ShowToast(GameTip.Styles.Blue_Long, ReplacePlaceholders(player, containerReceivedMessage));
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SendChatMessage(BasePlayer player, string message)
        {
            if (player == null || string.IsNullOrEmpty(message)) return;
            player.ChatMessage(message);
        }

        private Dictionary<ulong, DateTime> lastSpawnTimes = new Dictionary<ulong, DateTime>();

        private bool HasCooldown(ulong playerID)
        {
            if (lastSpawnTimes.ContainsKey(playerID))
            {
                var elapsedTime = DateTime.Now - lastSpawnTimes[playerID];
                if (elapsedTime.TotalMinutes >= cooldownDurationMinutes)
                {
                    lastSpawnTimes.Remove(playerID);
                    SaveCooldownData();
                    return false;
                }
                return true;
            }
            return false;
        }

        private void SetCooldown(ulong playerID)
        {
            if (lastSpawnTimes.ContainsKey(playerID))
            {
                lastSpawnTimes[playerID] = DateTime.Now;
            }
            else
            {
                lastSpawnTimes[playerID] = DateTime.Now;
            }
            SaveCooldownData();
        }

        private string ReplacePlaceholders(BasePlayer player, string message)
        {
            if (player == null || string.IsNullOrEmpty(message)) return message;

            string playerName = player.displayName;
            string serverName = ConVar.Server.hostname;

            return message.Replace("{0}", playerName).Replace("{1}", serverName);
        }

        public void GetSetItems(BaseEntity entity, string tableName, int minAmount, int maxAmount, float multiplier)
        {
            var storageContainer = entity as StorageContainer;
            if (storageContainer != null)
            {
                if (!string.IsNullOrEmpty(tableName))
                {
                    storageContainer.inventory?.Clear();
                    SimpleLootTable?.Call("GetSetItems", storageContainer, tableName, minAmount, maxAmount, multiplier);
                }
            }
        }

        private void CleanupContainers()
        {
            foreach (var container in spawnedContainers)
            {
                if (container != null && !container.IsDestroyed)
                {
                    container.Kill();
                }
            }
        }
        void OnNewSave()
        {
            lastSpawnTimes.Clear();
            SaveCooldownData();
        }
		
		[ConsoleCommand("npcgifts_wipe")]
        private void WipeData(ConsoleSystem.Arg arg)
        {
            if (arg.Args == null)
            {
                ResetData();
            }
        }

        private void ResetData()
        {
            lastSpawnTimes.Clear();
            SaveCooldownData();
            Puts("All player cooldowns have been reset...");
        }
    }
}

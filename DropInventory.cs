/*
Copyright © 2024 Mabel

All rights reserved. This plugin is protected by copyright law.

You may not modify, redistribute, or resell this software without explicit written permission from the copyright owner.

For any support please message me directly via Discord `mabel8686`
*/
﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("DropInventory", "Mabel", "0.0.3")]
    [Description("Drops a player's inventory on command")]
    class DropInventory : RustPlugin
    {
        private ConfigData configData;
        private Dictionary<string, string> messages;
        private Dictionary<ulong, float> cooldowns = new Dictionary<ulong, float>();
        private bool langLoaded = false;

        private Dictionary<string, string> defaultMessages = new Dictionary<string, string>
        {
            { "Cooldown", "Drop is on cooldown. Time remaining: {0:F1} seconds." },
            { "DropAll", "Your Main, Belt and Wear items have been dropped." },
            { "DropMain", "Your Main items have been dropped." },
            { "DropBelt", "Your Belt items have been dropped." },
            { "DropWear", "Your Wear items have been dropped." },
            { "DropMainBelt", "Your Main and Belt items have been dropped." },
            { "DropMainWear", "Your Main and Wear items have been dropped." },
            { "DropBeltWear", "Your Belt and Wear items have been dropped." },
            { "NoContainers", "No containers are enabled for dropping items." },
            { "AdminDroppedItems", "Your items were dropped by an admin." }
        };

        void Init()
        {
            LoadConfig();
            LoadDefaultMessages();
            permission.RegisterPermission("dropinventory.use", this);
            permission.RegisterPermission("dropinventory.cooldown.bypass", this);
            permission.RegisterPermission("dropinventory.admin", this);
            cmd.AddChatCommand(configData.PlayerChatCommand, this, "DropInventoryCommand");
            cmd.AddChatCommand(configData.AdminChatCommand, this, "DropInventoryAdminCommand");
            cmd.AddConsoleCommand("drop.admin", this, "DropInventoryAdminConsoleCommand");
        }

        protected override void LoadDefaultConfig()
        {
            PrintWarning("Creating a new configuration file for DropInventory...");
            configData = new ConfigData();
            SaveConfig();
        }

        private void LoadConfig()
        {
            configData = Config.ReadObject<ConfigData>();
        }

        private void SaveConfig()
        {
            Config.WriteObject(configData, true);
        }

        private void LoadDefaultMessages()
        {
            if (langLoaded)
                return;

            var sortedMessages = defaultMessages.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

            messages = new Dictionary<string, string>();
            foreach (var kvp in sortedMessages)
            {
                if (!messages.ContainsKey(kvp.Key))
                    messages.Add(kvp.Key, kvp.Value);
            }

            lang.RegisterMessages(messages, this, "en");
            langLoaded = true;
            //PrintWarning("Language file has been updated...");
        }

        private Vector3 GetRandomVelocity()
        {
            return new Vector3(UnityEngine.Random.Range(-configData.ItemVelocity, configData.ItemVelocity), configData.ItemVelocity, UnityEngine.Random.Range(-configData.ItemVelocity, configData.ItemVelocity));
        }

        private bool CanUseCommand(BasePlayer player)
        {
            if (permission.UserHasPermission(player.UserIDString, "dropinventory.cooldown.bypass"))
                return true;

            ulong userId = player.userID;
            float currentTime = Time.realtimeSinceStartup;

            float lastUsageTime;
            if (cooldowns.TryGetValue(userId, out lastUsageTime))
            {
                float timeRemaining = lastUsageTime + configData.CommandCooldown - currentTime;
                if (timeRemaining > 0f)
                {
                    SendReply(player, configData.ChatPrefix + " " + string.Format(GetMessage("Cooldown", player.UserIDString), timeRemaining));
                    return false;
                }
            }

            cooldowns[userId] = currentTime;
            return true;
        }

        [ChatCommand("drop")]
        void DropInventoryCommand(BasePlayer player, string command, string[] args)
        {
            if (!permission.UserHasPermission(player.UserIDString, "dropinventory.use"))
            {
                SendReply(player, configData.ChatPrefix + " You don't have permission to use this command.");
                return;
            }

            if (!CanUseCommand(player))
                return;

            if (!string.IsNullOrEmpty(configData.SoundPath))
            {
                Effect.server.Run(configData.SoundPath, player.transform.position);
            }

            DropPlayerInventory(player);
            DropPlayerBeltItems(player);
            DropPlayerWearItems(player);

            SendReply(player, configData.ChatPrefix + " " + GetDropMessage(player));
        }

        [ChatCommand("drop.admin")]
        void DropInventoryAdminCommand(BasePlayer player, string command, string[] args)
        {
            if (!permission.UserHasPermission(player.UserIDString, "dropinventory.admin"))
            {
                SendReply(player, configData.ChatPrefix + " You don't have permission to use this command.");
                return;
            }

            if (args.Length < 1)
            {
                SendReply(player, configData.ChatPrefix + " Usage: /drop.admin <playername or steamid>");
                return;
            }

            BasePlayer targetPlayer = FindPlayer(args[0]);
            if (targetPlayer == null)
            {
                SendReply(player, configData.ChatPrefix + " Player not found or multiple players found matching the name or ID provided.");
                return;
            }

            if (!CanUseCommand(targetPlayer))
                return;

            if (!string.IsNullOrEmpty(configData.SoundPath))
            {
                Effect.server.Run(configData.SoundPath, targetPlayer.transform.position);
            }

            DropPlayerInventory(targetPlayer);
            DropPlayerBeltItems(targetPlayer);
            DropPlayerWearItems(targetPlayer);

            SendReply(player, configData.ChatPrefix + $" You dropped (Player: <color=#FF0000>{targetPlayer.displayName}</color>) Items.");
            SendReply(targetPlayer, configData.ChatPrefix + " " + GetMessage("AdminDroppedItems", targetPlayer.UserIDString));
        }

        [ConsoleCommand("drop.admin")]
        void DropInventoryAdminConsoleCommand(ConsoleSystem.Arg arg)
        {
            if (arg?.Args == null || arg.Args.Length < 1)
            {
                SendReply(arg, "Usage: drop.admin <playername or steamid>");
                return;
            }

            BasePlayer player = BasePlayer.Find(arg.Args[0]);
            if (player == null)
            {
                SendReply(arg, "Player not found or multiple players found matching the name or ID provided.");
                return;
            }

            if (!CanUseCommand(player))
                return;

            if (!string.IsNullOrEmpty(configData.SoundPath))
            {
                Effect.server.Run(configData.SoundPath, player.transform.position);
            }

            DropPlayerInventory(player);
            DropPlayerBeltItems(player);
            DropPlayerWearItems(player);

            SendReply(arg, $"You dropped (Player: {player.displayName}) Items.");
            SendReply(player, configData.ChatPrefix + " " + GetMessage("AdminDroppedItems", player.UserIDString));
        }

        private BasePlayer FindPlayer(string target)
        {
            if (target.Length == 17 && target.All(char.IsDigit))
            {
                ulong steamId;
                if (ulong.TryParse(target, out steamId))
                {
                    return BasePlayer.FindByID(steamId);
                }
            }

            var players = BasePlayer.activePlayerList.ToList().FindAll(p => p.displayName.ToLower().Contains(target.ToLower()));
            if (players.Count == 1)
            {
                return players[0];
            }
            return null;
        }

        private string GetDropMessage(BasePlayer player)
        {
            bool mainContainerEnabled = configData.DropMainContainer;
            bool beltContainerEnabled = configData.DropBeltContainer;
            bool wearContainerEnabled = configData.DropWearContainer;

            if (!mainContainerEnabled && !beltContainerEnabled && !wearContainerEnabled)
                return GetMessage("NoContainers", player.UserIDString);
            else if (mainContainerEnabled && beltContainerEnabled && wearContainerEnabled)
                return GetMessage("DropAll", player.UserIDString);
            else if (mainContainerEnabled && beltContainerEnabled)
                return GetMessage("DropMainBelt", player.UserIDString);
            else if (mainContainerEnabled && wearContainerEnabled)
                return GetMessage("DropMainWear", player.UserIDString);
            else if (beltContainerEnabled && wearContainerEnabled)
                return GetMessage("DropBeltWear", player.UserIDString);
            else if (mainContainerEnabled)
                return GetMessage("DropMain", player.UserIDString);
            else if (beltContainerEnabled)
                return GetMessage("DropBelt", player.UserIDString);
            else if (wearContainerEnabled)
                return GetMessage("DropWear", player.UserIDString);

            return "Unknown drop combination.";
        }

        private void DropPlayerInventory(BasePlayer player)
        {
            if (configData.DropMainContainer)
            {
                List<Item> itemsToDrop = new List<Item>(player.inventory.containerMain.itemList);
                foreach (var item in itemsToDrop)
                {
                    var randomVelocity = GetRandomVelocity();
                    item.Drop(player.GetDropPosition(), randomVelocity);
                    item.RemoveFromContainer();
                }
            }
        }

        private void DropPlayerBeltItems(BasePlayer player)
        {
            if (configData.DropBeltContainer)
            {
                List<Item> itemsToDrop = new List<Item>(player.inventory.containerBelt.itemList);
                foreach (var item in itemsToDrop)
                {
                    var randomVelocity = GetRandomVelocity();
                    item.Drop(player.GetDropPosition(), randomVelocity);
                    item.RemoveFromContainer();
                }
            }
        }

        private void DropPlayerWearItems(BasePlayer player)
        {
            if (configData.DropWearContainer)
            {
                List<Item> itemsToDrop = new List<Item>(player.inventory.containerWear.itemList);
                foreach (var item in itemsToDrop)
                {
                    var randomVelocity = GetRandomVelocity();
                    item.Drop(player.GetDropPosition(), randomVelocity);
                    item.RemoveFromContainer();
                }
            }
        }

        private string GetMessage(string key, string userId)
        {
            string message;
            if (messages.TryGetValue(key, out message))
                return message;
            return key;
        }

        class ConfigData
        {
            public string ChatPrefix { get; set; } = "<color=#FF0000>[DropInv]</color> ";
            public string SoundPath { get; set; } = "assets/bundled/prefabs/fx/gestures/drink_vomit.prefab";
            public float ItemVelocity { get; set; } = 5.0f;
            public float CommandCooldown { get; set; } = 60f;
            public bool DropMainContainer { get; set; } = true;
            public bool DropBeltContainer { get; set; } = true;
            public bool DropWearContainer { get; set; } = true;
            public string PlayerChatCommand { get; set; } = "drop";
            public string AdminChatCommand { get; set; } = "drop.admin";
        }
    }
}

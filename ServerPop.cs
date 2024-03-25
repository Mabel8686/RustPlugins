using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Server Pop", "Mabel", "1.0.6")]
    [Description("Show server pop in chat with !pop trigger.")]
    public class ServerPop : RustPlugin
    {
        static Configuration config;
        static Dictionary<ulong, DateTime> cooldowns = new Dictionary<ulong, DateTime>();

        public class Configuration
        {
            [JsonProperty(PropertyName = "Chat Prefix")]
            public string chatPrefix { get; set; }

            [JsonProperty(PropertyName = "Value Color (HEX)")]
            public string valueColor { get; set; }

            [JsonProperty(PropertyName = "Chat Icon SteamID")]
            public ulong chatSteamID { get; set; } = 76561199216745239;

            [JsonProperty(PropertyName = "GlobalResponse (true = global response, false = player response)")]
            public bool globalResponse { get; set; }

            [JsonProperty(PropertyName = "Show Online Players")]
            public bool showOnlinePlayers { get; set; }

            [JsonProperty(PropertyName = "Show Sleeping Players")]
            public bool showSleepingPlayers { get; set; }

            [JsonProperty(PropertyName = "Show Joining Players")]
            public bool showJoiningPlayers { get; set; }

            [JsonProperty(PropertyName = "Show Queued Players")]
            public bool showQueuedPlayers { get; set; }

            [JsonProperty(PropertyName = "Show Pop On Connect")]
            public bool showPopOnConnect { get; set; }

            [JsonProperty(PropertyName = "Show Welcome Message")]
            public bool showWelcomeMessage { get; set; }

            [JsonProperty(PropertyName = "Cooldown (seconds)")]
            public int cooldownSeconds { get; set; } = 60;

            public static Configuration DefaultConfig()
            {
                return new Configuration
                {
                    chatPrefix = "<size=16><color=#FFA500>| Server Pop |</color></size>",
                    valueColor = "#FFA500",
                    chatSteamID = 76561199216745239,
                    globalResponse = true,
                    showOnlinePlayers = true,
                    showSleepingPlayers = true,
                    showJoiningPlayers = true,
                    showQueuedPlayers = true,
                    showPopOnConnect = false,
                    showWelcomeMessage = false,
                    cooldownSeconds = 60
                };
            }
        }

        protected override void LoadConfig()
        {
            base.LoadConfig();
            try
            {
				LoadDefaultConfig();
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
                ["Online"] = "{0} / {1} players online",
                ["Sleeping"] = "{0} players sleeping",
                ["Joining"] = "{0} players joining",
                ["Queued"] = "{0} players queued",
                ["WelcomeMessage"] = "Welcome to the server {0}!",
                ["CooldownMessage"] = "You must wait {0} seconds before using this command again."
            }, this);
        }

        private void OnPlayerConnected(BasePlayer player)
        {
            if (config.showWelcomeMessage)
            {
                string welcomeMessage = lang.GetMessage("WelcomeMessage", this);

                if (!string.IsNullOrEmpty(welcomeMessage))
                {
                    welcomeMessage = string.Format(welcomeMessage, player.displayName);

                    Player.Message(player, welcomeMessage, config.chatSteamID);
                }
            }

            if (config.showPopOnConnect)
            {
                SendMessage(player);
            }
        }

        private void OnPlayerChat(BasePlayer player, string message, ConVar.Chat.ChatChannel channel)
        {
            if (message.ToLower() == "!pop")
            {
                if (CanUseTrigger(player.userID))
                {
                    SendMessage(player);
                    cooldowns[player.userID] = DateTime.Now.AddSeconds(config.cooldownSeconds);
                }
                else
                {
                    TimeSpan remainingCooldown = cooldowns[player.userID] - DateTime.Now;
                    string cooldownMessage = lang.GetMessage("CooldownMessage", this);
                    cooldownMessage = string.Format(cooldownMessage, ApplyColor(Math.Round(remainingCooldown.TotalSeconds).ToString(), config.valueColor));
                    Player.Message(player, cooldownMessage, config.chatSteamID);
                }
            }
        }

        private bool CanUseTrigger(ulong userID)
        {
            if (!cooldowns.ContainsKey(userID))
                return true;

            return cooldowns[userID] <= DateTime.Now;
        }

        private void SendMessage(BasePlayer player)
        {
            StringBuilder popMessage = new StringBuilder($"{config.chatPrefix}\n\n");

            if (config.showOnlinePlayers)
            {
                string onlinePlayersText = $"{BasePlayer.activePlayerList.Count} / {ConVar.Server.maxplayers}";
                string onlineMessage = $"{lang.GetMessage("Online", this)}";
                onlineMessage = string.Format(onlineMessage, ApplyColor(BasePlayer.activePlayerList.Count.ToString(), config.valueColor), ApplyColor(ConVar.Server.maxplayers.ToString(), config.valueColor));
                popMessage.AppendLine($"{onlineMessage}\n");
            }

            if (config.showSleepingPlayers)
            {
                string sleepingPlayersText = BasePlayer.sleepingPlayerList.Count.ToString();
                string sleepingMessage = $"{lang.GetMessage("Sleeping", this)}";
                sleepingMessage = string.Format(sleepingMessage, ApplyColor(BasePlayer.sleepingPlayerList.Count.ToString(), config.valueColor));
                popMessage.AppendLine($"{sleepingMessage}\n");
            }

            if (config.showJoiningPlayers)
            {
                string joiningPlayersText = ServerMgr.Instance.connectionQueue.Joining.ToString();
                string joiningMessage = $"{lang.GetMessage("Joining", this)}";
                joiningMessage = string.Format(joiningMessage, ApplyColor(ServerMgr.Instance.connectionQueue.Joining.ToString(), config.valueColor));
                popMessage.AppendLine($"{joiningMessage}\n");
            }

            if (config.showQueuedPlayers)
            {
                string queuedPlayersText = ServerMgr.Instance.connectionQueue.Queued.ToString();
                string queuedMessage = $"{lang.GetMessage("Queued", this)}";
                queuedMessage = string.Format(queuedMessage, ApplyColor(ServerMgr.Instance.connectionQueue.Queued.ToString(), config.valueColor));
                popMessage.AppendLine($"{queuedMessage}\n");
            }

            if (config.globalResponse)
            {
                Server.Broadcast(popMessage.ToString(), null, config.chatSteamID);
            }
            else
            {
                Player.Message(player, popMessage.ToString(), null, config.chatSteamID);
            }
        }

        private string ApplyColor(string text, string hexColor)
        {
            return $"<color={hexColor}>{text}</color>";
        }
    }
}

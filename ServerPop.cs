using System.Text;

namespace Oxide.Plugins
{
    [Info("ServerPop", "Mabel", "1.0.3"), Description("Show server pop in chat with !pop trigger.")]
    class ServerPop : RustPlugin
    {
        private bool showOnlinePlayers;
        private bool showSleepingPlayers;
        private bool showJoiningPlayers;
        private bool showQueuedPlayers;

        private string chatPrefix = "<size=16><color=#FFA500>| ServerPop |</color></size>";
        private string valueColorHex = "#FFA500";

        private ulong customSteamID = 0;

        protected override void LoadDefaultConfig()
        {
            Config.Clear();
            Config["ShowOnlinePlayers"] = true;
            Config["ShowSleepingPlayers"] = true;
            Config["ShowJoiningPlayers"] = true;
            Config["ShowQueuedPlayers"] = true;
            Config["ChatPrefix"] = chatPrefix;
            Config["ValueColorHex"] = valueColorHex;
            Config["ChatIconSteamID"] = customSteamID;
            SaveConfig();
        }

        void Init()
        {
            LoadConfigValues();
        }

        void LoadConfigValues()
        {
            showOnlinePlayers = Config.Get<bool>("ShowOnlinePlayers");
            showSleepingPlayers = Config.Get<bool>("ShowSleepingPlayers");
            showJoiningPlayers = Config.Get<bool>("ShowJoiningPlayers");
            showQueuedPlayers = Config.Get<bool>("ShowQueuedPlayers");

            chatPrefix = Config.Get<string>("ChatPrefix");
            valueColorHex = Config.Get<string>("ValueColorHex");
            customSteamID = Config.Get<ulong>("ChatIconSteamID");
        }

        private void OnPlayerChat(BasePlayer player, string message, ConVar.Chat.ChatChannel channel)
        {
            if (message == "!pop")
            {
                SendMessage(player);
            }
        }

        private void SendMessage(BasePlayer player)
        {
            StringBuilder popMessage = new StringBuilder($"{chatPrefix}\n\n");

            if (showOnlinePlayers)
                popMessage.AppendLine($"{ColorizeText($"{BasePlayer.activePlayerList.Count} / {ConVar.Server.maxplayers}", valueColorHex)} player's online\n");

            if (showSleepingPlayers)
                popMessage.AppendLine($"{ColorizeText(BasePlayer.sleepingPlayerList.Count.ToString(), valueColorHex)} player's sleeping\n");

            if (showJoiningPlayers)
                popMessage.AppendLine($"{ColorizeText(ServerMgr.Instance.connectionQueue.Joining.ToString(), valueColorHex)} player's joining\n");

            if (showQueuedPlayers)
                popMessage.AppendLine($"{ColorizeText(ServerMgr.Instance.connectionQueue.Queued.ToString(), valueColorHex)} player's queued\n");

            Server.Broadcast(popMessage.ToString(), null, customSteamID);
        }

        string ColorizeText(string text, string hexColor)
        {
            return $"<color={hexColor}>{text}</color>";
        }
    }
}

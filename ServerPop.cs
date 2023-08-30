namespace Oxide.Plugins
{
    [Info("ServerPop", "Mabel", "1.0.2"), Description("Show server pop in chat with !pop trigger.")]
    class ServerPop : CovalencePlugin
    {
        private bool showOnlinePlayers;
        private bool showSleepingPlayers;
        private bool showJoiningPlayers;
        private bool showQueuedPlayers;

        private string chatPrefix = "<size=16><color=#FFA500>| ServerPop |</color></size>";
        private string valueColorHex = "#FFA500";

        protected override void LoadDefaultConfig()
        {
            Config["ShowOnlinePlayers"] = true;
            Config["ShowSleepingPlayers"] = true;
            Config["ShowJoiningPlayers"] = true;
            Config["ShowQueuedPlayers"] = true;
            Config["ChatPrefix"] = chatPrefix;
            Config["ValueColorHex"] = valueColorHex;
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
        }

        void OnPlayerChat(BasePlayer player, string message)
        {
            if (message == "!pop")
            {
                timer.Once(0.1f, () =>
                {
                    string popMessage = $"{chatPrefix}\n\n";

                    if (showOnlinePlayers)
                        popMessage += $"{ColorizeText($"{BasePlayer.activePlayerList.Count} / {ConVar.Server.maxplayers}", valueColorHex)} player's online\n\n";

                    if (showSleepingPlayers)
                        popMessage += $"{ColorizeText(BasePlayer.sleepingPlayerList.Count.ToString(), valueColorHex)} player's sleeping\n\n";

                    if (showJoiningPlayers)
                        popMessage += $"{ColorizeText(ServerMgr.Instance.connectionQueue.Joining.ToString(), valueColorHex)} player's joining\n\n";

                    if (showQueuedPlayers)
                        popMessage += $"{ColorizeText(ServerMgr.Instance.connectionQueue.Queued.ToString(), valueColorHex)} player's queued";

                    player.ChatMessage(popMessage);
                });
            }
        }

        string ColorizeText(string text, string hexColor)
        {
            return $"<color={hexColor}>{text}</color>";
        }
    }
}
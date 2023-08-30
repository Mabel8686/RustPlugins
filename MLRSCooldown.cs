using System.Collections.Generic;
using Oxide.Core;

namespace Oxide.Plugins
{
    [Info("MLRSCooldown", "Mabel", "1.0.0")]
    [Description("Prevents players from mounting the MLRS launcher if they are on cooldown.")]
    class MLRSCooldown : RustPlugin
    {
        private Dictionary<ulong, float> cooldowns = new Dictionary<ulong, float>();
        private float realtimeSinceStartup;
        private const float CooldownTime = 1200.0f;

        void Init()
        {
            permission.RegisterPermission("mlrscooldown.bypass", this);
        }

        void OnMlrsFired(MLRS mlrs, BasePlayer player)
        {
            if (mlrs == null || player == null || !player.IsConnected || !player.IsAlive())
                return;

            if (!HasPermission(player, "mlrscooldown.bypass"))
            {
                if (!IsOnCooldown(player.userID))
                {
                    ApplyCooldown(player.userID);
                }
            }
        }

        object CanMountEntity(BasePlayer player, MLRS mlrs)
        {
            if (player == null || mlrs == null || !player.IsConnected || !player.IsAlive())
                return null;

            if (!HasPermission(player, "mlrscooldown.bypass") && IsOnCooldown(player.userID))
            {
                SendReply(player, $" <color=#FF0000>{player.displayName}</color> You can only use the MLRS launcher once every <color=#FF0000>20</color> minutes.");
                return false;
            }

            return null;
        }

        private bool HasPermission(BasePlayer player, string permissionName)
        {
            return permission.UserHasPermission(player.UserIDString, permissionName);
        }

        private bool IsOnCooldown(ulong playerId)
        {
            float lastFiredTime;
            if (cooldowns.TryGetValue(playerId, out lastFiredTime))
            {
                float currentTime = realtimeSinceStartup;
                return currentTime - lastFiredTime < CooldownTime;
            }
            return false;
        }

        private void ApplyCooldown(ulong playerId)
        {
            float currentTime = realtimeSinceStartup;
            cooldowns[playerId] = currentTime;

            timer.Once(CooldownTime, () =>
            {
                cooldowns.Remove(playerId);
            });
        }

        void OnServerInitialized()
        {
            LoadData();
        }

        void OnServerSave()
        {
            SaveData();
        }

        void LoadData()
        {
            cooldowns = Interface.GetMod().DataFileSystem.ReadObject<Dictionary<ulong, float>>("MLRSCooldown");
        }

        void SaveData()
        {
            Interface.GetMod().DataFileSystem.WriteObject("MLRSCooldown", cooldowns);
        }
    }
}
using System;
using Oxide.Core;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("MLRSOwnershipControl", "Mabel", "1.0.0")]
    [Description("Prevents players who are not the owner or part of the same team as the owner of the MLRS from mounting it.")]
    class MLRSOwnershipControl : RustPlugin
    {
        void Init()
        {
            permission.RegisterPermission("mlrsownershipcontrol.bypass", this);
        }

        object CanMountEntity(BasePlayer player, MLRS mlrs)
        {
            if (player == null || mlrs == null || !player.IsConnected || !player.IsAlive())
                return null;

            if (!IsOwner(player, mlrs) && !IsTeamMember(player, mlrs))
            {
                if (!HasPermission(player, "mlrsownershipcontrol.bypass"))
                {
                    SendReply(player, " You can only mount your own/teams <color=red>MLRS</color>");
                    return false;
                }
            }

            return null;
        }

        private bool HasPermission(BasePlayer player, string permissionName)
        {
            return permission.UserHasPermission(player.UserIDString, permissionName);
        }

        private bool IsOwner(BasePlayer player, MLRS mlrs)
        {
            ulong ownerID = mlrs.OwnerID; // Get the owner ID of the MLRS

            // Compare the owner ID with the player's ID
            return ownerID == player.userID;
        }

        private bool IsTeamMember(BasePlayer player, MLRS mlrs)
        {
            ulong ownerID = mlrs.OwnerID; // Get the owner ID of the MLRS
            var ownerPlayer = BasePlayer.FindByID(ownerID);

            if (ownerPlayer != null)
            {
                var ownerTeam = RelationshipManager.ServerInstance.FindPlayersTeam(ownerPlayer.userID);
                var playerTeam = RelationshipManager.ServerInstance.FindPlayersTeam(player.userID);

                // Check if the owner and player are in the same team
                return ownerTeam != null && playerTeam != null && ownerTeam.teamID == playerTeam.teamID;
            }

            return false;
        }
    }
}
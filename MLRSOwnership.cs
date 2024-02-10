using System;
using Oxide.Core;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("MLRSOwnership", "Mabel", "1.0.1")]
    [Description("Prevents players who are not the owner or part of the same team as the owner of the MLRS from mounting it.")]
    class MLRSOwnership : RustPlugin
    {
        void Init()
        {
            permission.RegisterPermission("mlrsownership.bypass", this);
        }

        object CanMountEntity(BasePlayer player, MLRS mlrs)
        {
            if (player == null || mlrs == null || !player.IsConnected || !player.IsAlive())
                return null;

            bool hasRaidPermission = HasPermission(player, "raidablebases.expertraid");
			
			if (mlrs.OwnerID == 0)
			{
				if (!hasRaidPermission)
				{
					SendReply(player, $" <color=#4A95CC>{player.displayName}</color> you need to complete <color=#4A95CC>Expert Raids</color> to use the <color=red>MLRS</color>.");
				    return false;
				}
			  return null;
			}
			
            bool isOwnerOrTeam = IsOwner(player, mlrs) || IsTeamMember(player, mlrs);

            if (!isOwnerOrTeam)
            {
                SendReply(player, $" <color=#4A95CC>{player.displayName}</color> you can only mount your own/teams <color=red>MLRS</color>.");
                return false;
            }

            if (!hasRaidPermission)
            {
                SendReply(player, $" <color=#4A95CC>{player.displayName}</color> you need to complete <color=#4A95CC>Expert Raids</color> to use the <color=red>MLRS</color>.");
                return false;
            }

            return null;
        }

        private bool HasPermission(BasePlayer player, string permissionName)
        {
            return permission.UserHasPermission(player.UserIDString, permissionName);
        }

        private bool IsOwner(BasePlayer player, MLRS mlrs)
        {
            ulong ownerID = mlrs.OwnerID;
            return ownerID == player.userID;
        }

        private bool IsTeamMember(BasePlayer player, MLRS mlrs)
        {
            ulong ownerID = mlrs.OwnerID;
            var ownerPlayer = BasePlayer.FindByID(ownerID);

            if (ownerPlayer != null)
            {
                var ownerTeam = RelationshipManager.ServerInstance.FindPlayersTeam(ownerPlayer.userID);
                var playerTeam = RelationshipManager.ServerInstance.FindPlayersTeam(player.userID);

                return ownerTeam != null && playerTeam != null && ownerTeam.teamID == playerTeam.teamID;
            }

            return false;
        }
    }
}

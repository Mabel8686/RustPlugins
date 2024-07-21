/*
Copyright © 2024 Mabel

All rights reserved. This plugin is protected by copyright law.

You may not modify, redistribute, or resell this software without explicit written permission from the copyright owner.

For any support plaese message me directly via Discord `mabel8686`
*/
using System.Collections.Generic;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Quarry Notification", "Mabel", "1.0.0")]
    [Description("Broadcasts to the server when a player starts quarry, pump jack, or excavator")]

    public class QuarryNotification : RustPlugin
    {
        private readonly Dictionary<string, string> objectNames = new Dictionary<string, string>
        {
            { "miningquarry_static", "Mining Quarry" },
            { "pumpjack-static", "Pump Jack" }
        };

        private HashSet<MiningQuarry> activeQuarries = new HashSet<MiningQuarry>();

        void OnQuarryToggled(MiningQuarry quarry, BasePlayer player)
        {
            if (quarry == null || player == null) return;

            string playerName = player.displayName;
            Vector3 quarryPosition = quarry.transform.position;
            string gridLocation = PositionToGridCoord(quarryPosition);
            string prefabName = quarry.ShortPrefabName;
            string objectName = objectNames.ContainsKey(prefabName) ? objectNames[prefabName] : "Unknown Object";

            if (quarry.IsOn())
            {
                if (!activeQuarries.Contains(quarry))
                {
                    activeQuarries.Add(quarry);
                    Server.Broadcast($" <color=green>{playerName}</color> has activated the <color=red>{objectName}</color> at <color=green>{gridLocation}</color>");
                }
            }
            else
            {
                if (activeQuarries.Contains(quarry))
                {
                    activeQuarries.Remove(quarry);
                }
            }
        }

        void OnDieselEngineToggle(DieselEngine engine, BasePlayer player)
        {
            if (engine == null || player == null) return;

            string playerName = player.displayName;

            Server.Broadcast($" <color=green>{playerName}</color> has activated the <color=red>Excavator</color>");
        }

        private string PositionToGridCoord(Vector3 position)
        {
            return PhoneController.PositionToGridCoord(position);
        }
    }
}
/*
Copyright © 2024 Mabel

All rights reserved. This plugin is protected by copyright law.

You may not modify, redistribute, or resell this software without explicit written permission from the copyright owner.

For any support please message me directly via Discord `mabel8686`
*/
namespace Oxide.Plugins
{
    [Info("Kill Cargo Ship", "Mabel", "1.0.0")]
    [Description("Temp Fix For Bugged Cargo Ship After Server Restart")]
    public class KillCargoShip : RustPlugin
    {
        private void OnServerInitialized()
        {
            int cargoShipsKilled = 0;

            foreach (BaseEntity entity in BaseNetworkable.serverEntities)
            {
                if (entity is CargoShip)
                {
                    entity.Kill();
                    cargoShipsKilled++;
                }
            }

            if (cargoShipsKilled > 0)
            {
                Puts($"Killed {cargoShipsKilled} bugged cargo ship(s) after server restart.");
            }
            else
            {
                Puts("No bugged cargo ships found after server restart.");
            }
        }
    }
}

/*
Copyright © 2024 Mabel

All rights reserved. This plugin is protected by copyright law.

You may not modify, redistribute, or resell this software without explicit written permission from the copyright owner.

For any support plaese message me directly via Discord `mabel8686`
*/
namespace Oxide.Plugins
{
    [Info("Blackjack Fix", "Mabel", "1.0.2")]
    [Description("Fixes Blackjack spawned in wrong rotation")]
    
    public class BlackjackFix : RustPlugin
    {
        private void OnEntitySpawned(BlackjackMachine entity)
        {
            if (entity.OwnerID == 0 || entity.OwnerID == 86753095)
            {
                return;
            }
            var transform = entity.transform;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }
    }
}

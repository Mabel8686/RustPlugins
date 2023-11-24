namespace Oxide.Plugins
{
    [Info("No Lock-On Rocket", "Mabel", "1.0.0")]
    class NoLockOnRocket : RustPlugin
    {
        private const string RestrictedItemShortname = "ammo.rocket.smoke";

        private void Init()
        {
            permission.RegisterPermission("nolockonrocket.bypass", this);
        }

        private void OnItemAddedToContainer(ItemContainer container, Item item)
        {
            BasePlayer player = container.playerOwner as BasePlayer;
            if (player != null)
            {
                HandleRestrictedItem(item, player);
            }
        }

        private void HandleRestrictedItem(Item item, BasePlayer player)
        {
            if (item.info.shortname == RestrictedItemShortname)
            {
                if (!permission.UserHasPermission(player.UserIDString, "nolockonrocket.bypass"))
                {
                    int stackSize = item.amount;

                    item.Remove();

                    Item droppedItem = ItemManager.CreateByItemID(item.info.itemid, stackSize);
                    droppedItem.Drop(player.GetDropPosition(), player.GetDropVelocity());

                    player.ChatMessage(" :exclamation: You have not unlocked <color=red>Lock On Rockets</color> perk :exclamation:");
                }
            }
        }
    }
}

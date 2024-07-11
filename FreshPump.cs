namespace Oxide.Plugins
{
    [Info("Fresh Pump", "Mabel", "1.0.0")]
    [Description("Converts Salt Water --> Fresh Water in water pumps")]

    public class FreshPump : RustPlugin
    {
        private void OnWaterCollect(WaterPump instance, ItemDefinition atPoint)
        {
            if (atPoint.itemid == -277057363)
            {
                var itemAmount = 85;
                var freshWater = ItemManager.CreateByItemID(-1779180711, itemAmount, 0uL);

                if (!instance.inventory.GiveItem(freshWater))
                {
                    freshWater.Drop(instance.transform.position, instance.transform.forward);
                }
            }
        }
    }
}
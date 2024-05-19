namespace Oxide.Plugins
{
    [Info("Disable Recycle Efficiency", "Mabel", "1.0.1")]
    [Description("Disables the recycle efficiency and returns recyclers back to green.")]
    public class DisableRecycleEfficiency : RustPlugin
    {
        private const float DisabledRecycleEfficiency = 0.5f;

        private void OnServerInitialized()
        {
            foreach (var entity in BaseNetworkable.serverEntities)
            {
                if (entity is Recycler recycler)
                {
                    DisableEfficiency(recycler);
                    recycler.SetFlag(BaseEntity.Flags.Reserved9, false, false);
                }
            }
        }

        private void OnEntitySpawned(BaseNetworkable entity)
        {
            if (entity is Recycler recycler)
            {
                DisableEfficiency(recycler);
                recycler.SetFlag(BaseEntity.Flags.Reserved9, false, false);
            }
        }

        private void DisableEfficiency(Recycler recycler)
        {
            recycler.recycleEfficiency = DisabledRecycleEfficiency;
            recycler.safezoneRecycleEfficiency = DisabledRecycleEfficiency;
            recycler.radtownRecycleEfficiency = DisabledRecycleEfficiency;
        }
    }
}

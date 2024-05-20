using System.Collections.Generic;

namespace Oxide.Plugins
{
    [Info("Disable Recycle Efficiency", "Mabel", "1.0.2")]
    [Description("Disables the recycle efficiency and returns recyclers back to green.")]

    public class DisableRecycleEfficiency : RustPlugin
    {
        private const float DisabledRecycleEfficiency = 0.5f;

        private Dictionary<ulong, bool> recyclerOriginalStates = new Dictionary<ulong, bool>();

        private void OnServerInitialized()
        {
            foreach (var entity in BaseNetworkable.serverEntities)
            {
                if (entity is Recycler recycler)
                {
                    recyclerOriginalStates[recycler.net.ID.Value] = recycler.HasFlag(BaseEntity.Flags.Reserved9);
                    DisableEfficiency(recycler);
                    recycler.SetFlag(BaseEntity.Flags.Reserved9, false, false);
                }
            }
        }

        private void OnEntitySpawned(BaseNetworkable entity)
        {
            if (entity is Recycler recycler)
            {
                recyclerOriginalStates[recycler.net.ID.Value] = recycler.HasFlag(BaseEntity.Flags.Reserved9);
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

        private void Unload()
        {
            foreach (var entity in BaseNetworkable.serverEntities)
            {
                if (entity is Recycler recycler)
                {
                    recycler.recycleEfficiency = 0.6f;
                    recycler.safezoneRecycleEfficiency = 0.4f;
                    recycler.radtownRecycleEfficiency = 0.6f;

                    if (recyclerOriginalStates.TryGetValue(recycler.net.ID.Value, out bool originalState))
                    {
                        recycler.SetFlag(BaseEntity.Flags.Reserved9, originalState, true, true);
                    }
                }
            }
        recyclerOriginalStates.Clear();
        }
    }
}

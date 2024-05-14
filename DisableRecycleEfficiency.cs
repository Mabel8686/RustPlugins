namespace Oxide.Plugins
{
    [Info("DisableRecycleEfficiency", "Mabel", "1.0.0")]
    [Description("Disables the recycle efficiency values for recyclers.")]
    public class DisableRecycleEfficiency : RustPlugin
    {
        private const float DisabledRecycleEfficiency = 0.5f;

        private void OnServerInitialized()
        {
            foreach (var recycler in UnityEngine.Object.FindObjectsOfType<Recycler>())
            {
                DisableEfficiency(recycler);
            }
        }

        private void OnEntitySpawned(BaseNetworkable entity)
        {
            if (entity is Recycler recycler)
            {
                DisableEfficiency(recycler);
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
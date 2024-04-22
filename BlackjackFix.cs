namespace Oxide.Plugins
{
    [Info("Blackjack Fix", "Mabel", "1.0.1")]
    [Description("Fixes Blackjack spawned in wrong rotation")]
    public class BlackjackFix : RustPlugin
    {
        private void OnEntitySpawned(BlackjackMachine entity)
        {
            if (entity.OwnerID == 0)
                return;

            var transform = entity.transform;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }
    }
}

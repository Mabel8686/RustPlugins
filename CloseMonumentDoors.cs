namespace Oxide.Plugins
{
    [Info("Close Monument Doors", "Mabel", "1.0.0")]
    [Description("Closes Monument Doors on server start or via console command")]

    public class CloseMonumentDoors : RustPlugin
    {
        private void OnServerInitialized()
        {
            CloseDoors();
        }

        private void CloseDoors()
        {
            foreach (var entity in BaseNetworkable.serverEntities)
            {
                if (entity is Door door)
                {
                    if (door.OwnerID == 0)
                    {
                        door.SetFlag(BaseEntity.Flags.Open, false);
                        door.SendNetworkUpdateImmediate();
                    }
                }
            }
        }

        [ConsoleCommand("closedoors")]
        private void CloseCommand(ConsoleSystem.Arg arg)
        {
            if (arg.Connection != null)
            {
                return;
            }
            CloseDoors();
        }
    }
}
# RustPlugins

**DropInventory.cs** - Useful plugin for BattleField type servers, allows players with permission to drop there inventory to claim kits faster or can be used by Admins to clear players inventory 
                     
                    Players chat command: /drop
                    Admin chat command: /drop.admin <playername or steamid>
                    Admin console command: drop.admin <playername or steamid>

**MLRSCooldown.cs** - Prevents players mounting the mlrs after use for x amount of time (default 20 minutes), useful for servers with multiple Mlrs

**MLRSOwnershipControl.cs** - Prevents players who are not the owner or a team member of the owner from using the MLRS, useful for servers with multiple Mlrs

**ServerPop.cs** - Shows server pop in chat with !pop trigger

**NoLockOnRockets.cs** - prevents players without the bypass permission from putting lock on rockets in there inventory

**NPCGifts.cs** - spawns gifts on npc death, can configure the drop rate and cooldown for the gifts withe personalised message to the player

**EventMessages.cs** - adds game tip event messages for servers who are changing TOD or skipping night and are manually spawning the default events with AutomatedEvents or similar

**MapNameTimers.cs** - Displays map name, wipe timer and purge timers in the map name field  (Timers work off Epoch timestamp, use https://www.epochconverter.com/ to set your Epoch timestamp)

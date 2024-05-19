# RustPlugins

**DropInventory.cs** - Useful plugin for BattleField type servers, allows players with permission to drop there inventory to claim kits faster or can be used by Admins to clear players inventory 
                     
                    Players chat command: /drop
                    Admin chat command: /drop.admin <playername or steamid>
                    Admin console command: drop.admin <playername or steamid>

**MLRSCooldown.cs** - Prevents players mounting the mlrs after use for x amount of time (default 20 minutes), useful for servers with multiple Mlrs

**MLRSOwnership.cs** - Prevents players who are not the owner or a team member of the owner from using the MLRS, useful for servers with multiple Mlrs

**ServerPop.cs** - Shows server pop in chat with !pop trigger

**NoLockOnRockets.cs** - prevents players without the bypass permission from putting lock on rockets in there inventory

**NPCGifts.cs** - spawns gifts on npc death, can configure the drop rate and cooldown for the gifts with personalised message to the player

**BlackjackFix.cs** - Fixes Blackjack spawned in wrong rotation

**DisableRecycleEfficiency.cs** - Sets all recyclers back to the old efficiency (UI Remains as its client side)



                                                                            **NPC Gifts**
 

The NPC Gifts plugin provides a system where players can receive random gifts upon killing NPCs.  The gifts are spawned as containers, and there are customizable settings for cooldowns, spawn chances, and permissions.                                                                                                                    
     

**Features**

Randomly spawns gifts when players kill NPCs.

Customizable cooldowns to prevent gift spamming.

Different container types with individual spawn chances.

Permission-based access to specific container types.

*Configurable messages for chat and game tips.


**Configuration Options**

**Cooldown Settings:**

**Cooldown Duration Minutes:** The duration before a player can receive another gift after killing an NPC.
 

**Message Settings:**

**Enable Chat Message:** Whether to send a chat message to the player when they receive a gift.

**Enable Game Tip Message:** Whether to show a game tip message to the player when they receive a gift.

 

**Container Settings:**

**Enabled:** Whether this container type is enabled.

**Container Prefab:** The prefab path for the container to spawn.

**Spawn Chance:** The chance (between 0 and 1) that this container will spawn when an NPC is killed.

**Permission:** The permission required for a player to receive this container type.

**Loot Table Name:** The name of the loot table to use for this container. If left empty, the default loot table is used.

**Min Items:** The minimum number of items to spawn in the container.

**Max Items:** The maximum number of items to spawn in the container.

 
**Adding/Editing Containers**

**Enable/Disable Containers:** Set the Enabled field to true or false to control whether the container is used.

**Adjust Spawn Chances:** Modify the Spawn Chance to change how often a container is spawned. A value of 0.5 means there's a 50% chance.

**Set Permissions:** Specify a Permission for each container. Ensure these permissions are registered and granted to the appropriate players or groups.

**Set Loot Table Name:** This filed is only required if using the Simple Loot Table Plugin, left blank will use the servers default loot tables

**Set Min/Max Items:** These Fields are only required for users of  the Simple Loot Table Plugin, they will not adjust the min/max values of your server loot table


**Permissions**

**Register Permissions:** The plugin will automatically register permissions based on the configuration.

**Grant Permissions:** Use the following commands to grant permissions to players or groups:
 
```
oxide.grant user <username> <permission>

oxide.grant group <groupname> <permission>
```

**Example:**

`oxide.grant user Mabel npcgifts.example1`
 

**Default Configuration**
```cs

{
  "Cooldown Settings": {
    "Cooldown Duration Minutes": 60
  },
  "Message Settings": {
    "Enable Chat Message": false,
    "Enable Game Tip Message": true
  },
  "Container Settings": [
    {
      "Enabled": true,
      "Container Prefab": "assets/prefabs/misc/xmas/sleigh/presentdrop.prefab",
      "Spawn Chance": 0.5,
      "Permission": "npcgifts.example1",
      "Loot Table Name": null,
      "Min Items": 0,
      "Max Items": 0
    },
    {
      "Enabled": true,
      "Container Prefab": "assets/prefabs/missions/portal/proceduraldungeon/xmastunnels/loot/xmastunnellootbox.prefab",
      "Spawn Chance": 0.5,
      "Permission": "npcgifts.example2",
      "Loot Table Name": null,
      "Min Items": 0,
      "Max Items": 0
    },
    {
      "Enabled": true,
      "Container Prefab": "assets/prefabs/misc/xmas/giftbox/giftbox_loot.prefab",
      "Spawn Chance": 0.5,
      "Permission": "npcgifts.example3",
      "Loot Table Name": null,
      "Min Items": 0,
      "Max Items": 0
    }
  ],
  "Version": {
    "Major": 1,
    "Minor": 0,
    "Patch": 4
  }
}
```


**Default Language:**
```cs
{
  "ChatMessage": " :dance: Congratulations <color=#abf229>{0}</color> You Received A Gift From <color=#abf229>{1}</color> :dance:",
  "GameTipMessage": "Congratulations <color=#abf229>{0}</color> You Received A Gift From <color=#abf229>{1}</color>"
}
```

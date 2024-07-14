# RustPlugins

**DropInventory** - Useful plugin for BattleField type servers, allows players with permission to drop there inventory to claim kits faster or can be used by Admins to clear players inventory 
 ```cs
Players chat command: /drop
Admin chat command: /drop.admin <playername or steamid>
Admin console command: drop.admin <playername or steamid>
```

**MLRSCooldown** - Prevents players mounting the mlrs after use for x amount of time (default 20 minutes), useful for servers with multiple Mlrs

**MLRSOwnership** - Prevents players who are not the owner or a team member of the owner from using the MLRS, useful for servers with multiple Mlrs

**NoLockOnRockets** - prevents players without the bypass permission from putting lock on rockets in there inventory

**BlackjackFix** - Fixes Blackjack spawned in wrong rotation

**DisableRecycleEfficiency** - Sets all recyclers back to the old efficiency (UI Remains as its client side)

**CloseMonumentDoors** - Closes Monument Doors on server start or via console command (Command:`closedoors`)

**FreshPump** - Converts Salt Water --> Fresh Water in water pumps





# NPC GIFTS
 

The NPC Gifts plugin provides a system where players can receive random gifts upon killing NPCs.  The gifts are spawned as containers, and there are customizable settings for cooldowns, spawn chances, and permissions.                                                                                                                    
     

# **Features**

Randomly spawns gifts when players kill NPCs.

Customizable cooldowns to prevent gift spamming.

Different container types with individual spawn chances.

Permission-based access to specific container types.

*Configurable messages for chat and game tips.


# **Configuration Options**

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

 
# **Adding/Editing Containers**

**Enable/Disable Containers:** Set the Enabled field to true or false to control whether the container is used.

**Adjust Spawn Chances:** Modify the Spawn Chance to change how often a container is spawned. A value of 0.5 means there's a 50% chance.

**Set Permissions:** Specify a Permission for each container. Ensure these permissions are registered and granted to the appropriate players or groups.

**Set Loot Table Name:** This filed is only required if using the Simple Loot Table Plugin, left blank will use the servers default loot tables

**Set Min/Max Items:** These Fields are only required for users of  the Simple Loot Table Plugin, they will not adjust the min/max values of your server loot table


# **Permissions**

**Register Permissions:** The plugin will automatically register permissions based on the configuration.

**Grant Permissions:** Use the following commands to grant permissions to players or groups:
 
```cs
oxide.grant user <username> <permission>

oxide.grant group <groupname> <permission>
```

**Example:**
```cs
oxide.grant user Mabel npcgifts.example1
```
 
# **Console Command**

```cs
npcgifts_wipe
```

# **Default Configuration**

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
    "Patch": 5
  }
}
```


# **Default Language:**
```cs
{
  "ChatMessage": " :dance: Congratulations <color=#abf229>{0}</color> You Received A Gift From <color=#abf229>{1}</color> :dance:",
  "GameTipMessage": "Congratulations <color=#abf229>{0}</color> You Received A Gift From <color=#abf229>{1}</color>"
}
```


# SERVER POP

Display server population statistics in the chat using the !pop trigger. It provides detailed information about online, sleeping, joining, and queued players. The plugin also supports cooldowns for command usage, customizable chat messages, and game tip responses.

 

# **Features**

**Cooldown Management**

Prevents spam by enforcing a configurable cooldown period for the !pop trigger.

 

**Detailed Player Statistics**

Displays the number of online players, sleeping players, players joining, and players in the queue.

 

**Customizable Messages**

Configure the chat prefix and SteamID for message icons.

Customize messages' appearance, including colors and formats.

 

**Multi-Response Options**

Option to broadcast the server population message globally or to the player who issued the trigger.

Supports both chat messages and game tip notifications.

 

**Welcome Messages**

Displays a customizable welcome message to players when they connect.

Optionally shows the server population on player connect.


**Trigger Filter**
Auto filters the !pop trigger so it isn't displayed in the chat **(Reqs BetterChat)**

 


# **Configuration Options:**

**Cooldown Settings**

   **• Cooldown (seconds):** The cooldown period in seconds between uses of the !pop command.

 

**Chat Settings**

   **• Chat Prefix:** The prefix displayed before the server population message in chat.

   **• Chat Icon SteamID:** The SteamID of the icon used for chat messages.

 

**Message Settings**

   **• Global Response:** Determines if the response should be broadcast globally or sent only to the player who triggered the command.

   **• Use Chat Response:** If true, responses will be sent in the chat.

   **• Use Game Tip Response:** If true, responses will be shown as game tips (toasts).

   **• Value Color (HEX):** The color used for values in the messages.

 

**Response Settings**

   **• Show Online Players:** Show the number of online players.

   **• Show Sleeping Players:** Show the number of sleeping players.

   **• Show Joining Players:** Show the number of players currently joining.

   **• Show Queued Players:** Show the number of players in the queue.

 

**Connect Settings**

  **• Show Pop On Connect:** Show the server population message when a player connects.

   **• Show Welcome Message:** Show a welcome message when a player connects.



# **Default Configuration:**

```cs
{
  "Cooldown Settings": {
    "Cooldown (seconds)": 60
  },
  "Chat Settings": {
    "Chat Prefix": "<size=16><color=#FFA500>| Server Pop |</color></size>",
    "Chat Icon SteamID": 76561199216745230
  },
  "Messgae Settings": {
    "Global Response (true = global response, false = player response)": false,
    "Use Chat Response": false,
    "Use Game Tip Response": true,
    "Value Color (HEX)": "#FFA500"
  },
  "Response Settings": {
    "Show Online Players": true,
    "Show Sleeping Players": true,
    "Show Joining Players": true,
    "Show Queued Players": true
  },
  "Connect Settings": {
    "Show Pop On Connect": false,
    "Show Welcome Message": false
  },
  "Version": {
    "Major": 1,
    "Minor": 0,
    "Patch": 8
  }
}
```

# **Default Language:**
```cs
{
  "Online": "{0} / {1} players online",
  "Sleeping": "{0} players sleeping",
  "Joining": "{0} players joining",
  "Queued": "{0} players queued",
  "WelcomeMessage": "Welcome to the server {0}!",
  "CooldownMessage": "You must wait {0} seconds before using this command again."
}
```

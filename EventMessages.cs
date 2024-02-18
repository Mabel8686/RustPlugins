using System.Collections.Generic;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Event Messages", "Mabel", "1.0.0")]
    [Description("Sends Game Tip When Events Are Manually Triggered")]
    public class EventMessages : RustPlugin
    {
        void OnEntitySpawned(BaseEntity entity)
        {
            if (entity == null || entity.IsDestroyed) return;

            if (entity is CargoPlane)
            {
                SendToastToActivePlayers("CargoPlaneSpawn");
            }
			else if (entity is CH47Helicopter)
			{
                SendToastToActivePlayers("CH47Spawn");
            }
			else if (entity is PatrolHelicopter)
			{
                SendToastToActivePlayers("PatrolHeliSpawn");
            }
			else if (entity is CargoShip)
			{
                SendToastToActivePlayers("CargoShipSpawn");
            }
			else if (entity is BradleyAPC)
			{
                SendToastToActivePlayers("BradleySpawn");
            }
			else if (entity is HackableLockedCrate)
			{
				HackableLockedCrate crate = entity as HackableLockedCrate;
				if (crate != null && crate.ShortPrefabName == "codelockedhackablecrate_oilrig")
				{
					if (IsLargeOilRigCrate(crate))
					{
						SendToastToActivePlayers("LargeOilRigCrateSpawn");
					}
					else if (IsSmallOilRigCrate(crate))
					{
						SendToastToActivePlayers("SmallOilRigCrateSpawn");
					}
				}
			}	
        }
		
		bool IsLargeOilRigCrate(HackableLockedCrate crate)
		{
			foreach (var monument in TerrainMeta.Path.Monuments)
			{
				if (monument == null) continue;
				if (Vector3.Distance(crate.transform.position, monument.transform.position) < 100)
				{	
			        if (monument.displayPhrase.english.Contains("Large Oil Rig"))
					{
						return true;
					}	
				}
			}
			return false;
		}
		
		bool IsSmallOilRigCrate(HackableLockedCrate crate)
		{
			foreach (var monument in TerrainMeta.Path.Monuments)
			{
				if (monument == null) continue;
				if (Vector3.Distance(crate.transform.position, monument.transform.position) < 100)
				{	
			        if (monument.displayPhrase.english.Contains("Oil Rig") && !monument.displayPhrase.english.Contains("Large"))
					{
						return true;
					}	
				}
			}
			return false;
		}
		
		void OnDieselEngineToggle(DieselEngine engine, BasePlayer player)
        {
            SendToastToActivePlayers("ExcavatorActivated");
        }
		
		void SendToastToActivePlayers(string messageKey)
		{
			foreach (BasePlayer player in BasePlayer.activePlayerList)
			{
				if (player != null)
				{
					player.ShowToast(GameTip.Styles.Red_Normal, lang.GetMessage(messageKey, this, player.UserIDString));
				}
			}
		}	

        protected override void LoadDefaultMessages()
        {
            lang.RegisterMessages(new Dictionary<string, string>
            {
                ["BradleySpawn"] = "Bradley has spawned",
                ["CargoShipSpawn"] = "Cargo Ship has spawned",
                ["PatrolHeliSpawn"] = "Patrol Helicopter has spawned",
                ["CH47Spawn"] = "CH47 Helicopter has spawned",
                ["CargoPlaneSpawn"] = "Cargo Plane has spawned",
                ["ExcavatorActivated"] = "Excavator has been activated",
				["LargeOilRigCrateSpawn"] = "Large Oil Rig is online",
                ["SmallOilRigCrateSpawn"] = "Small Oil Rig is online"
            }, this);
        }
    }
}
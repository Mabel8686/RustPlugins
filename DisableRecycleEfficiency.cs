/*
Copyright © 2024 Mabel

All rights reserved. This plugin is protected by copyright law.

You may not modify, redistribute, or resell this software without explicit written permission from the copyright owner.

For any support please message me directly via Discord `mabel8686`
*/
using System.Collections.Generic;
using Oxide.Core;

namespace Oxide.Plugins
{
    [Info("Disable Recycle Efficiency", "Mabel", "1.0.1")]
    [Description("Disables the recycle efficiency.")]
    public class DisableRecycleEfficiency : RustPlugin
    {
        private const float DisabledRecycleEfficiency = 0.5f;

        private class RecyclerState
        {
            public float RecycleEfficiency { get; set; }
            public float SafezoneRecycleEfficiency { get; set; }
            public float RadtownRecycleEfficiency { get; set; }
            public bool OriginalFlagState { get; set; }
        }

        private Dictionary<ulong, RecyclerState> recyclerOriginalStates = new Dictionary<ulong, RecyclerState>();

        private void Init()
        {
            LoadData();
        }

        private void OnServerInitialized()
        {
            foreach (var entity in BaseNetworkable.serverEntities)
            {
                if (entity is Recycler recycler)
                {
                    SaveOriginalState(recycler);
                    DisableEfficiency(recycler);
                    recycler.SetFlag(BaseEntity.Flags.Reserved9, false, false);
                }
            }
        }

        private void OnEntitySpawned(BaseNetworkable entity)
        {
            if (entity is Recycler recycler)
            {
                SaveOriginalState(recycler);
                DisableEfficiency(recycler);
                recycler.SetFlag(BaseEntity.Flags.Reserved9, false, false);
            }
        }

        private void SaveOriginalState(Recycler recycler)
        {
            if (!recyclerOriginalStates.ContainsKey(recycler.net.ID.Value))
            {
                recyclerOriginalStates[recycler.net.ID.Value] = new RecyclerState
                {
                    RecycleEfficiency = recycler.recycleEfficiency,
                    SafezoneRecycleEfficiency = recycler.safezoneRecycleEfficiency,
                    RadtownRecycleEfficiency = recycler.radtownRecycleEfficiency,
                    OriginalFlagState = recycler.HasFlag(BaseEntity.Flags.Reserved9)
                };
                SaveData();
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
                    RestoreOriginalState(recycler);
                }
            }
            recyclerOriginalStates.Clear();
            SaveData();
        }

        private void RestoreOriginalState(Recycler recycler)
        {
            if (recyclerOriginalStates.TryGetValue(recycler.net.ID.Value, out RecyclerState originalState))
            {
                recycler.recycleEfficiency = originalState.RecycleEfficiency;
                recycler.safezoneRecycleEfficiency = originalState.SafezoneRecycleEfficiency;
                recycler.radtownRecycleEfficiency = originalState.RadtownRecycleEfficiency;
                recycler.SetFlag(BaseEntity.Flags.Reserved9, originalState.OriginalFlagState, true, true);
            }
        }

        private void SaveData()
        {
            Interface.Oxide.DataFileSystem.WriteObject(Name, recyclerOriginalStates);
        }

        private void LoadData()
        {
            recyclerOriginalStates = Interface.Oxide.DataFileSystem.ReadObject<Dictionary<ulong, RecyclerState>>(Name) ?? new Dictionary<ulong, RecyclerState>();
        }

        private void NewSave()
        {
            recyclerOriginalStates.Clear(); SaveData(); Puts("Recycler states data has been wiped.");
        }

        [ConsoleCommand("dre_wipe")]
        private void WipeRecyclerData(ConsoleSystem.Arg arg) { if (arg.Args == null) { ResetData(); } }

        private void ResetData() { recyclerOriginalStates.Clear(); SaveData(); Puts("Recycler states data has been wiped."); }
    }
}

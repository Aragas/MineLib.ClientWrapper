using System;
using System.Collections.Generic;
using MineLib.Network.Data;
using MineLib.Network.Enums;
using MineLib.Network.Packets.Client;

namespace MineLib.ClientWrapper.BigData
{
    public class Player
    {
        public Player()
        {
            Effects = new List<PlayerEffect>();
            Windows = new List<PlayerWindow>();
        }

        public int EntityID;

        public PlayerPosition Position; //
        public PlayerNewPosition NewPosition; //
        public PlayerLook Look; //
        public PlayerAbilities Abilities;

        public PlayerHealth Health; //
        public PlayerItems Items; //
        public byte HeldItem; // //Slot
        public PlayerExperience Experience; //
        public Animation Animation;

        ///
        public PlayerStatistics Statistics;

        public List<PlayerEffect> Effects;
        public List<PlayerWindow> Windows;
        public PlayerScoreboardObjective ScoreboardObjective;

        public void SetWindowItems(byte windowId, ItemStack[] slotData)
        {
            if (Items.WindowId == windowId)
                Items.SlotData = slotData;
        }

        public void SetSlot(byte windowId, short slot, ItemStack slotData)
        {
            if (Items.WindowId == windowId)
                Items.SlotData[slot] = slotData;

        }

        public void OpenWindow(byte windowId, byte inventoryType, string windowTitle, byte NumberOfSlots,
            bool UseProvidedWindowTitle, int? EntityID = 0)
        {
        }

        public void CloseWindow(byte windowId)
        {
        }

        public void ConfirmTransaction(byte windowsId, short actionNumber, bool accepted)
        {
            
        }
    }

    public struct PlayerPosition
    {
        public Vector3 Vector3;
        public bool OnGround;
        public bool Initialized;
    }

    public struct PlayerNewPosition
    {
        public Vector3 Vector3;
        public float Yaw;
        public float Pitch;
        public bool OnGround;
    }

    public struct PlayerAbilities
    {
        public byte Flags;
        public bool DamageDisabled { get { return Convert.ToBoolean((Flags >> 3) & 1); } }
        public bool CanFly { get { return Convert.ToBoolean((Flags >> 2) & 1); } }
        public bool IsFlying { get { return Convert.ToBoolean((Flags >> 1) & 1); } }
        public bool CreativeMode { get { return Convert.ToBoolean((Flags >> 0) & 1); } }
        public float FlyingSpeed;
        public float WalkingSpeed;
    }

    public struct PlayerLook
    {
        public float Yaw;
        public float Pitch;
    }

    public struct PlayerHealth
    {
        public float Health;
        public short Food;
        public float FoodSaturation;
    }

    public struct PlayerItems
    {
        public byte WindowId;
        public ItemStack[] SlotData;
    }

    public struct PlayerExperience
    {
        public float ExperienceBar;
        public short Level;
        public short TotalExperience;
    }

    public struct PlayerStatistics
    {
        public int Count;
        public string[] StatisticsName;
        public int[] Value;
    }

    public struct PlayerEffect
    {
        public EffectID EffectID;
        public byte Amplifier;
        public short Duration;
    }

    public struct PlayerWindow
    {

    }

    public struct PlayerScoreboardObjective
    {
        public string ObjectiveName;
        public string ObjectiveValue;
        public string ItemName;
        public int Value;

    }

}

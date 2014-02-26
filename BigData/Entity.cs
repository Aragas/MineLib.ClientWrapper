using System.Collections.Generic;
using MineLib.Network.Data;
using MineLib.Network.Data.EntityMetadata;
using MineLib.Network.Enums;

namespace MineLib.ClientWrapper.BigData
{
    public class Entity
    {
        public Entity()
        {
            Metadata = new MetadataDictionary();
            Vehile = new Vehile();
            Effects = new Dictionary<int, EntityEffect>();
        }

        public int EntityID;
        public MetadataDictionary Metadata;

        public Vehile Vehile;
        public bool Leash;
        public EntityProperty[] Properties;

        public EntityStatus Status;
        public EntityEquipment Equipment;
        public EntityAnimation Animation;
        public EntityPosition Position;
        public EntityLook Look;
        public Dictionary<int, EntityEffect> Effects;
        public EntityBed Bed;
        public EntityPlayer Player;
        public EnityVelocity Velocity;
        public EnityNewPosition NewPosition;
    }

    public struct EntityEquipment
    {
        public EntityEquipmentSlot Slot;
        public ItemStack Item;

        public short CurrentItem;
    }

    public struct EntityUseBed
    {
        public int X;
        public byte Y;
        public int Z;
    }

    public struct EntityAnimation
    {
        public Animation Animation;
    }

    public struct EntityPosition
    {
        public int X, Y, Z;
    }

    public struct EntityLook
    {
        public byte Yaw, HeadPitch, Pitch;
    }

    public struct EntityEffect
    {
        public byte EffectID;
        public byte Amplifier;
        public short Duration;
    }

    public struct EntityBed
    {
        public int X;
        public byte Y;
        public int Z;
    }

    public struct EntityPlayer
    {
        public string PlayerUUID, PlayerName;
    }

    public struct EnityVelocity
    {
        public short VelocityX, VelocityY, VelocityZ;
    }

    public struct EnityNewPosition
    {
        public double X, Y, Z;
        public float Yaw, Pitch;
        public bool OnGround;
    }

}

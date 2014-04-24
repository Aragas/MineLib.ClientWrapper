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
            Vehicle = new Vehicle();
            Effects = new List<EntityEffect>();
        }

        public int EntityID;
        public MetadataDictionary Metadata;

        public Vehicle Vehicle;
        public bool Leash;
        public EntityProperty[] Properties;

        public EntityStatus Status;
        public EntityEquipment Equipment;
        public Animation Animation;
        public Vector3 Position;
        public EntityLook Look;
        public List<EntityEffect> Effects;
        public Vector3 Bed;
        public EntityPlayer Player;
        public EntityVelocity Velocity;
        public EntityNewPosition NewPosition;

        public bool IsPlayer
        {
            get { return Player.Name != null && Player.UUID != null; }
        }
    }

    public struct EntityEquipment
    {
        public EntityEquipmentSlot Slot;
        public ItemStack Item;

        public short CurrentItem;
    }

    public struct EntityLook
    {
        public byte Yaw;
        public byte HeadPitch;
        public byte Pitch;
    }

    public struct EntityEffect
    {
        public EffectID EffectID;
        public byte Amplifier;
        public short Duration;
    }

    public struct EntityPlayer
    {
        public string UUID;
        public string Name;
    }

    public struct EntityVelocity
    {
        public short VelocityX;
        public short VelocityY;
        public short VelocityZ;
    }

    public struct EntityNewPosition
    {
        public Vector3 Vector3;
        public float Yaw;
        public float Pitch;
        public bool OnGround;
    }

}

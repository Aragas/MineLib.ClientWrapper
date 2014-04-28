using System;
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
        public Coordinates3D Bed;
        public EntityPlayer Player;
        public EntityVelocity Velocity;

        public bool IsPlayer
        {
            get { return Player.Name != null && Player.UUID != null; }
        }

        public static Vector3 ToVector3(byte yaw, byte pitch)
        {
            return new Vector3
            {
                X = -Math.Cos(pitch) * Math.Sin(yaw),
                Y = -Math.Sin(pitch),
                Z =  Math.Cos(pitch) * Math.Cos(yaw),
            };
        }

        public static int ToYaw(Vector3 position, Vector3 look)
        {
            var l = look.X - position.X;
            var w = look.Z - position.Z;
            var c = Math.Sqrt(l * l + w * w);
            var alpha1 = -Math.Asin(l / c) / Math.PI * 180;
            var alpha2 = Math.Acos(w / c) / Math.PI * 180;
            return alpha2 > 90 ? (180 - (int)alpha1) : (int)alpha1;
        }
    }

    public struct EntityEquipment
    {
        public EntityEquipmentSlot Slot;
        public ItemStack Item;

        public short CurrentItem;
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

    public struct EntityLook
    {
        public byte Yaw;
        public byte Pitch;
    }

    public struct EntityVelocity
    {
        public short VelocityX;
        public short VelocityY;
        public short VelocityZ;
    }

}

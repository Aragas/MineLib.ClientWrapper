using System;
using System.Collections.Generic;
using MineLib.Network.Main.Data;
using MineLib.Network.Main.Enums;
using Org.BouncyCastle.Math;

namespace MineLib.ClientWrapper.BigData
{
    public class Entity
    {
        public int EntityID;

        public Mobs Type;

        public EntityMetadata Metadata;

        public EntityPlayer Player;

        public Animation Animation;

        public Vector3 Position;
        public EntityLook Look;

        public Position Bed;

        public EntityStatus Status;
        public EntityEquipment Equipment;

        public EntityVelocity Velocity;

        public List<EntityEffect> Effects;

        public EntityPropertyList Properties;

        public Vehicle Vehicle;

        public bool Leash;

        public bool OnGround;

        public Entity()
        {
            Metadata = new EntityMetadata();
            Vehicle = new Vehicle();
            Effects = new List<EntityEffect>();

            OnGround = false;
        }

        public Entity(int id)
        {
            EntityID = id;
            Metadata = new EntityMetadata();
            Vehicle = new Vehicle();
            Effects = new List<EntityEffect>();
        }

        public bool IsPlayer
        {
            get { return Player.UUID != null && Player.UUID != null; }
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
        public sbyte Amplifier;
        public int Duration;
        public bool HideParticle;
    }

    public struct EntityPlayer
    {
        public BigInteger UUID;
        //public string Name;
    }

    public struct EntityLook
    {
        public sbyte Yaw;
        public sbyte HeadYaw;
        public sbyte Pitch;
        public sbyte HeadPitch;
    }

    public struct EntityVelocity
    {
        public short VelocityX;
        public short VelocityY;
        public short VelocityZ;
    }

}

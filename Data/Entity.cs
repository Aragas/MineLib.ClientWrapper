using System;
using System.Collections.Generic;
using MineLib.Network.Data;
using MineLib.Network.Data.Structs;
using Org.BouncyCastle.Math;

namespace MineLib.ClientWrapper.Data.BigData
{
    public class Entity
    {
        public int EntityID;

        public int Type;

        public EntityMetadataList Metadata;

        public EntityPlayer Player;

        public int Animation;

        public Vector3 Position;
        public EntityLook Look;

        public Position Bed;

        public int Status;
        public EntityEquipment Equipment;

        public EntityVelocity Velocity;

        public List<EntityEffect> Effects;

        public EntityPropertyList Properties;

        public Vehicle Vehicle;

        public bool Leash;

        public bool OnGround;

        public Entity()
        {
            Metadata = new EntityMetadataList();
            Vehicle = new Vehicle();
            Effects = new List<EntityEffect>();

            OnGround = false;
        }

        public Entity(int id)
        {
            EntityID = id;
            Metadata = new EntityMetadataList();
            Vehicle = new Vehicle();
            Effects = new List<EntityEffect>();
        }

        public bool IsPlayer
        {
            get { return Player.UUID != null && Player.UUID != null; }
        }
    }

    public struct EntityEquipment
    {
        public int Slot;
        public ItemStack Item;

        public short CurrentItem;
    }

    public struct EntityEffect
    {
        public int EffectID;
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

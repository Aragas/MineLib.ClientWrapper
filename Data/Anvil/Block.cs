using System.Collections.Generic;
using MineLib.ClientWrapper.BigData;
using MineLib.Network.Data;

namespace MineLib.ClientWrapper.Data.Anvil
{
    // Get Block methods
    public partial class Block
    {
        public static string GetName(int id, int meta = 0)
        {
            foreach (var blockNew in List)
            {
                if (blockNew.Id == id && blockNew.Meta == meta)
                    return blockNew.Name;
            }
            return null;
        }

        public static Block GetBlock(int id, int meta = 0)
        {
            foreach (var blockNew in List)
            {
                if (blockNew.Id == id && blockNew.Meta == meta)
                    return blockNew;
            }

            // -- Debugging
            var text = string.Format("ID: {0}, Meta: {1}", id, meta);
            if (!World.UnsupportedBlockList.Contains(text))
                World.UnsupportedBlockList.Add(text);
            // -- Debugging

            return new Block
            {
                Id = id,
                Meta = meta,
                Name = string.Format("Unsupported Block ID: {0}, Meta: {1}", id, meta)
            };
        }

        #region List

        private static readonly List<Block> List = new List<Block>
        {
            new Block(0, "Air"),
            new Block(1, "Stone"),
            new Block(1, 1, "Granite"),
            new Block(1, 2, "PolishedGranite"),
            new Block(1, 3, "Diorite"),
            new Block(1, 4, "PolishedDiorite"),
            new Block(1, 5, "Andesite"),
            new Block(1, 6, "PolishedAndesite"),
            new Block(2, "Grass"),
            new Block(3, "Dirt"),
            new Block(3, 1, "GrasslessDirt"),
            new Block(3, 2, "Podzol"),
            new Block(4, "Cobblestone"),
            new Block(5, "OakWoodPlank"),
            new Block(5, 1, "SpruceWoodPlan"),
            new Block(5, 2, "BirchWoodPlank"),
            new Block(5, 3, "JungleWoodPlank"),
            new Block(5, 4, "AcaciaWoodPlank"),
            new Block(5, 5, "DarkOakWoodPlank"),
            new Block(6, "OakSapling"),
            new Block(6, 1, "SpruceSapling"),
            new Block(6, 2, "BirchSapling"),
            new Block(6, 3, "JungleSapling"),
            new Block(6, 4, "AcaciaSapling"),
            new Block(6, 5, "DarkOakSapling"),
            new Block(7, "Bedrock"),
            new Block(8, "Water"),
            new Block(9, "StationaryWater"),
            new Block(10, "Lava"),
            new Block(11, "StationaryLava"),
            new Block(12, "Sand"),
            new Block(12, 1, "RedSand"),
            new Block(13, "Gravel"),
            new Block(14, "GoldOre"),
            new Block(15, "IronOre"),
            new Block(16, "CoalOre"),
            new Block(17, "OakWood"),
            new Block(17, 1, "SpruceWood"),
            new Block(17, 2, "BirchWood"),
            new Block(17, 3, "JungleWood"),
            new Block(18, "OakLeaves"),
            new Block(18, 1, "SpruceLeaves"),
            new Block(18, 2, "BirchLeaves"),
            new Block(18, 3, "JungleLeaves"),
            new Block(19, "Sponge"),
            new Block(20, "Glass"),
            new Block(21, "LapisLazuliOre"),
            new Block(22, "LapisLazuliBlock"),
            new Block(23, "Dispenser"),
            new Block(24, "Sandstone"),
            new Block(24, 1, "ChiseledSandstone"),
            new Block(24, 2, "SmoothSandstone"),
            new Block(25, "NoteBlock"),
            new Block(26, "BedBlock"),
            new Block(27, "PoweredRail"),
            new Block(28, "DetectorRail"),
            new Block(29, "StickyPiston"),
            new Block(30, "Web"),
            new Block(31, "DeadShrub"),
            new Block(31, 1, "Grass"),
            new Block(31, 2, "Fern"),
            new Block(32, "DeadShrub"),
            new Block(33, "Piston"),
            new Block(34, "PistonHead"),
            new Block(35, "WhiteWool"),
            new Block(35, 1, "OrangeWool"),
            new Block(35, 2, "MagentaWool"),
            new Block(35, 3, "LightBlueWool"),
            new Block(35, 4, "YellowWool"),
            new Block(35, 5, "LimeWool"),
            new Block(35, 6, "PinkWool"),
            new Block(35, 7, "GrayWool"),
            new Block(35, 8, "LightGrayWool"),
            new Block(35, 9, "CyanWool"),
            new Block(35, 10, "PurpleWool"),
            new Block(35, 11, "BlueWool"),
            new Block(35, 12, "BrownWool"),
            new Block(35, 13, "GreenWool"),
            new Block(35, 14, "RedWool"),
            new Block(35, 15, "BlackWool"),
            new Block(37, "Dandelion"),
            new Block(38, "Poppy"),
            new Block(38, 1, "BlueOrchid"),
            new Block(38, 2, "Allium"),
            new Block(38, 3, "AzureBluet"),
            new Block(38, 4, "RedTulip"),
            new Block(38, 5, "OrangeTulip"),
            new Block(38, 6, "WhiteTulip"),
            new Block(38, 7, "PinkTulip"),
            new Block(38, 8, "OxeyeDaisy"),
            new Block(39, "BrownMushroom"),
            new Block(40, "RedMushroom"),
            new Block(41, "GoldBlock"),
            new Block(42, "IronBlock"),
            new Block(43, "DoubleStoneSlab"),
            new Block(43, 1, "DoubleSandstoneSlab"),
            new Block(43, 2, "DoubleWoodenSlab"),
            new Block(43, 3, "DoubleCobblestoneSlab"),
            new Block(43, 4, "DoubleBrickSlab"),
            new Block(43, 5, "DoubleStoneBrickSlab"),
            new Block(43, 6, "DoubleNetherBrickSlab"),
            new Block(43, 7, "DoubleQuartzSlab"),
            new Block(44, "StoneSlab"),
            new Block(44, 1, "SandstoneSlab"),
            new Block(44, 2, "WoodenSlab"),
            new Block(44, 3, "CobblestoneSlab"),
            new Block(44, 4, "BrickSlab"),
            new Block(44, 5, "StoneBrickSlab"),
            new Block(44, 6, "NetherBrickSlab"),
            new Block(44, 7, "QuartzSlab"),
            new Block(45, "Brick"),
            new Block(46, "TNT"),
            new Block(47, "Bookshelf"),
            new Block(48, "MossyCobblestone"),
            new Block(49, "Obsidian"),
            new Block(50, "Torch"),
            new Block(51, "Fire"),
            new Block(52, "MonsterSpawner"),
            new Block(53, "OakWoodStairs"),
            new Block(54, "Chest"),
            new Block(55, "RedstoneWire"),
            new Block(56, "DiamondOre"),
            new Block(57, "DiamondBlock"),
            new Block(58, "Workbench"),
            new Block(59, "WheatCrops"),
            new Block(60, "Soil"),
            new Block(61, "Furnace"),
            new Block(62, "BurningFurnace"),
            new Block(63, "SignPost"),
            new Block(64, "WoodenDoorBlock"),
            new Block(65, "Ladder"),
            new Block(66, "Rails"),
            new Block(67, "CobblestoneStairs"),
            new Block(68, "WallSign"),
            new Block(69, "Lever"),
            new Block(70, "StonePressurePlate"),
            new Block(71, "IronDoorBlock"),
            new Block(72, "WoodenPressurePlate"),
            new Block(73, "RedstoneOre"),
            new Block(74, "GlowingRedstoneOre"),
            new Block(75, "RedstoneTorch(off)"),
            new Block(76, "RedstoneTorch(on)"),
            new Block(77, "StoneButton"),
            new Block(78, "Snow"),
            new Block(79, "Ice"),
            new Block(80, "SnowBlock"),
            new Block(81, "Cactus"),
            new Block(82, "Clay"),
            new Block(83, "SugarCane"),
            new Block(84, "Jukebox"),
            new Block(85, "Fence"),
            new Block(86, "Pumpkin"),
            new Block(87, "Netherrack"),
            new Block(88, "SoulSand"),
            new Block(89, "Glowstone"),
            new Block(90, "Portal"),
            new Block(91, "Jack-O-Lantern"),
            new Block(92, "CakeBlock"),
            new Block(93, "RedstoneRepeaterBlock(off)"),
            new Block(94, "RedstoneRepeaterBlock(on)"),
            new Block(95, "WhiteStainedGlass"),
            new Block(95, 1, "OrangeStainedGlass"),
            new Block(95, 2, "MagentaStainedGlass"),
            new Block(95, 3, "LightBlueStainedGlass"),
            new Block(95, 4, "YellowStainedGlass"),
            new Block(95, 5, "LimeStainedGlass"),
            new Block(95, 6, "PinkStainedGlass"),
            new Block(95, 7, "GrayStainedGlass"),
            new Block(95, 8, "LightGrayStainedGlass"),
            new Block(95, 9, "CyanStainedGlass"),
            new Block(95, 10, "PurpleStainedGlass"),
            new Block(95, 11, "BlueStainedGlass"),
            new Block(95, 12, "BrownStainedGlass"),
            new Block(95, 13, "GreenStainedGlass"),
            new Block(95, 14, "RedStainedGlass"),
            new Block(95, 15, "BlackStainedGlass"),
            new Block(96, "Trapdoor"),
            new Block(97, "Stone(Silverfish)"),
            new Block(97, 1, "Cobblestone(Silverfish)"),
            new Block(97, 2, "StoneBrick(Silverfish)"),
            new Block(97, 3, "MossyStoneBrick(Silverfish)"),
            new Block(97, 4, "CrackedStoneBrick(Silverfish)"),
            new Block(97, 5, "ChiseledStoneBrick(Silverfish)"),
            new Block(98, "StoneBrick"),
            new Block(98, 1, "MossyStoneBrick"),
            new Block(98, 2, "CrackedStoneBrick"),
            new Block(98, 3, "ChiseledStoneBrick"),
            new Block(99, "RedMushroomCap"),
            new Block(100, "BrownMushroomCap"),
            new Block(101, "IronBars"),
            new Block(102, "GlassPane"),
            new Block(103, "MelonBlock"),
            new Block(104, "PumpkinStem"),
            new Block(105, "MelonStem"),
            new Block(106, "Vines"),
            new Block(107, "FenceGate"),
            new Block(108, "BrickStairs"),
            new Block(109, "StoneBrickStairs"),
            new Block(110, "Mycelium"),
            new Block(111, "LilyPad"),
            new Block(112, "NetherBrick"),
            new Block(113, "NetherBrickFence"),
            new Block(114, "NetherBrickStairs"),
            new Block(115, "NetherWart"),
            new Block(116, "EnchantmentTable"),
            new Block(117, "BrewingStand"),
            new Block(118, "Cauldron"),
            new Block(119, "EndPortal"),
            new Block(120, "EndPortalFrame"),
            new Block(121, "EndStone"),
            new Block(122, "DragonEgg"),
            new Block(123, "RedstoneLamp(inactive)"),
            new Block(124, "RedstoneLamp(active)"),
            new Block(125, "DoubleOakWoodSlab"),
            new Block(125, 1, "DoubleSpruceWoodSlab"),
            new Block(125, 2, "DoubleBirchWoodSlab"),
            new Block(125, 3, "DoubleJungleWoodSlab"),
            new Block(125, 4, "DoubleAcaciaWoodSlab"),
            new Block(125, 5, "DoubleDarkOakWoodSlab"),
            new Block(126, "OakWoodSlab"),
            new Block(126, 1, "SpruceWoodSlab"),
            new Block(126, 2, "BirchWoodSlab"),
            new Block(126, 3, "JungleWoodSlab"),
            new Block(126, 4, "AcaciaWoodSlab"),
            new Block(126, 5, "DarkOakWoodSlab"),
            new Block(127, "CocoaPlant"),
            new Block(128, "SandstoneStairs"),
            new Block(129, "EmeraldOre"),
            new Block(130, "EnderChest"),
            new Block(131, "TripwireHook"),
            new Block(132, "Tripwire"),
            new Block(133, "EmeraldBlock"),
            new Block(134, "SpruceWoodStairs"),
            new Block(135, "BirchWoodStairs"),
            new Block(136, "JungleWoodStairs"),
            new Block(137, "CommandBlock"),
            new Block(138, "BeaconBlock"),
            new Block(139, "CobblestoneWall"),
            new Block(139, 1, "MossyCobblestoneWall"),
            new Block(140, "FlowerPot"),
            new Block(141, "Carrots"),
            new Block(142, "Potatoes"),
            new Block(143, "WoodenButton"),
            new Block(144, "MobHead"),
            new Block(145, "Anvil"),
            new Block(146, "TrappedChest"),
            new Block(147, "WeightedPressurePlate(light)"),
            new Block(148, "WeightedPressurePlate(heavy)"),
            new Block(149, "RedstoneComparator(inactive)"),
            new Block(150, "RedstoneComparator(active)"),
            new Block(151, "DaylightSensor"),
            new Block(152, "RedstoneBlock"),
            new Block(153, "NetherQuartzOre"),
            new Block(154, "Hopper"),
            new Block(155, "QuartzBlock"),
            new Block(155, 1, "ChiseledQuartzBlock"),
            new Block(155, 2, "PillarQuartzBlock"),
            new Block(156, "QuartzStairs"),
            new Block(157, "ActivatorRail"),
            new Block(158, "Dropper"),
            new Block(159, "WhiteStainedClay"),
            new Block(159, 1, "OrangeStainedClay"),
            new Block(159, 2, "MagentaStainedClay"),
            new Block(159, 3, "LightBlueStainedClay"),
            new Block(159, 4, "YellowStainedClay"),
            new Block(159, 5, "LimeStainedClay"),
            new Block(159, 6, "PinkStainedClay"),
            new Block(159, 7, "GrayStainedClay"),
            new Block(159, 8, "LightGrayStainedClay"),
            new Block(159, 9, "CyanStainedClay"),
            new Block(159, 10, "PurpleStainedClay"),
            new Block(159, 11, "BlueStainedClay"),
            new Block(159, 12, "BrownStainedClay"),
            new Block(159, 13, "GreenStainedClay"),
            new Block(159, 14, "RedStainedClay"),
            new Block(159, 15, "BlackStainedClay"),
            new Block(160, "WhiteStainedGlassPane"),
            new Block(160, 1, "OrangeStainedGlassPane"),
            new Block(160, 2, "MagentaStainedGlassPane"),
            new Block(160, 3, "LightBlueStainedGlassPane"),
            new Block(160, 4, "YellowStainedGlassPane"),
            new Block(160, 5, "LimeStainedGlassPane"),
            new Block(160, 6, "PinkStainedGlassPane"),
            new Block(160, 7, "GrayStainedGlassPane"),
            new Block(160, 8, "LightGrayStainedGlassPane"),
            new Block(160, 9, "CyanStainedGlassPane"),
            new Block(160, 10, "PurpleStainedGlassPane"),
            new Block(160, 11, "BlueStainedGlassPane"),
            new Block(160, 12, "BrownStainedGlassPane"),
            new Block(160, 13, "GreenStainedGlassPane"),
            new Block(160, 14, "RedStainedGlassPane"),
            new Block(160, 15, "BlackStainedGlassPane"),
            new Block(161, "AcaciaLeaves"),
            new Block(161, 1, "DarkOakLeaves"),
            new Block(162, "AcaciaWood"),
            new Block(162, 1, "DarkOakWood"),
            new Block(163, "AcaciaWoodStairs"),
            new Block(164, "DarkOakWoodStairs"),
            new Block(165, "SlimeBlock"),
            new Block(166, "Barrier"),
            new Block(167, "IronTrapdoor"),
            new Block(170, "HayBale"),
            new Block(171, "WhiteCarpet"),
            new Block(171, 1, "OrangeCarpet"),
            new Block(171, 2, "MagentaCarpet"),
            new Block(171, 3, "LightBlueCarpet"),
            new Block(171, 4, "YellowCarpet"),
            new Block(171, 5, "LimeCarpet"),
            new Block(171, 6, "PinkCarpet"),
            new Block(171, 7, "GrayCarpet"),
            new Block(171, 8, "LightGrayCarpet"),
            new Block(171, 9, "CyanCarpet"),
            new Block(171, 10, "PurpleCarpet"),
            new Block(171, 11, "BlueCarpet"),
            new Block(171, 12, "BrownCarpet"),
            new Block(171, 13, "GreenCarpet"),
            new Block(171, 14, "RedCarpet"),
            new Block(171, 15, "BlackCarpet"),
            new Block(172, "HardenedClay"),
            new Block(173, "BlockofCoal"),
            new Block(174, "PackedIce"),
            new Block(175, "Sunflower"),
            new Block(175, 1, "Lilac"),
            new Block(175, 2, "DoubleTallgrass"),
            new Block(175, 3, "LargeFern"),
            new Block(175, 4, "RoseBush"),
            new Block(175, 5, "Peony"),
        };

        #endregion

    }

    // Block data
    public partial class Block
    {
        public string Name;

        public int Id;
        public int Meta;

        public Coordinates3D Coordinates;
        public Coordinates2D Chunk;


        public Block()
        { 
        }

        public Block(int id, string name)
            : this(id, 0, name)
        {
        }

        public Block(int id, int meta, string name)
        {
            Name = name;
            Id = id;
            Meta = meta;
        }

        public override string ToString()
        {
            return Name;
        }

        public bool Equals(Block other)
        {
            return other.Id.Equals(Id) && other.Meta.Equals(Meta);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(Block)) return false;
            return Equals((Block)obj);
        }

        // We don't use Name, so we can find Block name just by Id and Metadata. 
        public override int GetHashCode()
        {
            unchecked
            {
                var result = Id.GetHashCode();
                result = (result * 397) ^ Meta.GetHashCode();
                return result;
            }
        }
    }
}
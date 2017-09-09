using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarshipTheory.ModLib.Resources
{
    public class Costs
    {
        public static EntityCost Hull = new EntityCost() { Name = "Hull", Metal = 2, Gold = 0, Silicon = 0, Water = 0, Power = 0, CPU = 0, TileHP = 40 * 3, Research = 0, Internal_Name = "Hull" };
        public static EntityCost HullCorner = new EntityCost() { Name = "HullCorner", Metal = 2, Gold = 0, Silicon = 0, Water = 0, Power = 0, CPU = 0, TileHP = 40 * 3, Research = 0, Internal_Name = "HullCorner" };
        public static EntityCost Floor = new EntityCost() { Name = "Floor", Metal = 2, Gold = 0, Silicon = 0, Water = 0, Power = 0, CPU = 0, TileHP = 40 * 3, Research = 0, Internal_Name = "Floor" };
        public static EntityCost Small_Solar = new EntityCost() { Name = "Small_Solar", Metal = 5, Gold = 0, Silicon = 2, Water = 0, Power = 0, CPU = 0, TileHP = 5 * 3, Research = 0, Internal_Name = "SSolar" };
        public static EntityCost Medium_Solar = new EntityCost() { Name = "Medium_Solar", Metal = 10, Gold = 0, Silicon = 4, Water = 0, Power = 0, CPU = 0, TileHP = 10 * 3, Research = 2, Internal_Name = "MSolar" };
        public static EntityCost Small_Reactor = new EntityCost() { Name = "Small_Reactor", Metal = 20, Gold = 2, Silicon = 4, Water = 0, Power = 0, CPU = 0, TileHP = 40 * 3, Research = 2, Internal_Name = "SReactor" };
        public static EntityCost Medium_Reactor = new EntityCost() { Name = "Medium_Reactor", Metal = 70, Gold = 8, Silicon = 16, Water = 0, Power = 0, CPU = 0, TileHP = 80 * 3, Research = 4, Internal_Name = "MReactor" };
        public static EntityCost Large_Reactor = new EntityCost() { Name = "Large_Reactor", Metal = 150, Gold = 18, Silicon = 36, Water = 0, Power = 0, CPU = 0, TileHP = 160 * 3, Research = 6, Internal_Name = "LReactor" };
        public static EntityCost Mini_Engine = new EntityCost() { Name = "Mini_Engine", Metal = 15, Gold = 0, Silicon = 8, Water = 0, Power = 5, CPU = 5, TileHP = 40 * 3, Research = 0, Internal_Name = "MiniEngine" };
        public static EntityCost Small_Engine = new EntityCost() { Name = "Small_Engine", Metal = 25, Gold = 3, Silicon = 12, Water = 0, Power = 20, CPU = 20, TileHP = 80 * 3, Research = 3, Internal_Name = "SEngine" };
        public static EntityCost Medium_Engine = new EntityCost() { Name = "Medium_Engine", Metal = 40, Gold = 8, Silicon = 18, Water = 0, Power = 40, CPU = 40, TileHP = 150 * 3, Research = 6, Internal_Name = "MEngine" };
        public static EntityCost Large_Engine = new EntityCost() { Name = "Large_Engine", Metal = 300, Gold = 20, Silicon = 50, Water = 0, Power = 500, CPU = 500, TileHP = 250 * 3, Research = 9, Internal_Name = "LEngine" };
        public static EntityCost Life_Support = new EntityCost() { Name = "Life_Support", Metal = 20, Gold = 3, Silicon = 10, Water = 0, Power = 10, CPU = 10, TileHP = 40 * 3, Research = 2, Internal_Name = "SLifeSupport" };
        public static EntityCost Airlock = new EntityCost() { Name = "Airlock", Metal = 5, Gold = 0, Silicon = 5, Water = 0, Power = 0, CPU = 0, TileHP = 20 * 3, Research = 0, Internal_Name = "Airlock" };
        public static EntityCost Food_Dispenser = new EntityCost() { Name = "Food_Dispenser", Metal = 5, Gold = 0, Silicon = 2, Water = 0, Power = 2, CPU = 2, TileHP = 10 * 3, Research = 0, Internal_Name = "Food" };
        public static EntityCost Small_Plant = new EntityCost() { Name = "Small_Plant", Metal = 5, Gold = 1, Silicon = 5, Water = 2, Power = 5, CPU = 5, TileHP = 20 * 3, Research = 0, Internal_Name = "SPlant" };
        public static EntityCost Medium_Plant = new EntityCost() { Name = "Medium_Plant", Metal = 10, Gold = 10, Silicon = 10, Water = 4, Power = 10, CPU = 10, TileHP = 40 * 3, Research = 3, Internal_Name = "MPlant" };
        public static EntityCost Large_Plant = new EntityCost() { Name = "Large_Plant", Metal = 30, Gold = 15, Silicon = 30, Water = 8, Power = 30, CPU = 30, TileHP = 100 * 3, Research = 10, Internal_Name = "LPlant" };
        public static EntityCost Cargo_Rack = new EntityCost() { Name = "Cargo_Rack", Metal = 5, Gold = 0, Silicon = 0, Water = 0, Power = 0, CPU = 0, TileHP = 10 * 3, Research = 0, Internal_Name = "CargoRack" };
        public static EntityCost Small_Crate = new EntityCost() { Name = "Small_Crate", Metal = 10, Gold = 0, Silicon = 0, Water = 0, Power = 0, CPU = 0, TileHP = 15 * 3, Research = 0, Internal_Name = "SCrate" };
        public static EntityCost Medium_Crate = new EntityCost() { Name = "Medium_Crate", Metal = 20, Gold = 0, Silicon = 0, Water = 0, Power = 0, CPU = 0, TileHP = 30 * 3, Research = 2, Internal_Name = "MCrate" };
        public static EntityCost Large_Crate = new EntityCost() { Name = "Large_Crate", Metal = 40, Gold = 0, Silicon = 0, Water = 0, Power = 0, CPU = 0, TileHP = 60 * 3, Research = 4, Internal_Name = "LCrate" };
        public static EntityCost Navigation_Console = new EntityCost() { Name = "Navigation_Console", Metal = 10, Gold = 5, Silicon = 10, Water = 0, Power = 10, CPU = 10, TileHP = 40 * 3, Research = 0, Internal_Name = "NavigationConsole" };
        public static EntityCost Radar_Console = new EntityCost() { Name = "Radar_Console", Metal = 10, Gold = 5, Silicon = 10, Water = 0, Power = 10, CPU = 10, TileHP = 40 * 3, Research = 2, Internal_Name = "RadarConsole" };
        public static EntityCost Targeting_Console = new EntityCost() { Name = "Targeting_Console", Metal = 10, Gold = 5, Silicon = 10, Water = 0, Power = 10, CPU = 10, TileHP = 40 * 3, Research = 4, Internal_Name = "TargetingConsole" };
        public static EntityCost Shield_Console = new EntityCost() { Name = "Shield_Console", Metal = 10, Gold = 5, Silicon = 10, Water = 0, Power = 10, CPU = 10, TileHP = 40 * 3, Research = 4, Internal_Name = "ShieldConsole" };
        public static EntityCost Armor_Console = new EntityCost() { Name = "Armor_Console", Metal = 10, Gold = 5, Silicon = 10, Water = 0, Power = 10, CPU = 10, TileHP = 40 * 3, Research = 4, Internal_Name = "ArmorConsole" };
        public static EntityCost Communication_Console = new EntityCost() { Name = "Communication_Console", Metal = 10, Gold = 5, Silicon = 10, Water = 0, Power = 10, CPU = 10, TileHP = 40 * 3, Research = 4, Internal_Name = "CommunicationsConsole" };
        public static EntityCost Heat_Console = new EntityCost() { Name = "Heat_Console", Metal = 10, Gold = 5, Silicon = 10, Water = 0, Power = 10, CPU = 10, TileHP = 40 * 3, Research = 2, Internal_Name = "HeatConsole" };
        public static EntityCost Research_Console = new EntityCost() { Name = "Research_Console", Metal = 10, Gold = 5, Silicon = 10, Water = 0, Power = 10, CPU = 10, TileHP = 40 * 3, Research = 0, Internal_Name = "ResearchConsole" };
        public static EntityCost Mining_Console = new EntityCost() { Name = "Mining_Console", Metal = 5, Gold = 1, Silicon = 2, Water = 0, Power = 5, CPU = 5, TileHP = 40 * 3, Research = 1, Internal_Name = "MiningConsole" };
        public static EntityCost Door = new EntityCost() { Name = "Door", Metal = 3, Gold = 0, Silicon = 3, Water = 0, Power = 0, CPU = 2, TileHP = 30 * 3, Research = 0, Internal_Name = "Door" };
        public static EntityCost Bed = new EntityCost() { Name = "Bed", Metal = 2, Gold = 0, Silicon = 0, Water = 0, Power = 0, CPU = 0, TileHP = 15 * 3, Research = 0, Internal_Name = "Bed" };
        public static EntityCost Chair = new EntityCost() { Name = "Chair", Metal = 2, Gold = 0, Silicon = 0, Water = 0, Power = 0, CPU = 0, TileHP = 2 * 3, Research = 0, Internal_Name = "Chair" };
        public static EntityCost Small_Table = new EntityCost() { Name = "Small_Table", Metal = 2, Gold = 0, Silicon = 0, Water = 0, Power = 0, CPU = 0, TileHP = 5 * 3, Research = 0, Internal_Name = "STable" };
        public static EntityCost Medium_Table = new EntityCost() { Name = "Medium_Table", Metal = 4, Gold = 0, Silicon = 0, Water = 0, Power = 0, CPU = 0, TileHP = 10 * 3, Research = 0, Internal_Name = "MTable" };
        public static EntityCost Large_Table = new EntityCost() { Name = "Large_Table", Metal = 8, Gold = 0, Silicon = 0, Water = 0, Power = 0, CPU = 0, TileHP = 20 * 3, Research = 0, Internal_Name = "LTable" };
        public static EntityCost Small_Laser = new EntityCost() { Name = "Small_Laser", Metal = 20, Gold = 2, Silicon = 10, Water = 0, Power = 10, CPU = 10, TileHP = 15 * 3, Research = 0, Internal_Name = "SLaser" };
        public static EntityCost Small_Plasma = new EntityCost() { Name = "Small_Plasma", Metal = 30, Gold = 5, Silicon = 30, Water = 0, Power = 20, CPU = 20, TileHP = 25 * 3, Research = 6, Internal_Name = "SPlasma" };
        public static EntityCost Small_EMP = new EntityCost() { Name = "Small_EMP", Metal = 30, Gold = 6, Silicon = 40, Water = 0, Power = 30, CPU = 30, TileHP = 25 * 3, Research = 6, Internal_Name = "SEMP" };
        public static EntityCost Small_Chaingun = new EntityCost() { Name = "Small_Chaingun", Metal = 30, Gold = 10, Silicon = 20, Water = 0, Power = 50, CPU = 50, TileHP = 30 * 3, Research = 4, Internal_Name = "SChaingun" };
        public static EntityCost Small_Rocket = new EntityCost() { Name = "Small_Rocket", Metal = 40, Gold = 15, Silicon = 30, Water = 0, Power = 30, CPU = 50, TileHP = 40 * 3, Research = 8, Internal_Name = "SRocket" };
        public static EntityCost Small_Torpedo = new EntityCost() { Name = "Small_Torpedo", Metal = 50, Gold = 30, Silicon = 50, Water = 0, Power = 50, CPU = 50, TileHP = 40 * 3, Research = 10, Internal_Name = "STorpedo" };
        public static EntityCost Small_Railgun = new EntityCost() { Name = "Small_Railgun", Metal = 100, Gold = 50, Silicon = 100, Water = 0, Power = 100, CPU = 100, TileHP = 50 * 3, Research = 15, Internal_Name = "SRailgun" };
        public static EntityCost Small_Mining_Laser = new EntityCost() { Name = "Small_Mining_Laser", Metal = 10, Gold = 0, Silicon = 5, Water = 0, Power = 7, CPU = 10, TileHP = 15 * 3, Research = 0, Internal_Name = "SMiningLaser" };
        public static EntityCost Small_Armor_Plate = new EntityCost() { Name = "Small_Armor_Plate", Metal = 20, Gold = 0, Silicon = 0, Water = 0, Power = 0, CPU = 0, TileHP = 30 * 3, Research = 4, Internal_Name = "SArmorPlate" };
        public static EntityCost Medium_Armor_Plate = new EntityCost() { Name = "Medium_Armor_Plate", Metal = 40, Gold = 0, Silicon = 0, Water = 0, Power = 0, CPU = 0, TileHP = 60 * 3, Research = 6, Internal_Name = "MArmorPlate" };
        public static EntityCost Large_Armor_Plate = new EntityCost() { Name = "Large_Armor_Plate", Metal = 80, Gold = 0, Silicon = 0, Water = 0, Power = 0, CPU = 0, TileHP = 120 * 3, Research = 8, Internal_Name = "LArmorPlate" };
        public static EntityCost Armor_Repairer = new EntityCost() { Name = "Armor_Repairer", Metal = 40, Gold = 10, Silicon = 20, Water = 0, Power = 40, CPU = 40, TileHP = 60 * 3, Research = 7, Internal_Name = "SArmorRepairer" };
        public static EntityCost Shield = new EntityCost() { Name = "Shield", Metal = 100, Gold = 15, Silicon = 25, Water = 0, Power = 50, CPU = 50, TileHP = 60 * 3, Research = 10, Internal_Name = "SShield" };
        public static EntityCost Shield_Boost = new EntityCost() { Name = "Shield_Boost", Metal = 80, Gold = 15, Silicon = 40, Water = 0, Power = 60, CPU = 60, TileHP = 60 * 3, Research = 12, Internal_Name = "SShieldBoost" };
        public static EntityCost Captains_Chair = new EntityCost() { Name = "Captains_Chair", Metal = 20, Gold = 10, Silicon = 20, Water = 0, Power = 30, CPU = 30, TileHP = 30 * 3, Research = 0, Internal_Name = "CaptainsChair" };
        public static EntityCost Health_Pad = new EntityCost() { Name = "Health_Pad", Metal = 40, Gold = 20, Silicon = 40, Water = 0, Power = 100, CPU = 100, TileHP = 30 * 3, Research = 10, Internal_Name = "HealthPad" };
        public static EntityCost Warp_Drive = new EntityCost() { Name = "Warp_Drive", Metal = 100, Gold = 70, Silicon = 200, Water = 0, Power = 200, CPU = 250, TileHP = 200 * 3, Research = 20, Internal_Name = "WarpDrive" };
        public static EntityCost Radar = new EntityCost() { Name = "Radar", Metal = 20, Gold = 10, Silicon = 20, Water = 0, Power = 20, CPU = 20, TileHP = 50 * 3, Research = 4, Internal_Name = "Radar" };
        public static EntityCost Communication_Array = new EntityCost() { Name = "Communication_Array", Metal = 20, Gold = 10, Silicon = 20, Water = 0, Power = 20, CPU = 20, TileHP = 50 * 3, Research = 4, Internal_Name = "CommsArray" };
        public static EntityCost CPU_Array = new EntityCost() { Name = "CPU_Array", Metal = 10, Gold = 10, Silicon = 35, Water = 0, Power = 10, CPU = 0, TileHP = 40 * 3, Research = 6, Internal_Name = "CPU" };
        public static EntityCost CPU_Panel = new EntityCost() { Name = "CPU_Panel", Metal = 5, Gold = 0, Silicon = 2, Water = 0, Power = 3, CPU = 0, TileHP = 5 * 3, Research = 0, Internal_Name = "CPUPanel" };
        public static EntityCost Water_Cooler = new EntityCost() { Name = "Water_Cooler", Metal = 2, Gold = 0, Silicon = 0, Water = 0, Power = 2, CPU = 0, TileHP = 5 * 3, Research = 0, Internal_Name = "WaterCooler" };
        public static EntityCost Hull_Brace = new EntityCost() { Name = "Hull_Brace", Metal = 10, Gold = 0, Silicon = 0, Water = 0, Power = 2, CPU = 0, TileHP = 10 * 3, Research = 2, Internal_Name = "HullBrace" };
        public static EntityCost Small_Heat_Vent = new EntityCost() { Name = "Small_Heat_Vent", Metal = 10, Gold = 0, Silicon = 2, Water = 0, Power = 5, CPU = 5, TileHP = 10 * 3, Research = 0, Internal_Name = "SHeatVent" };
        public static EntityCost Medium_Heat_Vent = new EntityCost() { Name = "Medium_Heat_Vent", Metal = 40, Gold = 2, Silicon = 8, Water = 0, Power = 15, CPU = 15, TileHP = 50 * 3, Research = 10, Internal_Name = "MHeatVent" };
        public static EntityCost Floor_Marking = new EntityCost() { Name = "Floor_Marking", Metal = 0, Gold = 0, Silicon = 0, Water = 0, Power = 0, CPU = 0, TileHP = 0, Research = 0, Internal_Name = "FloorMarking" };
        public static EntityCost Hull_Marking = new EntityCost() { Name = "Hull_Marking", Metal = 0, Gold = 0, Silicon = 0, Water = 0, Power = 0, CPU = 0, TileHP = 0, Research = 0, Internal_Name = "HullMarking" };
        public static EntityCost Mining_Extender = new EntityCost() { Name = "Mining_Extender", Metal = 15, Gold = 10, Silicon = 15, Water = 0, Power = 10, CPU = 10, TileHP = 10 * 3, Research = 15, Internal_Name = "MiningExtender" };
        public static EntityCost Energy_Penetration = new EntityCost() { Name = "Energy_Penetration", Metal = 15, Gold = 10, Silicon = 15, Water = 0, Power = 10, CPU = 10, TileHP = 10 * 3, Research = 15, Internal_Name = "EnergyPenetration" };
        public static EntityCost Projectile_Penetration = new EntityCost() { Name = "Projectile_Penetration", Metal = 15, Gold = 10, Silicon = 15, Water = 0, Power = 10, CPU = 10, TileHP = 10 * 3, Research = 15, Internal_Name = "ProjectilePenetration" };
        public static EntityCost Explosive_Penetration = new EntityCost() { Name = "Explosive_Penetration", Metal = 15, Gold = 10, Silicon = 15, Water = 0, Power = 10, CPU = 10, TileHP = 10 * 3, Research = 15, Internal_Name = "ExplosivePenetration" };
        public static EntityCost Shield_Amplifier = new EntityCost() { Name = "Shield_Amplifier", Metal = 15, Gold = 10, Silicon = 15, Water = 0, Power = 10, CPU = 10, TileHP = 10 * 3, Research = 15, Internal_Name = "ShieldAmplifier" };
        public static EntityCost Armor_Amplifier = new EntityCost() { Name = "Armor_Amplifier", Metal = 15, Gold = 10, Silicon = 15, Water = 0, Power = 10, CPU = 10, TileHP = 10 * 3, Research = 15, Internal_Name = "ArmorAmplifier" };
        public static EntityCost Ship_Core = new EntityCost() { Name = "Ship_Core", Metal = 200, Gold = 50, Silicon = 100, Water = 0, Power = 0, CPU = 0, TileHP = 200 * 3, Research = 50, Internal_Name = "ShipCore" };

        private static Dictionary<String, EntityCost> _TranslateTable;

        static Costs()
        {
            UnityEngine.Debug.Log("--Initializing Costs--");
            _TranslateTable = new Dictionary<String, EntityCost>();

            SetEntityCost(Hull);
            SetEntityCost(HullCorner);
            SetEntityCost(Floor);
            SetEntityCost(Small_Solar);
            SetEntityCost(Medium_Solar);
            SetEntityCost(Small_Reactor);
            SetEntityCost(Medium_Reactor);
            SetEntityCost(Large_Reactor);
            SetEntityCost(Mini_Engine);
            SetEntityCost(Small_Engine);
            SetEntityCost(Medium_Engine);
            SetEntityCost(Large_Engine);
            SetEntityCost(Life_Support);
            SetEntityCost(Airlock);
            SetEntityCost(Food_Dispenser);
            SetEntityCost(Small_Plant);
            SetEntityCost(Medium_Plant);
            SetEntityCost(Large_Plant);
            SetEntityCost(Cargo_Rack);
            SetEntityCost(Small_Crate);
            SetEntityCost(Medium_Crate);
            SetEntityCost(Large_Crate);
            SetEntityCost(Navigation_Console);
            SetEntityCost(Radar_Console);
            SetEntityCost(Targeting_Console);
            SetEntityCost(Shield_Console);
            SetEntityCost(Armor_Console);
            SetEntityCost(Communication_Console);
            SetEntityCost(Heat_Console);
            SetEntityCost(Research_Console);
            SetEntityCost(Mining_Console);
            SetEntityCost(Door);
            SetEntityCost(Bed);
            SetEntityCost(Chair);
            SetEntityCost(Small_Table);
            SetEntityCost(Medium_Table);
            SetEntityCost(Large_Table);
            SetEntityCost(Small_Laser);
            SetEntityCost(Small_Plasma);
            SetEntityCost(Small_EMP);
            SetEntityCost(Small_Chaingun);
            SetEntityCost(Small_Rocket);
            SetEntityCost(Small_Torpedo);
            SetEntityCost(Small_Railgun);
            SetEntityCost(Small_Mining_Laser);
            SetEntityCost(Small_Armor_Plate);
            SetEntityCost(Medium_Armor_Plate);
            SetEntityCost(Large_Armor_Plate);
            SetEntityCost(Armor_Repairer);
            SetEntityCost(Shield);
            SetEntityCost(Shield_Boost);
            SetEntityCost(Captains_Chair);
            SetEntityCost(Health_Pad);
            SetEntityCost(Warp_Drive);
            SetEntityCost(Radar);
            SetEntityCost(Communication_Array);
            SetEntityCost(CPU_Array);
            SetEntityCost(CPU_Panel);
            SetEntityCost(Water_Cooler);
            SetEntityCost(Hull_Brace);
            SetEntityCost(Small_Heat_Vent);
            SetEntityCost(Medium_Heat_Vent);
            SetEntityCost(Floor_Marking);
            SetEntityCost(Hull_Marking);
            SetEntityCost(Mining_Extender);
            SetEntityCost(Energy_Penetration);
            SetEntityCost(Projectile_Penetration);
            SetEntityCost(Explosive_Penetration);
            SetEntityCost(Shield_Amplifier);
            SetEntityCost(Armor_Amplifier);
            SetEntityCost(Ship_Core);

            Data.SaveLoadData.AddType(new Data.ES2_UserType_CostSaveData());
        }

        public Costs()
        {

        }

        public static EntityCost[] GetEntityCosts()
        {
            return _TranslateTable.Values.ToArray();
        }

        public static EntityCost GetEntityCost(String name)
        {
            return _TranslateTable.Where(p => p.Key == name || p.Value.Internal_Name == name || p.Value.Name == name).Select(p => p.Value).FirstOrDefault();
        }

        public static void SetEntityCost(EntityCost cost, String displayName = "")
        {
            if (String.IsNullOrEmpty(cost.Internal_Name))
                throw new InvalidOperationException("EntityCost requires an internal name");

            if (String.IsNullOrEmpty(displayName))
                displayName = cost.Name;

            cost.Name = displayName;
            _TranslateTable[cost.Internal_Name] = cost;
        }

        public int GetResourceRequirements(String type, String resource)
        {
            if (String.IsNullOrEmpty(type) || String.IsNullOrEmpty(resource))
                return 0;

            EntityCost cost = GetEntityCost(type);
            if (cost != null)
                return cost.GetCost(resource);

            UnityEngine.Debug.LogWarning("Resource requirement not defined: " + type);
            return 0;
        }

    }

    [Serializable]
    public class EntityCost
    {
        public String Name;
        public int Metal;
        public int Gold;
        public int Silicon;
        public int Water;
        public int Power;
        public int CPU;
        public int TileHP;
        public int Research;
        public String Internal_Name;

        public int GetCost(String resource)
        {
            String lResource = resource.ToLower();
            if (lResource == "metal")
                return this.Metal;
            else if (lResource == "gold")
                return this.Gold;
            else if (lResource == "silicon")
                return this.Silicon;
            else if (lResource == "water")
                return this.Water;
            else if (lResource == "power")
                return this.Power;
            else if (lResource == "cpu")
                return this.CPU;
            else if (lResource == "tilehp")
                return this.TileHP;
            else if (lResource == "research")
                return this.Research;

            return 99;
        }

        public void SetCost(String resource, int value)
        {
            String lResource = resource.ToLower();
            if (lResource == "metal")
                this.Metal = value;
            else if (lResource == "gold")
                this.Gold = value;
            else if (lResource == "silicon")
                this.Silicon = value;
            else if (lResource == "water")
                this.Water = value;
            else if (lResource == "power")
                this.Power = value;
            else if (lResource == "cpu")
                this.CPU = value;
            else if (lResource == "tilehp")
                this.TileHP = value;
            else if (lResource == "research")
                this.Research = value;
        }

    }
}

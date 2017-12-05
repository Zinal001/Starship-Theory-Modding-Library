using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarshipTheory.ModLib.Resources
{
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

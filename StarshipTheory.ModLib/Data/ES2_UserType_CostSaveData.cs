using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarshipTheory.ModLib.Data
{
    public class ES2_UserType_CostSaveData : ES2Type
    {
        public ES2_UserType_CostSaveData() : base(typeof(Resources.EntityCost))
        {

        }

        public override object Read(ES2Reader reader)
        {
            Resources.EntityCost cost = new Resources.EntityCost();
            this.Read(reader, cost);
            return cost;
        }

        public override void Read(ES2Reader reader, object c)
        {
            Resources.EntityCost cost = (Resources.EntityCost)c;
            cost.Name = reader.Read<String>();
            cost.Metal = reader.Read<int>();
            cost.Gold = reader.Read<int>();
            cost.Silicon = reader.Read<int>();
            cost.Water = reader.Read<int>();
            cost.Power = reader.Read<int>();
            cost.CPU = reader.Read<int>();
            cost.TileHP = reader.Read<int>();
            cost.Research = reader.Read<int>();
            cost.Internal_Name = reader.Read<String>();
        }

        public override void Write(object data, ES2Writer writer)
        {
            Resources.EntityCost cost = data as Resources.EntityCost;
            writer.Write<String>(cost.Name);
            writer.Write<int>(cost.Metal);
            writer.Write<int>(cost.Gold);
            writer.Write<int>(cost.Silicon);
            writer.Write<int>(cost.Water);
            writer.Write<int>(cost.Power);
            writer.Write<int>(cost.CPU);
            writer.Write<int>(cost.TileHP);
            writer.Write<int>(cost.Research);
            writer.Write<String>(cost.Internal_Name);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ExporterImporter.Mod
{
    public class TileData2
    {
        public int X { get; set; }
        public int Y { get; set; }
        public BuildData.BuildDataRotation Rotation { get; set; }
        public String Type { get; set; }

        public int NumParts { get; set; }

        public TileData2()
        {
        }

        public TileData2(int X, int Y, String Type) : this()
        {
            this.X = X;
            this.Y = Y;
            this.Type = Type;
        }

        public TileData2(int X, int Y, String Type, BuildData.BuildDataRotation Rotation) : this(X, Y, Type)
        {
            this.Rotation = Rotation;
        }
    }

    public class TileData
    {
        public int X { get; set; }
        public int Y { get; set; }
        public BuildData.BuildDataRotation Rotation { get; set; }
        public String TileType { get; set; }
        public String StructureType { get; set; }

        public List<Vec3> StructureParts { get; set; }

        public class Vec3
        {
            public float X { get; set; }
            public float Y { get; set; }
            public float Z { get; set; }

            public Vec3()
            {

            }

            public static Vec3 FromVector(Vector3 vec)
            {
                return new Vec3() { X = vec.x, Y = vec.y, Z = vec.z };
            }

            public Vector3 GetVector()
            {
                return new Vector3(X, Y, Z);
            }
        }

    }
}

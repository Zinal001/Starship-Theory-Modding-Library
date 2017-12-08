using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ExporterImporter.Mod
{
    public class TileData
    {
        public int X { get; set; }
        public int Y { get; set; }
        public TileRotation Rotation { get; set; }
        public String TileType { get; set; }

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


        public enum TileRotation : int
        {
            UP = 0,
            RIGHT = 1,
            DOWN = 2,
            LEFT = 3
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ExporterImporter.Mod
{
    public class TileData
    {
        public float X { get; set; }
        public float Y { get; set; }
        public String Type { get; set; }

        public List<StructData> StructDatas { get; private set; }

        public List<StructureData> Structures { get; private set; }

        public TileData()
        {
            this.StructDatas = new List<StructData>();
            this.Structures = new List<StructureData>();
        }

        public TileData(float X, float Y, String Type) : this()
        {
            this.X = X;
            this.Y = Y;
            this.Type = Type;
        }


        public void WriteTo(StreamWriter Writer)
        {
            Writer.WriteLine(String.Format("{0} {1} {2} {3} {4} {5}", X, Y, Type, StructDatas.Count, Structures.Count));

            if(StructDatas.Count > 0)
            {
                for (int i = 0; i < StructDatas.Count; i++)
                    Writer.WriteLine(String.Format("{0} {1} {2}", StructDatas[i].X, StructDatas[i].Y, StructDatas[i].Rotation));
            }

            if(Structures.Count > 0)
            {
                for (int i = 0; i < Structures.Count; i++)
                    Writer.WriteLine(String.Format("{0} {1} {2}", Structures[i].X, Structures[i].Y, Structures[i].Type));
            }            
        }

        public static TileData ReadFrom(StreamReader Reader)
        {
            try
            {
                TileData Res = new TileData();
                String Line = Reader.ReadLine();
                String[] Parts = Line.Trim().Split(' ');

                Res.X = float.Parse(Parts[0]);
                Res.Y = float.Parse(Parts[1]);
                Res.Type = Parts[2];

                int numStructs = int.Parse(Parts[4]);
                for(int i = 0; i < numStructs; i++)
                {
                    StructData sd = StructData.ReadFrom(Reader);
                    if (sd != null)
                        Res.StructDatas.Add(sd);
                }

                int numStructures = int.Parse(Parts[5]);
                for(int i = 0; i < numStructures; i++)
                {
                    StructureData sd = StructureData.ReadFrom(Reader);
                    if (sd != null)
                        Res.Structures.Add(sd);
                }

                return Res;
            }
            catch { }

            return null;
        }

        public class StructData
        {
            public float X { get; set; }
            public float Y { get; set; }
            public float Rotation { get; set; }

            public StructData()
            {

            }

            public StructData(float X, float Y, float Rotation)
            {
                this.X = X;
                this.Y = Y;
                this.Rotation = Rotation;
            }

            public static StructData ReadFrom(StreamReader Reader)
            {
                try
                {
                    StructData Res = new StructData();
                    String Line = Reader.ReadLine();
                    String[] Parts = Line.Trim().Split(' ');

                    Res.X = float.Parse(Parts[0]);
                    Res.Y = float.Parse(Parts[1]);
                    Res.Rotation = float.Parse(Parts[2]);

                    return Res;
                }
                catch { }

                return null;
            }
        }
    }

    public class StructureData
    {
        public float X { get; set; }
        public float Y { get; set; }
        public String Type { get; set; }

        public StructureData()
        {

        }

        public StructureData(float X, float Y, String Type)
        {
            this.X = X;
            this.Y = Y;
            this.Type = Type;
        }

        public static StructureData ReadFrom(StreamReader Reader)
        {
            try
            {
                StructureData Res = new StructureData();
                String Line = Reader.ReadLine();
                String[] Parts = Line.Trim().Split(' ');

                Res.X = float.Parse(Parts[0]);
                Res.Y = float.Parse(Parts[1]);
                Res.Type = Parts[2];

                return Res;
            }
            catch { }

            return null;
        }
    }
}

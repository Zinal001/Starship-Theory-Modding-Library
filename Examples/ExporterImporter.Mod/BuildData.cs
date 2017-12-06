using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ExporterImporter.Mod
{
    public class BuildData
    {
        public String Type { get; set; }
        public int Rotation { get; set; }

        public Part[] Parts { get; set; }

        public BuildData()
        {
            this.Parts = new Part[0];
        }

        public class Part
        {
            public int RelativeX { get; set; }
            public int RelativeY { get; set; }
        }

        public static List<BuildData> ReadFrom(StreamReader reader)
        {
            List<BuildData> data = new List<BuildData>();

            BuildData current = new BuildData();
            List<Part> parts = new List<Part>();

            while(!reader.EndOfStream)
            {
                String line = reader.ReadLine();
                if(String.IsNullOrEmpty(line))
                {
                    if (_VerifyData(current))
                        data.Add(current);

                    current = new BuildData();
                    parts.Clear();
                }
                else if (line.StartsWith("Type:"))
                    current.Type = line.Substring(5).Trim();
                else if (line.StartsWith("Rotation:"))
                    current.Rotation = int.Parse(line.Substring(9).Trim());
                else if (line.StartsWith("Part:"))
                {
                    String[] pos = line.Substring(5).Trim().Split(',');
                    if (pos.Length == 2)
                        parts.Add(new Part() { RelativeX = int.Parse(pos[0].Trim()), RelativeY = int.Parse(pos[1].Trim()) });
                }
            }

            if (_VerifyData(current) && !data.Contains(current))
                data.Add(current);

            return data;
        }

        public static BuildData ReadFrom(String filename)
        {
            BuildData bd = new BuildData();

            List<Part> parts = new List<Part>();
            using (StreamReader reader = new StreamReader(filename, Encoding.UTF8))
            {
                while(!reader.EndOfStream)
                {
                    String line = reader.ReadLine();
                    if (line.StartsWith("Type:"))
                        bd.Type = line.Substring(5).Trim();
                    else if (line.StartsWith("Rotation:"))
                        bd.Rotation = int.Parse(line.Substring(9).Trim());
                    else if (line.StartsWith("Part:"))
                    {
                        String[] pos = line.Substring(5).Trim().Split(',');
                        if (pos.Length == 2)
                            parts.Add(new Part() {  RelativeX = int.Parse(pos[0].Trim()), RelativeY = int.Parse(pos[1].Trim())});
                    }
                }
            }

            bd.Parts = parts.ToArray();

            if (!_VerifyData(bd))
                return null;

            return bd;
        }

        private static bool _VerifyData(BuildData bd)
        {
            return !String.IsNullOrEmpty(bd.Type);
        }
    }


}

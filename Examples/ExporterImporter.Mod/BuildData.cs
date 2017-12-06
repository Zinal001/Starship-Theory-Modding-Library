using System;
using System.Collections.Generic;
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
    }


}

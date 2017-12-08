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
        public bool AllowRotation { get; set; }

        public Part[] Parts { get; set; }

        public BuildData()
        {

        }

        private static bool _VerifyData(BuildData bd)
        {
            return !String.IsNullOrEmpty(bd.Type);
        }

        public enum BuildDataRotation : int
        {
            UP = 0,
            RIGHT = 1,
            DOWN = 2,
            LEFT = 3
        }

        public class Part
        {
            public int RelativeX { get; set; }
            public int RelativeY { get; set; }
        }

    }


}

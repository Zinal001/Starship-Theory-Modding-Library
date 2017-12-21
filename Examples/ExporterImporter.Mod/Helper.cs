using System;
using System.Collections.Generic;

namespace ExporterImporter.Mod
{
    internal static class Helper
    {
        public static List<T> Copy<T>(IEnumerable<T> lst)
        {
            List<T> nLst = new List<T>();
            nLst.AddRange(lst);
            return nLst;
        }

    }
}

using System;
using System.Collections.Generic;

namespace ExporterImporter.Mod
{
    internal static class Extensions
    {

        public static IEnumerable<T> Where<T>(this IEnumerable<T> e, Func<T, bool> predice)
        {
            List<T> coll = new List<T>();
            foreach(T elm in e)
            {
                if (predice(elm))
                    coll.Add(elm);
            }

            return coll;
        }

        public static List<T> Copy<T>(IEnumerable<T> lst)
        {
            List<T> nLst = new List<T>();
            nLst.AddRange(lst);
            return nLst;
        }

    }
}

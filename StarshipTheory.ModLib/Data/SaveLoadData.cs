using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarshipTheory.ModLib.Data
{
    /// <summary>
    /// 
    /// </summary>
    public static class SaveLoadData
    {
        private static List<ES2Type> _Types;

        public static ES2Type[] RegisteredTypes
        {
            get { return _Types.ToArray(); }
        }

        static SaveLoadData()
        {
            _Types = new List<ES2Type>();
        }

        public static void AddType(ES2Type type)
        {
            if (ES2TypeManager.types == null)
                ES2TypeManager.types = new Dictionary<Type, ES2Type>();

            ES2TypeManager.types[type.type] = type;

            if (_Types.Where(t => t.type.FullName == type.type.FullName).Count() > 0)
                return;

            _Types.Add(type);
        }

        /// <summary>
        /// <para>DO NOT CALL THIS FUNCTION DIRECTLY</para>
        /// </summary>
        public static void __Init()
        {
            if (ES2TypeManager.types == null)
                ES2TypeManager.types = new Dictionary<Type, ES2Type>();

            foreach (ES2Type T in _Types)
                ES2TypeManager.types[T.type] = T;

            UnityEngine.Debug.Log("Initialized " + _Types.Count + " save/load types");
        }


    }
}

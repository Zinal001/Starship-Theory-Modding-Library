using Pathfinding.Serialization.JsonFx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarshipTheory.ModLib.Util
{
    public static class SerializationUtil
    {
        private static List<JsonConverter> _LoadedConverters;

        public static JsonConverter[] LoadedConverters
        {
            get { return _LoadedConverters.ToArray(); }
        }

        public static JsonReaderSettings ReaderSettings { get; private set; }
        public static JsonWriterSettings WriterSettings { get; private set; }

        static SerializationUtil()
        {
            _LoadedConverters = new List<JsonConverter>();
            ReaderSettings = new JsonReaderSettings() { AllowNullValueTypes = true, AllowUnquotedObjectKeys = false, HandleCyclicReferences = true };
            WriterSettings = new JsonWriterSettings() { HandleCyclicReferences = true, MaxDepth = 50, NewLine = "\n", PrettyPrint = true, Tab = "\t" };

            AddConvertersFromAssembly(typeof(SerializationUtil).Assembly);
        }

        public static void Initialize()
        {

        }

        public static T Deserialize<T>(String json)
        {
            JsonReader reader = new JsonReader(json, ReaderSettings);
            return (T)reader.Deserialize(typeof(T));
        }

        public static String Serialize(Object obj)
        {
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(ms))
                {
                    using (JsonWriter jsonWriter = new JsonWriter(writer, WriterSettings))
                        jsonWriter.Write(obj);
                }

                return Encoding.UTF8.GetString(ms.ToArray());
            }            

        }

        public static bool AddConverter(JsonConverter converter)
        {
            if (!_LoadedConverters.Contains(converter))
            {
                _LoadedConverters.Add(converter);
                ReaderSettings.AddTypeConverter(converter);
                WriterSettings.AddTypeConverter(converter);

                return true;
            }

            return false;
        }

        public static void AddConvertersFromAssembly(System.Reflection.Assembly assembly)
        {
            Type[] converterTypes = assembly.GetTypes().Where(t => t.IsPublic && !t.IsAbstract && !t.IsInterface && t.IsSubclassOf(typeof(JsonConverter))).ToArray();
            int convertersAdded = 0;

            foreach(Type t in converterTypes)
            {
                try
                {
                    JsonConverter inst = (JsonConverter)Activator.CreateInstance(t);
                    if (inst != null)
                    {
                        if (AddConverter(inst))
                            convertersAdded++;
                    }
                }
                catch(Exception ex)
                {
                    UnityEngine.Debug.LogWarning("Failed to instantiation JsonConverter class " + t.FullName + ": " + ex.Message);
                }
            }
        }


    }
}

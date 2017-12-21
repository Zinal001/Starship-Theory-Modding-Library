using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarshipTheory.ModLib.Converters.Json
{
    public class UnityColorConverter : Pathfinding.Serialization.JsonFx.JsonConverter
    {
        public override bool CanConvert(Type t)
        {
            return (t.FullName == typeof(UnityEngine.Color).FullName);   
        }

        public override object ReadJson(Type type, Dictionary<string, object> value)
        {
            if((type.FullName == typeof(UnityEngine.Color).FullName))
            {
                UnityEngine.Color c = new UnityEngine.Color(ToFloat(value["r"]), ToFloat(value["g"]), ToFloat(value["b"]), ToFloat(value["a"]));
                return c;
            }

            return default(UnityEngine.Color);
        }

        public override Dictionary<string, object> WriteJson(Type type, object value)
        {
            if (value is UnityEngine.Color)
            {
                return new Dictionary<string, object>() {
                    { "r", ((UnityEngine.Color)value).r },
                    { "g", ((UnityEngine.Color)value).g },
                    { "b", ((UnityEngine.Color)value).b },
                    { "a", ((UnityEngine.Color)value).a }
                };
            }

            return new Dictionary<string, object>();
        }

        private static float ToFloat(Object obj)
        {
            if (obj is int)
                return (int)obj;
            else if (obj is Single)
                return (Single)obj;
            else if (obj is Double)
                return (float)(Double)obj;

            return float.NaN;
        }
    }
}

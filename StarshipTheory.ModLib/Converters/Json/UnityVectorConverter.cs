using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarshipTheory.ModLib.Converters.Json
{
    public class UnityVectorConverter : Pathfinding.Serialization.JsonFx.JsonConverter
    {
        public override bool CanConvert(Type t)
        {
            return (t.FullName == typeof(UnityEngine.Vector2).FullName) || (t.FullName == typeof(UnityEngine.Vector3).FullName) || (t.FullName == typeof(UnityEngine.Vector4).FullName);
        }

        public override object ReadJson(Type type, Dictionary<string, object> value)
        {
            if (type.FullName == typeof(UnityEngine.Vector2).FullName)
                return new UnityEngine.Vector2(ToFloat(value["x"]), ToFloat(value["y"]));
            else if (type.FullName == typeof(UnityEngine.Vector3).FullName)
                return new UnityEngine.Vector3(ToFloat(value["x"]), ToFloat(value["y"]), ToFloat(value["z"]));
            else if (type.FullName == typeof(UnityEngine.Vector4).FullName)
                return new UnityEngine.Vector4(ToFloat(value["x"]), ToFloat(value["y"]), ToFloat(value["z"]), ToFloat(value["w"]));

            return null;
        }

        public override Dictionary<string, object> WriteJson(Type type, object value)
        {
            if (type.FullName == typeof(UnityEngine.Vector2).FullName)
            {
                return new Dictionary<string, object>() {
                    { "x", ((UnityEngine.Vector2)value).x },
                    { "y", ((UnityEngine.Vector2)value).y }
                };
            }
            else if (type.FullName == typeof(UnityEngine.Vector3).FullName)
            {
                return new Dictionary<string, object>() {
                    { "x", ((UnityEngine.Vector3)value).x },
                    { "y", ((UnityEngine.Vector3)value).y },
                    { "z", ((UnityEngine.Vector3)value).z }
                };
            }
            else if (type.FullName == typeof(UnityEngine.Vector4).FullName)
            {
                return new Dictionary<string, object>() {
                    { "x", ((UnityEngine.Vector4)value).x },
                    { "y", ((UnityEngine.Vector4)value).y },
                    { "z", ((UnityEngine.Vector4)value).z },
                    { "w", ((UnityEngine.Vector4)value).w }
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

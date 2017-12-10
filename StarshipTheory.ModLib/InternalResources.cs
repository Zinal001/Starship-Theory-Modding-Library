using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarshipTheory.ModLib
{
    internal static class InternalResources
    {
        public static byte[] ReadResource(String name)
        {
            //return Properties.Resources.ResizeHandle;
            return (byte[])Properties.Resources.ResourceManager.GetObject(name);
        }

        public static UnityEngine.Texture2D ReadResourceAsTexture2D(String name)
        {
            byte[] data = ReadResource(name);

            UnityEngine.Texture2D tex = new UnityEngine.Texture2D(1, 1);
            if (tex.LoadImage(data))
            {
                tex.Apply(true);
                return tex;
            }

            return null;
        }
    }
}

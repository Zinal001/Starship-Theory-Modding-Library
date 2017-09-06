﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarshipTheory.ModLib
{
    public sealed class ModGUI : UnityEngine.MonoBehaviour
    {
        private static UnityEngine.GUISkin _DefaultSkin = null;

        private static int _WindowIndex = -1;

        /// <summary>
        /// Returns a new index for GUI.Window's
        /// <see>https://docs.unity3d.com/Manual/gui-Controls.html#Window</see>
        /// </summary>
        /// <returns></returns>
        public static int GetWindowIndex()
        {
            _WindowIndex++;
            return _WindowIndex;
        }

        private int tick = 0;


        /// <summary>
        /// <para>Called from within the game's code. Creates the ModGUI object which handles all custom gui.</para>
        /// <para>Do not call this manually</para>
        /// </summary>
        public static void __Create()
        {
            UnityEngine.GameObject modGui = UnityEngine.GameObject.Find("ModGUI");
            if (modGui == null)
            {
                UnityEngine.Debug.Log("Created Mod GUI");
                modGui = new UnityEngine.GameObject("ModGUI");
                modGui.AddComponent<ModGUI>();
            }
        }

        /// <summary>
        /// This method is called by Unity each frame
        /// </summary>
        void OnGUI()
        {
            if (tick > 10000)
                tick = 0;

            if (_DefaultSkin == null)
                _DefaultSkin = UnityEngine.GUI.skin;

            if (tick % 10 == 0)
                UnityEngine.GUI.skin = _DefaultSkin;

            ModLoader.Instance.__OnGui();
            tick++;
        }
    }
}
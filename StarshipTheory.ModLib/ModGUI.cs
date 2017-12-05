using System;
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

        private static long _tick = 0;

        public static long Tick
        {
            get { return _tick; }
            private set { _tick = value; }
        }


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
            if (_tick > 100000)
                _tick = 0;

            if (_DefaultSkin == null)
                _DefaultSkin = UnityEngine.GUI.skin;

            if (_tick % 10 == 0)
                UnityEngine.GUI.skin = _DefaultSkin;

            ModLoader.Instance.__OnGui();
            _tick++;

            foreach (AbstractMod M in ModLoader.Instance.Mods.Where(m => m.Enabled))
            {
                try
                {
                    if (!M._FirstGuiPassCalled)
                    {
                        M.FirstGUIPass();
                        M._FirstGuiPassCalled = true;
                    }

                    if (ManagerMenu.mainMenuActive && M.CanShowModWindow)
                        M.ModWindow.__Draw();

                    M.OnGUI();
                }
                catch (Exception ex)
                {
                    //TODO: Show Mod error window
                    ModLoader.Instance.ShowError(M, ex, "GUI");
                    UnityEngine.Debug.LogError(ex);
                }
            }


        }
    }
}

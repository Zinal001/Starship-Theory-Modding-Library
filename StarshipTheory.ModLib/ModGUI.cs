using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarshipTheory.ModLib
{
    public sealed class ModGUI : UnityEngine.MonoBehaviour
    {
        private static UnityEngine.GUISkin _DefaultSkin = null;
        public static UnityEngine.GUIStyle TrasparentGUIStyle { get; private set; }

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

        private static List<GUI.IResizeable> _ResizeHandles = new List<GUI.IResizeable>();

        public static void RegisterResize(GUI.IResizeable resizeable)
        {
            _ResizeHandles.Add(resizeable);
        }

        public static void UnregisterResize(GUI.IResizeable resizeable)
        {
            _ResizeHandles.Remove(resizeable);
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

            if (TrasparentGUIStyle == null)
            {
                UnityEngine.Texture2D emptyText = InternalResources.ReadResourceAsTexture2D("Transparent");

                TrasparentGUIStyle = new UnityEngine.GUIStyle();
                TrasparentGUIStyle.active.background = emptyText;
                TrasparentGUIStyle.hover.background = emptyText;
                TrasparentGUIStyle.normal.background = emptyText;
                TrasparentGUIStyle.onActive.background = emptyText;
                TrasparentGUIStyle.onFocused.background = emptyText;
                TrasparentGUIStyle.onHover.background = emptyText;
                TrasparentGUIStyle.onNormal.background = emptyText;
            }

            if (_tick > 100000)
                _tick = 0;

            if (_DefaultSkin == null)
                _DefaultSkin = UnityEngine.GUI.skin;

            if (_tick % 10 == 0)
                UnityEngine.GUI.skin = _DefaultSkin;

            ModLoader.Instance.__OnGui();
            _tick++;

            if(_ResizeHandles.Count > 0)
            {
                UnityEngine.GUI.Box(new UnityEngine.Rect(0, 0, UnityEngine.Screen.width, UnityEngine.Screen.height), "", TrasparentGUIStyle);

                if ((UnityEngine.Event.current.type == UnityEngine.EventType.mouseUp && UnityEngine.Event.current.button == 0))
                {
                    GUI.IResizeable[] resizeables = new GUI.IResizeable[_ResizeHandles.Count];
                    _ResizeHandles.CopyTo(resizeables, 0);

                    foreach (GUI.IResizeable resizeable in resizeables)
                        resizeable.ResizeEnd(UnityEngine.Event.current.mousePosition);
                }

            }

            foreach (Mod M in ModLoader.Instance.Mods.Where(m => m.Enabled))
            {
                try
                {
                    if (!M._FirstGuiPassCalled)
                    {
                        M.OnCreateGUI();
                        M._FirstGuiPassCalled = true;
                    }

                    if (ManagerMenu.mainMenuActive && M.CanShowModWindow)
                        M.ModWindow.__Draw();

                    M.OnGUI();
                }
                catch (Exception ex)
                {
                    ModLoader.Instance.ShowError(M, ex, "GUI");
                    M._Logger.LogException(ex);
                }
            }


        }
    }
}

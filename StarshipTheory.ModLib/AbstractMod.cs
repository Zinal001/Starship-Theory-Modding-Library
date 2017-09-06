using System;
using System.Collections.Generic;
using System.Text;

namespace StarshipTheory.ModLib
{
    /// <summary>
    /// <para>The abstract Mod class.</para>
    /// <para>Note: The exported dll file needs to end with .Mod.dll for the modloader to recognize it as a mod</para>
    /// </summary>
    public abstract class AbstractMod
    {
        /// <summary>
        /// The Display name of the mod
        /// </summary>
        public abstract String ModName { get; }

        /// <summary>
        /// The Version of the mod
        /// </summary>
        public abstract Version ModVersion { get; }

        /// <summary>
        /// Whenever this mod is enabled or not (Defaults to true)
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// The full path to the directory the .dll file is in
        /// </summary>
        public String ModFolder { get; internal set; }

        /// <summary>
        /// The GUI window for this mod
        /// </summary>
        public GUI.Window ModWindow { get; internal set; }


        public AbstractMod()
        {

        }

        /// <summary>
        /// <para>Is called when the mod is loaded and initialized by the modloader</para>
        /// <para>Note: This doesn't mean that the game is up and running</para>
        /// </summary>
        public abstract void OnInitialize();

        /// <summary>
        /// This method is called when a new game is started
        /// </summary>
        public virtual void OnGameStarted() { }

        /// <summary>
        /// This method is called when a game is being loaded from a previous save
        /// </summary>
        /// <param name="saveSlot"></param>
        public virtual void OnGameLoad(int saveSlot) { }

        /// <summary>
        /// This method is called when a game is being saved to a slot
        /// </summary>
        /// <param name="saveSlot"></param>
        public virtual void OnGameSave(int saveSlot) { }

        /// <summary>
        /// This method is called by the modloader whenever custom gui should be drawn.
        /// <see>https://docs.unity3d.com/Manual/GUIScriptingGuide.html</see>
        /// </summary>
        public virtual void OnGUI() { }


        internal void ToggleModWindow(UnityEngine.Rect? position = null)
        {
            ModWindow.Visible = !ModWindow.Visible;
            if (ModWindow.Visible && position.HasValue)
                ModWindow.Rect = position.Value;
        }
    }
}

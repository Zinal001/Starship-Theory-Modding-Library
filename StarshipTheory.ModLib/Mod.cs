using System;
using System.Collections.Generic;
using System.Text;

namespace StarshipTheory.ModLib
{
    /// <summary>
    /// <para>The abstract Mod class.</para>
    /// <para>Note: The exported dll filename needs to end with .Mod.dll for the modloader to recognize it as a mod</para>
    /// </summary>
    public abstract class Mod
    {
        public ModInfo Info { get; internal set; }

        /// <summary>
        /// Whenever this mod is enabled or not (Defaults to true)
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// The full path to the directory the .dll file is in
        /// </summary>
        public String ModFolder { get; internal set; }

        /// <summary>
        /// The GUI window for this mod
        /// </summary>
        public GUI.Window ModWindow { get; internal set; }

        /// <summary>
        /// <para>Can the GUI Window for this mod be shown?</para>
        /// <para>Defaults to true</para>
        /// </summary>
        public bool CanShowModWindow { get; set; } = true;


        internal UnityEngine.Logger _Logger;
        /// <summary>
        /// 
        /// </summary>
        protected UnityEngine.Logger Logger
        {
            get { return _Logger; }
        }

        internal bool _FirstGuiPassCalled = false;

        public Mod()
        {

        }

        /// <summary>
        /// <para>Is called when the mod is loaded and initialized by the modloader</para>
        /// <para>Note: This doesn't mean that the game is up and running</para>
        /// </summary>
        public abstract void OnInitialize();

        /// <summary>
        /// <para>This method is called by the modloader at the first gui pass.</para>
        /// <para>Use this method to create your desired mod window components.</para>
        /// </summary>
        public virtual void OnCreateGUI() { }

        /// <summary>
        /// This method is called by the modloader whenever custom gui should be drawn.
        /// <see>https://docs.unity3d.com/Manual/GUIScriptingGuide.html</see>
        /// </summary>
        public virtual void OnGUI() { }

        /// <summary>
        /// Used to display an in-game error window.
        /// </summary>
        /// <param name="error"></param>
        protected void DisplayError(Exception error)
        {
            ModLoader.Instance.ShowError(this, error);
        }

        internal void ToggleModWindow(UnityEngine.Rect? position = null)
        {
            ModWindow.Visible = !ModWindow.Visible;
            if (ModWindow.Visible && position.HasValue)
                ModWindow.Rect = position.Value;
        }
    }
}

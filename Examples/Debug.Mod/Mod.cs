using System;
using StarshipTheory.ModLib;
using UnityEngine;

namespace Debug.Mod
{
    public class Mod : AbstractMod
    {
        /// <summary>
        /// The display name of the mod
        /// </summary>
        public override string ModName => "Debug Mod";

        /// <summary>
        /// The current version of the mod
        /// </summary>
        public override Version ModVersion => new Version("1.0.0");

        private GameObject _Manager;

        private StarshipTheory.ModLib.GUI.Button _ToggleBtn;

        /// <summary>
        /// <para>OnInitialize is called when the modloader loads the mods</para>
        /// <para>Note: This function may be called several times due to how the in-game code works.</para>
        /// </summary>
        public override void OnInitialize()
        {
            //Find the Manager gameobject in the current scene
            //The manager gameobject contains most, if not all, components in the scene.
            _Manager = GameObject.Find("Manager");

            
        }

        /// <summary>
        /// FirstGUIPass is called whenever the gui of the mod window (or custom gui) should be created.
        /// </summary>
        public override void FirstGUIPass()
        {
            //Create a new GUI button
            _ToggleBtn = new StarshipTheory.ModLib.GUI.Button("Toggle Debug Mode");

            //Attach an event handler to the Clicked event of the button
            _ToggleBtn.Clicked += _ToggleBtn_Clicked;

            //Add the button to this mod's GUI window
            //Each mod has it's own window to use (the ModWindow variable)
            ModWindow.Items.Add(_ToggleBtn);
        }

        /// <summary>
        /// Event handler for the "Toggle Debug Mode" button
        /// </summary>
        /// <param name="item"></param>
        private void _ToggleBtn_Clicked(StarshipTheory.ModLib.GUI.GUIItem item)
        {
            //Check if the manager isn't null (It might be null if the game is still loading)
            if (_Manager != null)
            {
                MouseUI UI = _Manager.GetComponent<MouseUI>();
                ManagerOptions options = _Manager.GetComponent<ManagerOptions>();

                //Make sure we have both components before proceeding. They might be null if we are still loading
                if (UI != null && options != null)
                {
                    options.enableDebug = true; //Tricks the in-game code to think that we are in the debug mode.
                    UI.toggleDebugPanel(); //Toggles the debug panel on or off.
                }
            }
        }
    }
}

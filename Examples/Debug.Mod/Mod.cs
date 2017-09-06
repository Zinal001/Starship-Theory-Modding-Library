using System;
using StarshipTheory.ModLib;
using UnityEngine;

namespace Debug.Mod
{
    public class Mod : AbstractMod
    {
        public override string ModName => "Debug Mod";

        public override Version ModVersion => new Version("1.0.0");

        private GameObject _Manager;

        private StarshipTheory.ModLib.GUI.Button _ToggleBtn;

        public override void OnInitialize()
        {
            _Manager = GameObject.Find("Manager");

            _ToggleBtn = new StarshipTheory.ModLib.GUI.Button("Toggle Debug Mode");
            _ToggleBtn.Clicked += _ToggleBtn_Clicked;
            ModWindow.Items.Add(_ToggleBtn);
        }

        private void _ToggleBtn_Clicked(StarshipTheory.ModLib.GUI.GUIItem item)
        {
            if (_Manager != null)
            {
                MouseUI UI = _Manager.GetComponent<MouseUI>();
                ManagerOptions options = _Manager.GetComponent<ManagerOptions>();
                if (UI != null && options != null)
                {
                    options.enableDebug = true;
                    UI.toggleDebugPanel();
                }
            }
        }
    }
}

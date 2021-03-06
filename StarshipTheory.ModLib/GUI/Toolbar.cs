﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarshipTheory.ModLib.GUI
{
    /// <summary>
    /// A wrapper class for the GUILayout.Toolbar Unity method
    /// <see>https://docs.unity3d.com/ScriptReference/GUILayout.Toolbar.html</see>
    /// </summary>
    public class Toolbar : GUIItem, IGroupItem
    {
        /// <summary>
        /// The index of the selected button.
        /// </summary>
        public int Selected { get; set; }

        /// <summary>
        /// A list of strings to show on the buttons.
        /// </summary>
        public List<String> Buttons { get; private set; }

        /// <summary>
        /// Called whenever the selected button changes.
        /// </summary>
        public event StateChangedDelegate SelectedChanged;

        /// <summary>
        /// Creates a new Toolbar.
        /// </summary>
        /// <param name="selected">The index of the selected button.</param>
        /// <param name="buttons">A list of strings to show on the buttons.</param>
        public Toolbar(int selected = 0, IEnumerable<String> buttons = null)
        {
            this.Selected = selected;

            if (buttons == null)
                this.Buttons = new List<string>();
            else
                this.Buttons = buttons.ToList();
        }

        public override void Draw()
        {
            if (this.Style == null)
                this.Style = UnityEngine.GUI.skin.button;

            if (this.Visible)
            {
                if (Selected > this.Buttons.Count - 1)
                    Selected = this.Buttons.Count - 1;
                else if (Selected < 0)
                    Selected = 0;

                int S = UnityEngine.GUILayout.Toolbar(Selected, Buttons.ToArray(), Style, Options);
                if (S != Selected)
                    SelectedChanged?.Invoke(this);
                Selected = S;
            }
        }
    }
}

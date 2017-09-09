using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarshipTheory.ModLib.GUI
{
    /// <summary>
    /// A wrapper class for the GUILayout.SelectionGrid Unity method
    /// <see>https://docs.unity3d.com/ScriptReference/GUILayout.SelectionGrid.html</see>
    /// </summary>
    public class SelectionGrid : Toolbar
    {
        /// <summary>
        /// How many elements to fit in the horizontal direction. The elements will be scaled to fit unless the style defines a fixedWidth to use. The height of the control will be determined from the number of elements.
        /// </summary>
        public int NumRows { get; set; }

        /// <summary>
        /// Called whenever the selected button changes.
        /// </summary>
        public new event StateChangedDelegate SelectedChanged;

        /// <summary>
        /// Creates a new SelectionGrid.
        /// </summary>
        /// <param name="selected">The index of the selected button.</param>
        /// <param name="buttons">A list of strings to show on the buttons.</param>
        /// <param name="numRows">How many elements to fit in the horizontal direction.</param>
        public SelectionGrid(int selected = 0, IEnumerable<string> buttons = null, int numRows = 1) : base(selected, buttons)
        {
            this.NumRows = numRows;
        }

        public override void Draw()
        {
            if (this.Style == null)
                this.Style = UnityEngine.GUI.skin.button;

            if (this.Visible)
            {
                int S = UnityEngine.GUILayout.SelectionGrid(Selected, Buttons.ToArray(), NumRows, Style, Options);
                if (S != Selected)
                    SelectedChanged?.Invoke(this);
                Selected = S;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarshipTheory.ModLib.GUI
{
    /// <summary>
    /// A wrapper class for the GUILayout.TextArea Unity method
    /// <see>https://docs.unity3d.com/ScriptReference/GUILayout.TextArea.html</see>
    /// </summary>
    public class TextArea : TextField
    {
        /// <summary>
        /// Called whenever the text changes.
        /// </summary>
        public new event TextFieldEventDelegate TextChanged;

        internal override void Draw()
        {
            if (this.Visible)
            {
                String newText = UnityEngine.GUILayout.TextArea(Text, Options);
                if (newText != Text)
                    TextChanged?.Invoke(this);
                Text = newText;
            }
        }
    }
}

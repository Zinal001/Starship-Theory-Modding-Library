using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarshipTheory.ModLib.GUI
{
    /// <summary>
    /// A wrapper class for the GUILayout.TextField Unity method
    /// <see>https://docs.unity3d.com/ScriptReference/GUILayout.TextField.html</see>
    /// </summary>
    public class TextField : GUIItem
    {
        /// <summary>
        /// Text to edit.
        /// </summary>
        public String Text { get; set; }

        /// <summary>
        /// Called whenever the text changes.
        /// </summary>
        public event TextFieldEventDelegate TextChanged;

        /// <summary>
        /// Creates a new TextField.
        /// </summary>
        /// <param name="text">Text to edit.</param>
        public TextField(String text = "")
        {
            this.Text = text;
        }

        internal override void Draw()
        {
            if (this.Style == null)
                this.Style = UnityEngine.GUI.skin.textField;

            if (this.Visible)
            {
                String newText = UnityEngine.GUILayout.TextField(Text, Style, Options);
                if (newText != Text)
                    TextChanged?.Invoke(this);
                Text = newText;
            }
        }
    }

    public delegate void TextFieldEventDelegate(GUIItem item);
}

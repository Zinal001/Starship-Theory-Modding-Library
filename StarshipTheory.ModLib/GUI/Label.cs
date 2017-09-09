using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarshipTheory.ModLib.GUI
{
    /// <summary>
    /// A wrapper class for the GUILayout.Label Unity method
    /// <see>https://docs.unity3d.com/ScriptReference/GUILayout.Label.html</see>
    /// </summary>
    public class Label : GUIItem
    {
        /// <summary>
        /// Text to display on the label.
        /// </summary>
        public String Text { get; set; }

        /// <summary>
        /// Texture to display on the label.
        /// </summary>
        public UnityEngine.Texture Image { get; set; }

        /// <summary>
        /// Creates a new Label.
        /// </summary>
        /// <param name="text">Text to display on the label.</param>
        /// <param name="image">Texture to display on the label.</param>
        public Label(String text = "", UnityEngine.Texture image = null)
        {
            this.Text = text;
            this.Image = image;
        }

        /// <summary>
        /// Creates a new Label.
        /// </summary>
        /// <param name="text">Text to display on the label.</param>
        public Label(String text) : this(text, null) { }

        /// <summary>
        /// Creates a new Label.
        /// </summary>
        /// <param name="image">Texture to display on the label.</param>
        public Label(UnityEngine.Texture image) : this("", image) { }

        public override void Draw()
        {
            if (this.Style == null)
                this.Style = UnityEngine.GUI.skin.label;

            if (this.Visible)
                UnityEngine.GUILayout.Label(new UnityEngine.GUIContent(Text, Image, Tooltip), Style, Options);
        }
    }
}

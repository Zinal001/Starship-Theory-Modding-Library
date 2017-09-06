using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarshipTheory.ModLib.GUI
{
    /// <summary>
    /// A wrapper class for the GUILayout.Box Unity method
    /// <see>https://docs.unity3d.com/ScriptReference/GUILayout.Box.html</see>
    /// </summary>
    public class Box : GUIItem
    {
        /// <summary>
        /// Texture to display on the box.
        /// </summary>
        public UnityEngine.Texture Image { get; set; }

        /// <summary>
        /// Text to display on the box.
        /// </summary>
        public String Text { get; set; }

        /// <summary>
        /// Creates a new Box
        /// </summary>
        /// <param name="image">Texture to display on the box.</param>
        /// <param name="text">Text to display on the box.</param>
        public Box(UnityEngine.Texture image, String text)
        {
            this.Image = image;
            this.Text = text;
        }

        /// <summary>
        /// Createa a new Box
        /// </summary>
        /// <param name="text">Text to display on the box.</param>
        public Box(String text) : this(null, text) { }

        /// <summary>
        /// Createa a new Box
        /// </summary>
        /// <param name="image">Texture to display on the box.</param>
        public Box(UnityEngine.Texture image) : this(image, "") { }

        internal override void Draw()
        {
            if (this.Style == null)
                this.Style = UnityEngine.GUI.skin.box;
            if (this.Visible)
            {
                UnityEngine.GUILayout.Box(new UnityEngine.GUIContent(this.Text, this.Image, this.Tooltip), Style, this.Options);
            }
        }
    }
}

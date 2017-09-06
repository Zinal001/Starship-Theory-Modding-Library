using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarshipTheory.ModLib.GUI
{
    /// <summary>
    /// A wrapper class for the GUILayout.RepeatButton Unity method
    /// <see>https://docs.unity3d.com/ScriptReference/GUILayout.RepeatButton.html</see>
    /// </summary>
    public class RepeatButton : Button
    {
        /// <summary>
        /// Called when this button is clicked.
        /// </summary>
        public new event ButtonEventDelegate Clicked;

        /// <summary>
        /// Creates a new RepeatButton.
        /// </summary>
        /// <param name="text">Text to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        public RepeatButton(String text = "", UnityEngine.Texture image = null) : base(text, image)
        {
            this.Text = text;
            this.Image = image;
        }

        /// <summary>
        /// Creates a new RepeatButton.
        /// </summary>
        /// <param name="text">Text to display on the button.</param>
        public RepeatButton(String text = "") : this(text, null) { }

        /// <summary>
        /// Creates a new RepeatButton.
        /// </summary>
        /// <param name="image">Texture to display on the button.</param>
        public RepeatButton(UnityEngine.Texture image) : this("", image) { }

        internal override void Draw()
        {
            if (this.Style == null)
                this.Style = UnityEngine.GUI.skin.button;

            if (this.Visible)
            {
                if (UnityEngine.GUILayout.RepeatButton(new UnityEngine.GUIContent(Text, Image, Tooltip), Style, Options))
                    Clicked?.Invoke(this);
            }
        }
    }
}

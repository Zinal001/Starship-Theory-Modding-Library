using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarshipTheory.ModLib.GUI
{
    /// <summary>
    /// A wrapper class for the GUILayout.Toggle Unity method
    /// <see>https://docs.unity3d.com/ScriptReference/GUILayout.Toggle.html</see>
    /// </summary>
    public class Toggle : GUIItem
    {
        /// <summary>
        /// Texture to display on the button.
        /// </summary>
        public UnityEngine.Texture Image { get; set; }

        /// <summary>
        /// Is the button on or off?
        /// </summary>
        public bool IsChecked { get; set; }

        /// <summary>
        /// Text to display on the button.
        /// </summary>
        public String Text { get; set; }

        /// <summary>
        /// Called whenever IsChecked changes
        /// </summary>
        public event StateChangedDelegate CheckedChanged;

        /// <summary>
        /// Creates a new Toggle.
        /// </summary>
        /// <param name="text">Text to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="isChecked">Is the button on or off?</param>
        public Toggle(String text = "", UnityEngine.Texture image = null, bool isChecked = false)
        {
            this.Text = text;
            this.Image = image;
            this.IsChecked = isChecked;
        }

        /// <summary>
        /// Creates a new Toggle.
        /// </summary>
        /// <param name="text">Text to display on the button.</param>
        /// <param name="isChecked">Is the button on or off?</param>
        public Toggle(String text, bool isChecked = false) : this(text, null, isChecked) { }
        
        /// <summary>
        /// Creates a new Toggle.
        /// </summary>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="isChecked">Is the button on or off?</param>
        public Toggle(UnityEngine.Texture image, bool isChecked = false) : this("", image, isChecked) { }

        internal override void Draw()
        {
            if (this.Style == null)
                this.Style = UnityEngine.GUI.skin.button;

            if (this.Visible)
            {
                bool c = UnityEngine.GUILayout.Toggle(IsChecked, new UnityEngine.GUIContent(Text, Image, Tooltip), Style, Options);
                if (c != IsChecked)
                    CheckedChanged?.Invoke(this);
                IsChecked = c;
            }
        }
    }

    public delegate void StateChangedDelegate(GUIItem item);
}

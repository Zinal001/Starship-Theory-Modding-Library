using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarshipTheory.ModLib.GUI
{
    /// <summary>
    /// A wrapper class for the GUILayout.Button Unity method
    /// <see>https://docs.unity3d.com/ScriptReference/GUILayout.Button.html</see>
    /// </summary>
    public class Button : GUIItem
    {
        /// <summary>
        /// Texture to display on the button.
        /// </summary>
        public UnityEngine.Texture Image { get; set; }

        /// <summary>
        /// Text to display on the button.
        /// </summary>
        public String Text { get; set; }

        /// <summary>
        /// <para>Can this button be clicked?</para>
        /// <para>Defaults to true</para>
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// The style that the button should use when disabled.
        /// </summary>
        public UnityEngine.GUIStyle DisabledStyle { get; set; }

        /// <summary>
        /// Called when this button is clicked.
        /// </summary>
        public event ButtonEventDelegate Clicked;



        /// <summary>
        /// Creates a new Button
        /// </summary>
        /// <param name="text">Text to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        public Button(String text = "", UnityEngine.Texture image = null)
        {
            this.Text = text;
            this.Image = image;
        }

        /// <summary>
        /// Creates a new Button
        /// </summary>
        /// <param name="text">Text to display on the button.</param>
        public Button(String text = "") : this(text, null) { }

        /// <summary>
        /// Creates a new Button
        /// </summary>
        /// <param name="image">Texture to display on the button.</param>
        public Button(UnityEngine.Texture image) : this("", image) { }

        public override void Draw()
        {
            if (this.Style == null)
                this.Style = UnityEngine.GUI.skin.button;

            if(this.DisabledStyle == null)
            {
                this.DisabledStyle = this.Style;
                this.DisabledStyle.normal.textColor = UnityEngine.Color.grey;
                this.DisabledStyle.onNormal.textColor = UnityEngine.Color.grey;
            }

            if (this.Visible)
            {
                if (UnityEngine.GUILayout.Button(new UnityEngine.GUIContent(Text, Image, this.Tooltip), this.IsEnabled ? Style : DisabledStyle, this.Options))
                {
                    if(this.IsEnabled)
                        Clicked?.Invoke(this);
                }
            }
        }
    }

    public delegate void ButtonEventDelegate(GUIItem item);
}

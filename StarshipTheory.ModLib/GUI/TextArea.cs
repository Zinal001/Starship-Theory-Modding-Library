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

        /// <summary>
        /// Does this TextArea support rich text tags?
        /// <see>https://docs.unity3d.com/Manual/StyledText.html</see>
        /// </summary>
        public bool IsRichText
        {
            get
            {
                return _SetRichText.HasValue ? _SetRichText.Value : Style.richText;
            }
            set
            {
                _SetRichText = value;
            }
        }

        private bool? _SetRichText = null;


        public TextArea(String text = "") : base(text)
        {

        }

        public override void Draw()
        {
            if (this.Style == null)
                this.Style = new UnityEngine.GUIStyle(UnityEngine.GUI.skin.textArea);

            if (_SetRichText.HasValue)
            {
                this.Style.richText = _SetRichText.Value;
                _SetRichText = null;
            }

            if (this.Visible)
            {
                String newText = UnityEngine.GUILayout.TextArea(Text, Style, Options);

                if (IsEditable && newText != Text)
                    TextChanged?.Invoke(this);

                if(IsEditable)
                    Text = newText;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarshipTheory.ModLib.GUI
{
    /// <summary>
    /// A wrapper class for the GUILayout.HorizontalScrollbar and the GUILayout.VerticalScrollbar Unity methods
    /// <see>https://docs.unity3d.com/ScriptReference/GUILayout.HorizontalScrollbar.html</see>
    /// </summary>
    public class Scrollbar : Slider
    {
        /// <summary>
        /// Called whenever the value of this scrollbar changes.
        /// </summary>
        public new event ValueChangedDelegate ValueChanged;

        /// <summary>
        /// How much can we see?
        /// </summary>
        public float Size { get; set; }

        /// <summary>
        /// Creates a new Scrollbar.
        /// </summary>
        /// <param name="Direction">The direction of this scrollbar.</param>
        /// <param name="value">The position between min and max.</param>
        /// <param name="minimum">The value at the left/top end of the slider.</param>
        /// <param name="maximum">The value at the right/bottom end of the slider.</param>
        public Scrollbar(Direction Direction = Direction.Horizontal, float Value = 0, float Minimum = 0, float Maximum = 100, float Size = 20) : base(Direction, Value, Minimum, Maximum)
        {
            this.Size = Size;
        }

        public override void Draw()
        {
            if (this.Style == null)
                this.Style = SliderDirection == Direction.Horizontal ? UnityEngine.GUI.skin.horizontalScrollbar : UnityEngine.GUI.skin.verticalScrollbar;

            if (this.Visible)
            {
                float value = 0;

                if (SliderDirection == Direction.Horizontal)
                    value = UnityEngine.GUILayout.HorizontalScrollbar(Value, Size, Minumum, Maximum, Style, Options);
                else
                    value = UnityEngine.GUILayout.VerticalScrollbar(Value, Size, Minumum, Maximum, Style, Options);

                if (value != Value)
                    ValueChanged?.Invoke(this);
                Value = value;
            }
        }
    }
}

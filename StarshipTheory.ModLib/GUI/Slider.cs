using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarshipTheory.ModLib.GUI
{
    /// <summary>
    /// A wrapper class for the GUILayout.HorizontalSlider and the GUILayout.VerticalSlider Unity methods
    /// <see>https://docs.unity3d.com/ScriptReference/GUILayout.HorizontalSlider.html</see>
    /// </summary>
    public class Slider : GUIItem
    {
        /// <summary>
        /// The value the slider shows. This determines the position of the draggable thumb.
        /// </summary>
        public float Value { get; set; }

        /// <summary>
        /// The value at the right end of the slider.
        /// </summary>
        public float Maximum { get; set; }

        /// <summary>
        /// The value at the left end of the slider.
        /// </summary>
        public float Minumum { get; set; }

        /// <summary>
        /// The direction of this slider.
        /// </summary>
        public Direction SliderDirection { get; set; }

        /// <summary>
        /// Called whenever the value of this slider changes.
        /// </summary>
        public event ValueChangedDelegate ValueChanged;

        /// <summary>
        /// Creates a new Slider.
        /// </summary>
        /// <param name="Direction">The direction of this slider.</param>
        /// <param name="value">The value the slider shows. This determines the position of the draggable thumb.</param>
        /// <param name="minimum">The value at the left end of the slider.</param>
        /// <param name="maximum">The value at the right end of the slider.</param>
        public Slider(Direction direction = Direction.Horizontal, float value = 0, float minimum = 0, float maximum = 100)
        {
            this.SliderDirection = direction;
            this.Value = value;
            this.Minumum = minimum;
            this.Maximum = maximum;
        }

        internal override void Draw()
        {
            if (this.Style == null)
                this.Style = (SliderDirection == Direction.Horizontal ? UnityEngine.GUI.skin.horizontalSlider : UnityEngine.GUI.skin.verticalSlider);

            if (this.Visible)
            {
                float value = 0;

                if(SliderDirection == Direction.Horizontal)
                    value = UnityEngine.GUILayout.HorizontalSlider(Value, Minumum, Maximum, Style, UnityEngine.GUI.skin.horizontalSliderThumb, Options);
                else
                    value = UnityEngine.GUILayout.VerticalSlider(Value, Minumum, Maximum, Style, UnityEngine.GUI.skin.verticalSliderThumb, Options);

                if (value != Value)
                    ValueChanged?.Invoke(this);
                Value = value;
            }
        }
    }

    public delegate void ValueChangedDelegate(GUIItem item);
}

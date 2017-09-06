using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarshipTheory.ModLib.GUI
{
    /// <summary>
    /// The base class for all GUI items within the mod library
    /// </summary>
    public abstract class GUIItem
    {
        private static readonly UnityEngine.GUILayoutOption[] EmptyOptions = new UnityEngine.GUILayoutOption[0];

        private bool _Visible = true;

        /// <summary>
        /// Determines if this element should be drawn to the screen or not
        /// </summary>
        public bool Visible
        {
            get { return _Visible; }
            set
            {
                _Visible = value;
                VisibilityChanged?.Invoke(this);
            }
        }

        /// <summary>
        /// Gets or sets an arbitrary object value that can be used to store custom information about this element.
        /// </summary>
        public Object Tag { get; set; }

        /// <summary>
        /// <para>Gets or sets the tooltip text for the element.</para>
        /// <para>Note: This field is not used by all elements.</para>
        /// </summary>
        public String Tooltip { get; set; }

        /// <summary>
        /// An optional list of layout options that specify extra layouting properties.
        /// </summary>
        public UnityEngine.GUILayoutOption[] Options { get; set; }

        public GUIItem()
        {
            Options = EmptyOptions;
        }

        /// <summary>
        /// Called whenever the visiblity changes on this element.
        /// </summary>
        public event StateChangedDelegate VisibilityChanged;

        /// <summary>
        /// The method that actually drawn the element on the screen.
        /// </summary>
        internal abstract void Draw();

        public enum Direction
        {
            Horizontal = 1,
            Vertical = 2
        }
    }
}

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
        /// <para>Gets or sets the style of this element.</para>
        /// <para>Note: The element will use it's default style if this value is null</para>
        /// </summary>
        public virtual UnityEngine.GUIStyle Style { get; set; }

        /// <summary>
        /// An optional list of layout options that specify extra layouting properties.
        /// </summary>
        protected UnityEngine.GUILayoutOption[] Options { get; set; }

        public float? MinWidth { get; set; }
        public float? MaxWidth { get; set; }
        public float? MinHeight { get; set; }
        public float? MaxHeight { get; set; }

        protected UnityEngine.Rect? _ItemRect { get; private set; }

        public bool MouseHovering
        {
            get
            {
                return _ItemRect.HasValue && _ItemRect.Value.Contains(UnityEngine.Event.current.mousePosition);
            }
        }

        public GUIItem()
        {
            Options = EmptyOptions;
        }

        /// <summary>
        /// Called whenever the visiblity changes on this element.
        /// </summary>
        public event StateChangedDelegate VisibilityChanged;

        public event GUIMouseEvent MouseDown;
        public event GUIMouseEvent MouseUp;

        protected static int GroupDepth = 0;


        internal AbstractMod _drawingMod = null;

        internal virtual void __Draw()
        {
            List<UnityEngine.GUILayoutOption> _Options = new List<UnityEngine.GUILayoutOption>();

            if (MinWidth.HasValue)
                _Options.Add(UnityEngine.GUILayout.MinWidth(MinWidth.Value));

            if (MaxWidth.HasValue)
                _Options.Add(UnityEngine.GUILayout.MaxWidth(MaxWidth.Value));

            if (MinHeight.HasValue)
                _Options.Add(UnityEngine.GUILayout.MinHeight(MinHeight.Value));

            if (MaxHeight.HasValue)
                _Options.Add(UnityEngine.GUILayout.MaxHeight(MaxHeight.Value));

            this.Options = _Options.ToArray();

            this.Draw();

            try
            {
                if (UnityEngine.Event.current.type == UnityEngine.EventType.Repaint && !(this is IGroupItem))
                    _ItemRect = UnityEngine.GUILayoutUtility.GetLastRect();

                if(_ItemRect.HasValue && UnityEngine.Event.current.isMouse)
                {
                    if (UnityEngine.Event.current.type == UnityEngine.EventType.MouseDown)
                    {
                        if(_ItemRect.Value.Contains(UnityEngine.Event.current.mousePosition))
                            MouseDown?.Invoke(this, UnityEngine.Event.current.button, UnityEngine.Event.current.mousePosition);
                    }
                    else if(UnityEngine.Event.current.type == UnityEngine.EventType.MouseUp)
                    {
                        if (_ItemRect.Value.Contains(UnityEngine.Event.current.mousePosition))
                            MouseUp?.Invoke(this, UnityEngine.Event.current.button, UnityEngine.Event.current.mousePosition);
                    }

                }

            }
            catch(Exception ex)
            {
                UnityEngine.Debug.Log("Unable to get rect of " + this.GetType().Name + ": " + ex.Message);
            }
        }

        /// <summary>
        /// The method that actually draws the element on the screen.
        /// </summary>
        public abstract void Draw();

        public enum Direction
        {
            Horizontal = 1,
            Vertical = 2
        }
    }

    public delegate void GUIMouseEvent(GUIItem item, int button, UnityEngine.Vector2 position);
}

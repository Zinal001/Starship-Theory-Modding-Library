using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarshipTheory.ModLib.GUI
{
    /// <summary>
    /// A wrapper class for the GUILayout.BeginScrollView Unity method
    /// <see>https://docs.unity3d.com/ScriptReference/GUILayout.BeginScrollView.html</see>
    /// </summary>
    public class ScrollView : GUIItem
    {
        /// <summary>
        /// The current position of the viewable area.
        /// </summary>
        public UnityEngine.Vector2 ScrollPosition { get; set; }

        /// <summary>
        /// Should the horizontal scrollbar always be visible? If false or left out, it is only shown when the content inside the ScrollView is wider than the scrollview itself.
        /// </summary>
        public bool AlwaysShowHorizontal { get; set; }

        /// <summary>
        /// Should the vertical scrollbar always be visible? If false or left out, it is only shown when the content inside the ScrollView is taller than the scrollview itself.
        /// </summary>
        public bool AlwaysShowVertical { get; set; }

        /// <summary>
        /// The children that should be drawn within this ScrollView.
        /// </summary>
        public List<GUIItem> Items { get; private set; }

        /// <summary>
        /// Called whenever the viewable area changes.
        /// </summary>
        public event ValueChangedDelegate ScrollChanged;

        /// <summary>
        /// Creates a new ScrollView.
        /// </summary>
        /// <param name="horizontalBar">Should the horizontal scrollbar always be visible?</param>
        /// <param name="verticalBar">Should the vertical scrollbar always be visible?</param>
        /// <param name="items">The children that should be drawn within this ScrollView.</param>
        public ScrollView(bool horizontalBar = false, bool verticalBar = false, IEnumerable<GUIItem> items = null) : this(UnityEngine.Vector2.zero, horizontalBar, verticalBar, items)
        {

        }

        /// <summary>
        /// Creates a new ScrollView.
        /// </summary>
        /// <param name="scrollPosition">The current position of the viewable area.</param>
        /// <param name="horizontalBar">Should the horizontal scrollbar always be visible?</param>
        /// <param name="verticalBar">Should the vertical scrollbar always be visible?</param>
        /// <param name="items">The children that should be drawn within this ScrollView.</param>
        public ScrollView(UnityEngine.Vector2 scrollPosition, bool horizontalBar = false, bool verticalBar = false, IEnumerable<GUIItem> items = null)
        {
            this.ScrollPosition = scrollPosition;
            this.AlwaysShowHorizontal = horizontalBar;
            this.AlwaysShowVertical = verticalBar;

            if (items == null)
                this.Items = new List<GUIItem>();
            else
                this.Items = items.ToList();
        }

        internal override void Draw()
        {
            if (this.Style == null)
                this.Style = UnityEngine.GUI.skin.scrollView;

            if (this.Visible)
            {
                UnityEngine.Vector2 pos = UnityEngine.GUILayout.BeginScrollView(ScrollPosition, AlwaysShowHorizontal, AlwaysShowVertical, UnityEngine.GUI.skin.horizontalScrollbar, UnityEngine.GUI.skin.verticalScrollbar, Style, Options);
                if (pos != ScrollPosition)
                    ScrollChanged?.Invoke(this);
                ScrollPosition = pos;

                foreach(GUIItem item in Items)
                {
                    if (item.Visible)
                        item.Draw();
                }

                UnityEngine.GUILayout.EndScrollView();
            }
        }
    }
}

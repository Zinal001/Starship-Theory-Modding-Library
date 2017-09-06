using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StarshipTheory.ModLib.GUI
{
    /// <summary>
    /// A wrapper class for the GUILayout.Window Unity method
    /// <see>https://docs.unity3d.com/ScriptReference/GUILayout.Window.html</see>
    /// </summary>
    public class Window : GUIItem
    {
        /// <summary>
        /// A unique Index to use for each window. This is the Index you'll use to interface to it.
        /// <see cref="StarshipTheory.ModLib.ModGUI.GetWindowIndex"></see>
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// Text to display as a title for the window.
        /// </summary>
        public String Title { get; set; }

        /// <summary>
        /// Texture to display an image in the titlebar.
        /// </summary>
        public Texture Image { get; set; }

        /// <summary>
        /// The children to display within this window.
        /// </summary>
        public List<GUIItem> Items { get; private set; }

        /// <summary>
        /// Rectangle on the screen to use for the window. The layouting system will attempt to fit the window inside it - if that cannot be done, it will adjust the rectangle to fit.
        /// </summary>
        public Rect Rect { get; set; } = new Rect(0, 0, 200, 20);

        /// <summary>
        /// Is this Window draggable?
        /// </summary>
        public bool IsDraggable { get; set; }

        /// <summary>
        /// Is the Window resizeable?
        /// </summary>
        public bool IsResizeable { get; set; }

        private bool _IsResizing = false;

        /// <summary>
        /// Creates a new Window
        /// </summary>
        /// <param name="windowIndex">A unique Index to use for each window.</param>
        /// <param name="title">Text to display as a title for the window.</param>
        /// <param name="image">Texture to display an image in the titlebar.</param>
        public Window(int windowIndex, String title = "", Texture image = null)
        {
            Index = windowIndex;
            Items = new List<GUIItem>();
            this.Title = title;
            this.Image = image;
        }

        /// <summary>
        /// Creates a new Window
        /// </summary>
        /// <param name="windowIndex">A unique Index to use for each window.</param>
        /// <param name="title">Text to display as a title for the window.</param>
        public Window(int windowIndex, String title) : this(windowIndex, title, null) { }
        
        /// <summary>
        /// Creates a new Window
        /// </summary>
        /// <param name="windowIndex">A unique Index to use for each window.</param>
        /// <param name="image">Texture to display an image in the titlebar.</param>
        public Window(int windowIndex, Texture image) : this(windowIndex, "", image) { }

        internal override void Draw()
        {
            if (this.Style == null)
                this.Style = UnityEngine.GUI.skin.window;

            if (this.Visible)
            {
                Rect = GUILayout.Window(Index, Rect, new UnityEngine.GUI.WindowFunction(_OnDraw), new GUIContent(Title, Image, Tooltip), Style, Options);
            }
        }

        private void _OnDraw(int windowIndex)
        {
            if(this.Visible)
            {
                if (IsDraggable)
                    UnityEngine.GUI.DragWindow(new Rect(0, 0, Rect.width, 20));

                foreach (GUIItem Item in Items)
                {
                    if (Item.Visible)
                        Item.Draw();
                }

                if(IsResizeable)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Box("", GUILayout.Width(10), GUILayout.Height(10));
                    GUILayout.EndHorizontal();

                    if (Event.current.type == EventType.MouseUp)
                        _IsResizing = false;
                    else if (Event.current.type == EventType.MouseDown && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                        _IsResizing = true;

                    if (_IsResizing)
                        Rect = new Rect(Rect.x, Rect.y, Rect.width + Event.current.delta.x, Rect.height + Event.current.delta.y);
                }

            }
        }
    }
}

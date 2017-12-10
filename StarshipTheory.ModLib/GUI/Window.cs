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
    public class Window : GUIItem, IGroupItem
    {
        /* TODO: Fix Window Resizing
         * 
         * 
         */

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

        private Rect? OverrideRect = null;

        /// <summary>
        /// Is this Window draggable?
        /// </summary>
        public bool IsDraggable { get; set; }

        /// <summary>
        /// Is the Window resizeable?
        /// </summary>
        public bool IsResizeable { get; set; }

        private ResizeHandle _ResizeHandle;

        private UnityEngine.GUI.WindowFunction _OnDrawFunc = null;


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

        public void Resize(float width, float height)
        {
            float minW = MinWidth ?? 0;
            float maxW = MaxWidth ?? Screen.width;
            float minH = MinHeight ?? 0;
            float maxH = MaxHeight ?? Screen.height;

            if (width < minW)
                width = minW;
            if (width > maxW)
                width = maxW;
            if (height < minH)
                height = minH;
            if (height > maxH)
                height = maxH;

            this.OverrideRect = new Rect(Rect.x, Rect.y, width, height);
        }

        internal override void __Draw()
        {
            List<GUILayoutOption> _Options = new List<GUILayoutOption>();

            if (MinWidth.HasValue)
                _Options.Add(GUILayout.MinWidth(MinWidth.Value));

            if (MaxWidth.HasValue)
                _Options.Add(GUILayout.MaxWidth(MaxWidth.Value));

            if (MinHeight.HasValue)
                _Options.Add(GUILayout.MinHeight(MinHeight.Value));

            if (MaxHeight.HasValue)
                _Options.Add(GUILayout.MaxHeight(MaxHeight.Value));

            this.Options = _Options.ToArray();

            this.Draw();
        }

        public override void Draw()
        {
            if (this.Style == null)
                this.Style = UnityEngine.GUI.skin.window;

            if (this.Visible)
            {
                if (_OnDrawFunc == null)
                    _OnDrawFunc = new UnityEngine.GUI.WindowFunction(_OnDraw);

                if (OverrideRect.HasValue)
                {
                    Rect = new Rect(OverrideRect.Value.position, OverrideRect.Value.size);
                    Debug.Log("Overriding window with " + OverrideRect.Value.width + ", " + OverrideRect.Value.height);
                    OverrideRect = null;
                }

                Rect = GUILayout.Window(Index, Rect, _OnDrawFunc, new GUIContent(Title, Image, Tooltip), Style, Options);
            }
        }

        private void CloseBtn_Clicked(GUIItem item)
        {
            this.Visible = false;
        }

        private void _OnDraw(int windowIndex)
        {
            try
            {
                if (this.Visible)
                {
                    if (IsDraggable)
                        UnityEngine.GUI.DragWindow(new Rect(0, 0, Rect.width, 20));

                    foreach (GUIItem Item in Items)
                    {
                        if (Item.Visible)
                            Item.Draw();
                    }

                    if (IsResizeable)
                    {
                        if (_ResizeHandle == null)
                            _ResizeHandle = new ResizeHandle(this);

                        _ResizeHandle.Draw();
                    }

                }
            }
            catch(Exception ex)
            {
                ModLoader.Instance.ShowError(_drawingMod, ex, "OnGUI");
            }
        }
    }
}

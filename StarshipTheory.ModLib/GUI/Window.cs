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

        private Vector2? _ResizeStart = null;
        private bool _IsResizing = false;

        private Area _WindowArea;
        private Group TitleGroup;
        private Label _TitleLabel;
        private ScrollView _BodyView;

        private Space leftSpacer;
        private Space rightSpacer;
        private Space bottomSpacer;


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

        internal override void __Draw()
        {
            List<GUILayoutOption> _Options = new List<GUILayoutOption>();

            if(this.IsResizeable)
            {
                if (MinWidth.HasValue)
                    _Options.Add(GUILayout.MinWidth(MinWidth.Value));

                if (MaxWidth.HasValue)
                    _Options.Add(GUILayout.MaxWidth(MaxWidth.Value));

                if (MinHeight.HasValue)
                    _Options.Add(GUILayout.MinHeight(MinHeight.Value));

                if (MaxHeight.HasValue)
                    _Options.Add(GUILayout.MaxHeight(MaxHeight.Value));
            }

            this.Options = _Options.ToArray();

            this.Draw();
        }

        public override void Draw()
        {
            if (this.Style == null)
                this.Style = UnityEngine.GUI.skin.window;

            if (this.Visible)
            {
                Rect = GUILayout.Window(Index, Rect, new UnityEngine.GUI.WindowFunction(_OnDraw), new GUIContent(Title, Image, Tooltip), Style, Options);
            }
        }

        private void __CreateWindowComponents()
        {
            GUIStyle titleStyle = new GUIStyle(UnityEngine.GUI.skin.label)
            {
                fontStyle = FontStyle.Bold,
                wordWrap = false
            };

            Area windowArea = new Area(Rect);

            Group titleGroup = new Group(Direction.Horizontal);
            Label titleLable = new Label(Title) { Style = titleStyle, MinHeight = 20, MaxHeight = 20 };
            titleGroup.Items.Add(titleLable);
            titleGroup.Items.Add(new FlexibleSpace());
            Button closeBtn = new Button("X");
            closeBtn.Clicked += CloseBtn_Clicked;
            titleGroup.Items.Add(closeBtn);

            windowArea.Items.Add(titleGroup);
            TitleGroup = titleGroup;

            Group bodyGroup = new Group(Direction.Horizontal);
            leftSpacer = new Space(5);
            leftSpacer.MouseDown += Resize_MouseDown;
            bodyGroup.Items.Add(leftSpacer);
            ScrollView bodyView = new ScrollView();
            foreach (GUIItem item in Items)
                bodyView.Items.Add(item);
            bodyGroup.Items.Add(bodyView);
            rightSpacer = new Space(5);
            rightSpacer.MouseDown += Resize_MouseDown;
            bodyGroup.Items.Add(rightSpacer);

            windowArea.Items.Add(bodyGroup);

            Group footerGroup = new Group(Direction.Horizontal);
            bottomSpacer = new Space(5);
            bottomSpacer.MouseDown += Resize_MouseDown;
            footerGroup.Items.Add(bottomSpacer);

            windowArea.Items.Add(footerGroup);

            _WindowArea = windowArea;
            _TitleLabel = titleLable;
            _BodyView = bodyView;
        }

        private void Resize_MouseDown(GUIItem item, int button, Vector2 position)
        {
            if(button == 0)
            {
                Debug.Log("MOUSE DOWN: " + item.GetType().Name);
            }
        }

        private void CloseBtn_Clicked(GUIItem item)
        {
            this.Visible = false;
        }

        private void __OnWindow()
        {
            if (_WindowArea == null)
                __CreateWindowComponents();

            _TitleLabel.Text = Title;

            if (this.Visible)
            {
                _BodyView.Items.Clear();
                foreach (GUIItem item in Items)
                    _BodyView.Items.Add(item);

                leftSpacer.Visible = bottomSpacer.Visible = rightSpacer.Visible = IsResizeable;

                GroupDepth++;
                _WindowArea.__Draw();
                GroupDepth--;
            }
        }

        private void _OnDraw(int windowIndex)
        {
            try
            {
                if (this.Visible)
                {
                    if (IsDraggable)
                        UnityEngine.GUI.DragWindow(new Rect(0, 0, Rect.width, 20));

                    if (Event.current.type == EventType.MouseUp || Input.GetMouseButtonUp(0))
                        _ResizeStart = null;

                    foreach (GUIItem Item in Items)
                    {
                        if (Item.Visible)
                            Item.Draw();
                    }

                    if (IsResizeable)
                    {
                        GUILayout.Box("", GUILayout.Height(5));
                        
                        if (Event.current.type == EventType.MouseDown)
                        {
                            Rect r = GUILayoutUtility.GetLastRect();
                            if(r.Contains(Event.current.mousePosition))
                            {
                                _ResizeStart = new Vector2(Event.current.mousePosition.x, Event.current.mousePosition.y);
                            }
                        }

                        if (_ResizeStart.HasValue)
                        {
                            float w = Event.current.mousePosition.x - Rect.x;
                            if (w < 0)
                                w = 0;

                            float h = Event.current.mousePosition.y - Rect.y;
                            if (h < 0)
                                h = 0;

                            Rect = new Rect(Rect.x, Rect.y, w, h);
                        }
                    }

                }
            }
            catch(Exception ex)
            {
                ModLoader.Instance.ShowError(_drawingMod, ex, "OnGUI");
            }
        }

        bool draggingLeft = false;
        bool draggingRight = false;

        private Rect HorizResizer(Rect window, bool right = true, float detectionRange = 8f)
        {
            detectionRange *= 0.5f;
            Rect resizer = window;

            if (right)
            {
                resizer.xMin = resizer.xMax - detectionRange;
                resizer.xMax += detectionRange;
            }
            else
            {
                resizer.xMax = resizer.xMin + detectionRange;
                resizer.xMin -= detectionRange;
            }

            Event current = Event.current;
            //EditorGUIUtility.AddCursorRect(resizer, MouseCursor.ResizeHorizontal);

            // if mouse is no longer dragging, stop tracking direction of drag
            if (current.type == EventType.MouseUp)
            {
                draggingLeft = false;
                draggingRight = false;
            }

            // resize window if mouse is being dragged within resizor bounds
            if (current.mousePosition.x > resizer.xMin &&
                current.mousePosition.x < resizer.xMax &&
                current.type == EventType.MouseDrag &&
                current.button == 0 ||
                draggingLeft ||
                draggingRight)
            {
                if (right == !draggingLeft)
                {
                    window.width = current.mousePosition.x + current.delta.x;
                    //Repaint();
                    draggingRight = true;
                }
                else if (!right == !draggingRight)
                {
                    window.width = window.width - (current.mousePosition.x + current.delta.x);
                    //Repaint();
                    draggingLeft = true;
                }

            }

            return window;
        }
    }
}

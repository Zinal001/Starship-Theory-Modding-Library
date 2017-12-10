using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StarshipTheory.ModLib.GUI
{
    public class WindowButton : Area
    {
        public Vector2 HandleSize { get; set; } = new Vector2(10, 10);

        public String ButtonTexture { get; private set; }

        protected Window _Parent { get; private set; }

        private Box _BoxHandle;

        public Vector2 PositionOffset { get; set; }

        public WindowButton(Window Parent, String ButtonTexture, Vector2 PositionOffset) : base(new Rect(0, 0, 0, 0))
        {
            this._Parent = Parent;
            this.ButtonTexture = ButtonTexture;
            this.PositionOffset = PositionOffset;

            this.Options = new GUILayoutOption[] {
                GUILayout.Width(HandleSize.x),
                GUILayout.Height(HandleSize.y)
            };

            this.Style = new GUIStyle(ModGUI.TrasparentGUIStyle)
            {
                alignment = TextAnchor.MiddleCenter,
                border = new RectOffset(0, 0, 0, 0)
            };
        }

        private Rect _DrawRect;

        public override void Draw()
        {
            this.Size = new Rect(PositionOffset.x, PositionOffset.y, HandleSize.x, HandleSize.y);

            if (_BoxHandle == null)
            {
                Texture2D tex = InternalResources.ReadResourceAsTexture2D(ButtonTexture);
                _BoxHandle = new Box(tex)
                {
                    MinWidth = HandleSize.x,
                    MaxWidth = HandleSize.x,
                    MaxHeight = HandleSize.y,
                    MinHeight = HandleSize.y,
                    Style = new GUIStyle(ModGUI.TrasparentGUIStyle)
                    {
                        border = new RectOffset(0, 0, 0, 0),
                        alignment = TextAnchor.MiddleCenter
                    }
                };
                Items.Add(_BoxHandle);
            }

            base.Draw();

            if (Event.current.type == EventType.repaint)
                _DrawRect = GUILayoutUtility.GetLastRect();

            if (Event.current.type == EventType.mouseDown && Event.current.button == 0)
            {
                Debug.Log("Click: (" + Event.current.mousePosition.x + ", " + Event.current.mousePosition.y + ") -> (" + Size.x + ", " + Size.y + ", " + Size.width + ", " + Size.height + ")");
                if(_InResizeArea(Event.current.mousePosition))
                {
                    Debug.Log("WindowButton Clicked!");
                    Trigger_MouseDown(Event.current.button, Event.current.mousePosition);
                }
            }
        }

        private bool _InResizeArea(Vector2 pos)
        {
            return this.Size.Contains(pos);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StarshipTheory.ModLib.GUI
{
    public class ResizeHandle : Area, IResizeable
    {
        private static readonly Vector2 _HandleSize = new Vector2(10, 10);

        private Window _Parent;

        private Box _ResizeBox;

        public ResizeHandle(Window Parent) : base(new Rect(Parent.Rect.x + Parent.Rect.width - _HandleSize.x, Parent.Rect.y + Parent.Rect.height - _HandleSize.y, _HandleSize.x, _HandleSize.y))
        {
            this._Parent = Parent;

            this.Options = new GUILayoutOption[] {
                GUILayout.Width(_HandleSize.x),
                GUILayout.Height(_HandleSize.y)
            };

            this.Style = new GUIStyle(ModGUI.TrasparentGUIStyle) {
                alignment = TextAnchor.MiddleCenter,
                border = new RectOffset(0, 0, 0, 0)
            };
        }

        private Rect _DrawRect;

        private Vector2 dragStart = Vector2.zero;
        private Vector2 sizeStart = Vector2.zero;

        public override void Draw()
        {
            this.Size = new Rect(_Parent.Rect.width - _HandleSize.x, _Parent.Rect.height - _HandleSize.y, _HandleSize.x, _HandleSize.y);

            if (_ResizeBox == null)
            {
                Texture2D tex = InternalResources.ReadResourceAsTexture2D("ResizeHandle");
                _ResizeBox = new Box(tex)
                {
                    MinWidth = _HandleSize.x,
                    MaxWidth = _HandleSize.x,
                    MaxHeight = _HandleSize.y,
                    MinHeight = _HandleSize.y,
                    Style = new GUIStyle(ModGUI.TrasparentGUIStyle) {
                        border = new RectOffset(0, 0, 0, 0),
                        alignment = TextAnchor.MiddleCenter
                    }
                };
                Items.Add(_ResizeBox);
            }

            base.Draw();

            if (Event.current.type == EventType.repaint)
            {
                _DrawRect = GUILayoutUtility.GetLastRect();
            }

            if (Event.current.type == EventType.mouseDown && Event.current.button == 0 && _InResizeArea(Event.current.mousePosition) /*this.Size.Contains(Event.current.mousePosition)*/)
            {
                dragStart = new Vector2(Event.current.mousePosition.x, Event.current.mousePosition.y);
                sizeStart = new Vector2(_Parent.Rect.width, _Parent.Rect.height);
                ModGUI.RegisterResize(this);
                Debug.Log("Dragging Started: " + dragStart.x + ", " + dragStart.y);
            }

            if(dragStart != Vector2.zero)
            {
                float w = Event.current.mousePosition.x - dragStart.x;
                float h = Event.current.mousePosition.y - dragStart.y;

                Debug.Log("Dragging: " + w + ", " + h);

                _Parent.Resize(sizeStart.x + w, sizeStart.y + h);
            }

            if ((Event.current.type == EventType.mouseUp && Event.current.button == 0))
            {
                Debug.Log("Resetting drag");
                ModGUI.UnregisterResize(this);

                dragStart = Vector2.zero;
                sizeStart = Vector2.zero;
            }

        }

        private bool _InResizeArea(Vector2 pos)
        {
            return this.Size.Contains(pos);
        }

        public void ResizeStart(Vector2 mousePosition)
        {

        }

        public void ResizeEnd(Vector2 mousePosition)
        {
            Debug.Log("Resetting drag from ModGUI");
            ModGUI.UnregisterResize(this);

            dragStart = Vector2.zero;
            sizeStart = Vector2.zero;
        }
    }
}

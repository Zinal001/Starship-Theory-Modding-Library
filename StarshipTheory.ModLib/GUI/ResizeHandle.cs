using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StarshipTheory.ModLib.GUI
{
    public class ResizeHandle : WindowButton, IResizeable
    {
        private bool _IsResizing = false;

        Vector2 dragStart;
        Vector2 sizeStart;

        public ResizeHandle(Window Parent) : base(Parent, "ResizeHandle", new Vector2(0, 0))
        {
            this.MouseDown += ResizeHandle2_MouseDown;
            this.MouseUp += ResizeHandle2_MouseUp;
        }

        private void ResizeHandle2_MouseUp(GUIItem item, int button, Vector2 position)
        {
            ResizeEnd(Event.current.mousePosition);
        }

        private void ResizeHandle2_MouseDown(GUIItem item, int button, Vector2 position)
        {
            _IsResizing = true;
            dragStart = new Vector2(position.x, position.y);
            sizeStart = new Vector2(_Parent.Rect.width, _Parent.Rect.height);
            ModGUI.RegisterResize(this);
        }

        public override void Draw()
        {
            this.PositionOffset = new Vector2(_Parent.Rect.width - HandleSize.x, _Parent.Rect.height - HandleSize.y);
            base.Draw();

            if(Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                ResizeEnd(Event.current.mousePosition);
            }

            if(_IsResizing)
            {
                float w = Event.current.mousePosition.x - dragStart.x;
                float h = Event.current.mousePosition.y - dragStart.y;

                _Parent.Resize(sizeStart.x + w, sizeStart.y + h);
            }

        }

        public void ResizeStart(Vector2 mousePosition)
        {

        }

        public void ResizeEnd(Vector2 mousePosition)
        {
            _IsResizing = false;
            ModGUI.UnregisterResize(this);

            dragStart = Vector2.zero;
            sizeStart = Vector2.zero;
        }
    }
}

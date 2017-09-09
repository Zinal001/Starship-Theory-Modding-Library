using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StarshipTheory.ModLib.GUI
{
    /// <summary>
    /// 
    /// AUTHOR: sdgd
    /// AUTHOR LINK: http://answers.unity3d.com/questions/17676/guiwindow-resize-window.html
    /// </summary>
    internal struct ResizeWindow
    {
        public int Grab;
        public int MinWinSize;

        private bool ResizeUp;
        private bool ResizeDown;
        private bool ResizeLeft;
        private bool ResizeRight;

        private float rememberX;
        private float rememberResetX;
        private float rememberY;
        private float rememberResetY;
        // you actually don't name anything true or false with new one just less code to type
        public ResizeWindow(int GrabLength)
        {
            ResizeUp = false;
            ResizeDown = false;
            ResizeLeft = false;
            ResizeRight = false;
            rememberX = 0;
            rememberResetX = 0;
            rememberY = 0;
            rememberResetY = 0;
            Grab = GrabLength;
            MinWinSize = 50;
        }

        public ResizeWindow(int GrabLength, int MinimumWindowSize)
        {
            ResizeUp = false;
            ResizeDown = false;
            ResizeLeft = false;
            ResizeRight = false;
            rememberX = 0;
            rememberResetX = 0;
            rememberY = 0;
            rememberResetY = 0;
            Grab = GrabLength;
            MinWinSize = MinimumWindowSize;
        }


        public void ResizeF(ref Rect ResizingWindow)
        {
            ResizeXF(ref ResizingWindow);
            ResizeYF(ref ResizingWindow);
        }





        public void ResizeXF(ref Rect ResizingWindow)
        {
            ResizeLeftF(ref ResizingWindow);
            ResizeRightF(ref ResizingWindow);
        }


        public void ResizeYF(ref Rect ResizingWindow)
        {
            ResizeUpF(ref ResizingWindow);
            ResizeDownF(ref ResizingWindow);
        }




        // WARNING /*Input.mousePosition.y * -1 + Screen.height*/ is correct positioning pixles
        public void ResizeUpF(ref Rect ResizingWindow)
        {
            if (Event.current.type.ToString() == "mouseDown" && Event.current.mousePosition.y < (Grab))
            {
                ResizeUp = true;
                rememberY = ResizingWindow.y + ResizingWindow.height;
                rememberResetY = ResizingWindow.y;
            }
            if (ResizeUp)
            {
                ResizingWindow.y = (Screen.height - Input.mousePosition.y) - 1;
                ResizingWindow.height = rememberY - (Screen.height - Input.mousePosition.y);
                if (Event.current.type.ToString() == "mouseUp")
                {
                    ResizeUp = false;
                }
                else
                {
                    FalseItY(ref ResizingWindow, 'U');
                }
            }
        }

        public void ResizeDownF(ref Rect ResizingWindow)
        {
            if (Event.current.type.ToString() == "mouseDown" && Event.current.mousePosition.y > (ResizingWindow.height - Grab))
            {
                ResizeDown = true;
                rememberY = ResizingWindow.y + ResizingWindow.height;
                rememberResetY = ResizingWindow.y;
            }
            if (ResizeDown)
            {
                ResizingWindow.height = Event.current.mousePosition.y + 1;
                if (Event.current.type.ToString() == "mouseUp")
                {
                    ResizeDown = false;
                }
                else
                {
                    FalseItY(ref ResizingWindow, 'D');
                }
            }
        }

        public void ResizeLeftF(ref Rect ResizingWindow)
        {
            if (Event.current.type.ToString() == "mouseDown" && Event.current.mousePosition.x < (Grab))
            {
                ResizeLeft = true;
                rememberX = ResizingWindow.x + ResizingWindow.width;
                rememberResetX = ResizingWindow.x;
            }
            if (ResizeLeft)
            {
                ResizingWindow.x = Input.mousePosition.x - 1;
                ResizingWindow.width = rememberX - Input.mousePosition.x;
                if (Event.current.type.ToString() == "mouseUp")
                {
                    ResizeLeft = false;
                }
                else
                {
                    FalseItX(ref ResizingWindow, 'L');
                }
            }
        }

        public void ResizeRightF(ref Rect ResizingWindow)
        {
            if (Event.current.type.ToString() == "mouseDown" && Event.current.mousePosition.x > (ResizingWindow.width - Grab))
            {
                ResizeRight = true;
                rememberX = ResizingWindow.x + ResizingWindow.width;
                rememberResetX = ResizingWindow.x;
            }
            if (ResizeRight)
            {
                ResizingWindow.width = Event.current.mousePosition.x + 1;
                if (Event.current.type.ToString() == "mouseUp")
                {
                    ResizeRight = false;
                }
                else
                {
                    FalseItX(ref ResizingWindow, 'R');
                }
            }
        }

        void FalseItY(ref Rect Window, char c)
        {
            if ((Event.current.mousePosition.x < 0 || Event.current.mousePosition.x > Window.width)
                && (ResizeLeft == false && ResizeRight == false)
                || (Window.height < MinWinSize))
            {

                ResizeUp = false;
                ResizeDown = false;

                if (Window.height < MinWinSize + 1)
                {
                    if (c == 'U')
                    {
                        Window.y -= Grab;
                        Window.height += Grab;
                    }
                    else if (c == 'D')
                    {
                        Window.height += Grab;
                    }
                    // just a glich workaround (if PLayer pulles the window too fast so window becomes turned around)
                    if (Window.height < Grab)
                    {
                        if (c == 'U')
                        {
                            Window.y = rememberResetY;
                            Window.height = rememberY - rememberResetY;
                        }
                        else if (c == 'D')
                        {
                            Window.height = rememberY - rememberResetY;
                        }
                    }
                }
            }
        }
        void FalseItX(ref Rect Window, char c)
        {
            if ((Event.current.mousePosition.y < 0 || Event.current.mousePosition.y > Window.height)
                && (ResizeUp == false && ResizeDown == false)
                || (Window.width < MinWinSize))
            {

                ResizeLeft = false;
                ResizeRight = false;

                if (Window.width < MinWinSize + 1)
                {
                    if (c == 'L')
                    {
                        Window.x -= Grab;
                        Window.width += Grab;
                    }
                    else if (c == 'R')
                    {
                        Window.width += Grab;
                    }
                    // just a glich workaround (if PLayer pulles the window too fast so window becomes turned around)
                    if (Window.width < Grab)
                    {
                        if (c == 'L')
                        {
                            Window.x = rememberResetX;
                            Window.width = rememberX - rememberResetX;
                        }
                        else if (c == 'R')
                        {
                            Window.width = rememberX - rememberResetX;
                        }
                    }
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarshipTheory.ModLib.Events
{
    public static class MenuEvents
    {
        public class MenuGameObjectEventArgs : EventArgs
        {
            public UnityEngine.GameObject Parent { get; private set; }

            public MenuGameObjectEventArgs(UnityEngine.GameObject Parent)
            {
                this.Parent = Parent;
            }
        }

        public static event EventHandler<MenuGameObjectEventArgs> NewGamePanelOpened;



        public static void __OnNewGamePanelOpened(UnityEngine.GameObject Parent)
        {
            NewGamePanelOpened?.Invoke(null, new MenuGameObjectEventArgs(Parent));
        }
    }
}

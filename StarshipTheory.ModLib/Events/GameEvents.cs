using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarshipTheory.ModLib.Events
{
    public static class GameEvents
    {
        public class SaveLoadEventArgs : System.EventArgs
        {
            public int saveSlot { get; private set; }
            public String savePath { get; private set; }

            public SaveLoadEventArgs(int saveSlot)
            {
                this.saveSlot = saveSlot;
                this.savePath = UnityEngine.GameObject.Find("Manager").GetComponent<ManagerOptions>()._Mod_GetFieldValue<String>("path") + "SaveData" + saveSlot.ToString();
            }
        }

        public static event EventHandler<SaveLoadEventArgs> GameSaved;
        public static event EventHandler<SaveLoadEventArgs> GameLoaded;
        public static event EventHandler GameStarted;


        public static void __OnGameSaved(int saveSlot)
        {
            GameSaved?.Invoke(null, new SaveLoadEventArgs(saveSlot));
        }

        public static void __OnGameLoaded(int saveSlot)
        {
            GameLoaded?.Invoke(null, new SaveLoadEventArgs(saveSlot));
        }

        public static void __OnGameStarted()
        {
            GameStarted?.Invoke(null, new EventArgs());
        }
    }
}

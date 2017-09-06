using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarshipTheory.ModLib
{
    public sealed class ModLoader
    {
        internal static ModLoader Instance { get; private set; }

        private List<AbstractMod> _Mods;

        public AbstractMod[] Mods
        {
            get { return _Mods.ToArray(); }
        }

        private bool _ShowModList = false;
        private GUI.Area _ModListButtonArea;
        private GUI.Button _ToggleModListBtn;

        public ModLoader()
        {
            Instance = this;
        }

        /// <summary>
        /// <para>This method is called from within the game's code to load all the mods</para>
        /// <para>Note: This method might be called several times due to how the game code is constructed</para>
        /// </summary>
        public void __LoadMods()
        {
            _Mods = new List<AbstractMod>();

            _ModListButtonArea = new GUI.Area(new UnityEngine.Rect(UnityEngine.Screen.width - 100, 0, 100, UnityEngine.Screen.height));
            _ToggleModListBtn = new GUI.Button("Mods");
            _ToggleModListBtn.Clicked += ToggleModListBtn_Clicked;
            _ModListButtonArea.Items.Add(_ToggleModListBtn);

            String ModsFolder = null;
            if (UnityEngine.Application.platform == UnityEngine.RuntimePlatform.WindowsPlayer)
            {
                ModsFolder = System.IO.Path.Combine(UnityEngine.Application.dataPath, "..");
                ModsFolder = System.IO.Path.Combine(ModsFolder, "Mods");
            }

            if (String.IsNullOrEmpty(ModsFolder))
                ModsFolder = "Mods";

            if (!System.IO.Directory.Exists(ModsFolder))
            {
                System.IO.Directory.CreateDirectory(ModsFolder);
            }

            Type AbstractModType = typeof(AbstractMod);

            System.IO.DirectoryInfo ModsFolderInfo = new System.IO.DirectoryInfo(ModsFolder);

            foreach(System.IO.FileInfo DllFile in ModsFolderInfo.GetFiles("*.Mod.dll", System.IO.SearchOption.AllDirectories))
            {
                try
                {
                    System.Reflection.Assembly ModAssembly = System.Reflection.Assembly.LoadFile(DllFile.FullName);
                    bool ModTypeFound = false;
                    foreach(Type T in ModAssembly.GetTypes().Where(t => t.IsSubclassOf(AbstractModType) && !t.IsInterface && !t.IsAbstract))
                    {
                        AbstractMod M = (AbstractMod)Activator.CreateInstance(T);
                        if (M != null)
                        {
                            M.ModWindow = new GUI.Window(ModGUI.GetWindowIndex(), M.ModName) { Visible = false, IsDraggable = true };

                            M.ModFolder = DllFile.Directory.FullName;
                            M.Enabled = true;

                            GUI.Button modBtn = new GUI.Button(M.ModName) { Tag = M, Visible = false };
                            modBtn.Clicked += ModBtn_Clicked;
                            _ModListButtonArea.Items.Add(modBtn);

                            _Mods.Add(M);
                            ModTypeFound = true;
                        }
                    }

                    if (!ModTypeFound)
                        UnityEngine.Debug.LogWarning("No mod found in file " + DllFile.Name + " but it's designated as a mod");
                }
                catch(Exception ex)
                {
                    UnityEngine.Debug.LogError("Failed to initialize mod from file " + DllFile.Name + ": " + ex.Message);
                }
            }

            foreach (AbstractMod M in _Mods.Where(m => m.Enabled))
            {
                M.OnInitialize();
                UnityEngine.Debug.Log("Initialized " + M.ModName + " - " + M.ModVersion.ToString());
            }

        }

        /// <summary>
        /// <para>Called from within the game's code. Notifying all enabled mods that a new game was started</para>
        /// <para>Do not call this manually</para>
        /// </summary>
        public void __OnGameStarted()
        {
            foreach (AbstractMod M in _Mods.Where(m => m.Enabled))
                M.OnGameStarted();
        }

        /// <summary>
        /// <para>Called from within the game's code. Notifying all enabled mods that a game was loaded from a save</para>
        /// <para>Do not call this manually</para>
        /// </summary>
        public void __OnGameLoaded(int saveSlot)
        {
            foreach (AbstractMod M in _Mods.Where(m => m.Enabled))
                M.OnGameLoad(saveSlot);
        }

        /// <summary>
        /// <para>Called from within the game's code. Notifying all enabled mods that the game is being saved</para>
        /// <para>Do not call this manually</para>
        /// </summary>
        public void __OnGameSaved(int saveSlot)
        {
            foreach (AbstractMod M in _Mods.Where(m => m.Enabled))
                M.OnGameSave(saveSlot);
        }

        /// <summary>
        /// <para>Called from within the game's code. Notifying all enabled mods that it can draw custom gui elements</para>
        /// <para>Do not call this manually</para>
        /// </summary>
        public void __OnGui()
        {
            if(ManagerMenu.mainMenuActive)
                _ModListButtonArea.Draw();

            foreach (AbstractMod M in _Mods.Where(m => m.Enabled))
            {
                if(ManagerMenu.mainMenuActive)
                    M.ModWindow.Draw();

                M.OnGUI();
            }
        }

        private void ModBtn_Clicked(GUI.GUIItem item)
        {
            AbstractMod M = item.Tag as AbstractMod;
            UnityEngine.Debug.Log("Toggleing window for " + M.ModName);
            M.ToggleModWindow();
        }

        private void ToggleModListBtn_Clicked(GUI.GUIItem item)
        {
            _ShowModList = !_ShowModList;
            UnityEngine.Debug.Log("Showing Mods: " + (_ShowModList ? "True" : "False"));
            if(_ShowModList)
            {
                foreach (GUI.GUIItem modbtn in _ModListButtonArea.Items)
                    modbtn.Visible = true;
            }
            else
            {
                foreach (GUI.GUIItem modbtn in _ModListButtonArea.Items)
                {
                    if(modbtn != _ToggleModListBtn)
                        modbtn.Visible = false;
                }
            }
        }
    }
}

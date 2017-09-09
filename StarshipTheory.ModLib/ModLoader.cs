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

        private GUI.Window _debugWindow;

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

            _ModListButtonArea = new GUI.Area(new UnityEngine.Rect(UnityEngine.Screen.width - 100, 0, 100, UnityEngine.Screen.height)) { MaxWidth = 200, MaxHeight = 400 };
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
                            M.ModWindow = new GUI.Window(ModGUI.GetWindowIndex(), M.ModName)
                            {
                                Visible = false,
                                IsDraggable = true,
                                IsResizeable = true,
                                _drawingMod = M,
                                MinWidth = 200,
                                MaxWidth = UnityEngine.Screen.width,
                                MinHeight = 20,
                                MaxHeight =  UnityEngine.Screen.height
                            };

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
                    ShowError(DllFile.Name.Replace(".Mod.dll", ""), "", ex, "Load");
                    UnityEngine.Debug.LogError("Failed to initialize mod from file " + DllFile.Name + ": " + ex.Message);
                }
            }

            foreach (AbstractMod M in _Mods.Where(m => m.Enabled))
            {
                try
                {

                    M.OnInitialize();
                    UnityEngine.Debug.Log("Initialized " + M.ModName + " - " + M.ModVersion.ToString());
                }
                catch(Exception ex)
                {
                    //TODO: Show Mod error window
                    ShowError(M, ex, "Initialization");
                    UnityEngine.Debug.LogError(ex);
                }
            }

        }

        /// <summary>
        /// <para>Called from within the game's code. Notifying all enabled mods that a new game was started</para>
        /// <para>Do not call this manually</para>
        /// </summary>
        public void __OnGameStarted()
        {
            foreach (AbstractMod M in _Mods.Where(m => m.Enabled))
            {
                try
                {
                    M.OnGameStarted();
                }
                catch(Exception ex)
                {
                    //TODO: Show Mod error window
                    ShowError(M, ex, "Game Started");
                    UnityEngine.Debug.LogError(ex);
                }
            }
        }

        /// <summary>
        /// <para>Called from within the game's code. Notifying all enabled mods that a game was loaded from a save</para>
        /// <para>Do not call this manually</para>
        /// </summary>
        public void __OnGameLoaded(int saveSlot)
        {
            foreach (AbstractMod M in _Mods.Where(m => m.Enabled))
            {
                try
                {
                    M.OnGameLoad(saveSlot);
                }
                catch (Exception ex)
                {
                    //TODO: Show Mod error window
                    ShowError(M, ex, "Game Loaded");
                    UnityEngine.Debug.LogError(ex);
                }
            }
        }

        /// <summary>
        /// <para>Called from within the game's code. Notifying all enabled mods that the game is being saved</para>
        /// <para>Do not call this manually</para>
        /// </summary>
        public void __OnGameSaved(int saveSlot)
        {
            foreach (AbstractMod M in _Mods.Where(m => m.Enabled))
            {
                try
                {
                    M.OnGameSave(saveSlot);
                }
                catch (Exception ex)
                {
                    //TODO: Show Mod error window
                    ShowError(M, ex, "Game Saved");
                    UnityEngine.Debug.LogError(ex);
                }
            }
        }

        private bool _FirstPass = true;
        private bool _ModAreaResized = false;

        /// <summary>
        /// <para>Called from within the game's code. Notifying all enabled mods that it can draw custom gui elements</para>
        /// <para>Do not call this manually</para>
        /// </summary>
        public void __OnGui()
        {
            if(_FirstPass)
            {
                _FirstPass = false;
                _debugWindow = new GUI.Window(ModGUI.GetWindowIndex(), "Mod Debug") { Visible = false, IsDraggable = true, IsResizeable = true };
                _debugWindow.Rect = new UnityEngine.Rect((UnityEngine.Screen.width - 400) / 2, (UnityEngine.Screen.height - 400) / 2, 400, 400);
                _debugWindow.Items.Add(new GUI.TextArea() { IsRichText = true, IsEditable = false });
                GUI.Button closeDebugBtn = new GUI.Button("Close");
                closeDebugBtn.Clicked += CloseDebugBtn_Clicked;
                _debugWindow.Items.Add(new GUI.FlexibleSpace());
                _debugWindow.Items.Add(closeDebugBtn);
            }

            _debugWindow.__Draw();

            if (ManagerMenu.mainMenuActive)
            {
                if(!_ModAreaResized)
                {
                    _ModAreaResized = true;
                }

                _ModListButtonArea.__Draw();
            }

            foreach (AbstractMod M in _Mods.Where(m => m.Enabled))
            {
                try
                {
                    if (!M._FirstGuiPassCalled)
                    {                            
                        M.FirstGUIPass();
                        M._FirstGuiPassCalled = true;
                    }

                    if (ManagerMenu.mainMenuActive && M.CanShowModWindow)
                        M.ModWindow.__Draw();

                    M.OnGUI();
                }
                catch(Exception ex)
                {
                    //TODO: Show Mod error window
                    ShowError(M, ex, "GUI");
                    UnityEngine.Debug.LogError(ex);
                }
            }
        }

        private void CloseDebugBtn_Clicked(GUI.GUIItem item)
        {
            _debugWindow.Visible = false;
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
                float MaxWidth = 0;
                foreach (GUI.Button modbtn in _ModListButtonArea.Items)
                {
                    modbtn.Visible = true;

                    UnityEngine.Vector2 size;

                    if (modbtn.Style != null)
                        size = modbtn.Style.CalcSize(new UnityEngine.GUIContent(modbtn.Text, modbtn.Image, modbtn.Tooltip));
                    else
                        size = UnityEngine.GUI.skin.button.CalcSize(new UnityEngine.GUIContent(modbtn.Text, modbtn.Image, modbtn.Tooltip));

                    UnityEngine.Debug.Log(modbtn.Text + ": " + size.x + ", " + size.y);

                    if (size.x > MaxWidth)
                        MaxWidth = size.x;
                }

                _ModListButtonArea.Size = new UnityEngine.Rect((float)UnityEngine.Screen.width - MaxWidth, 0, MaxWidth, (float)UnityEngine.Screen.height);
                UnityEngine.Debug.Log("ModListArea: (" + _ModListButtonArea.Size.x + ", " + _ModListButtonArea.Size.y + ", " + _ModListButtonArea.Size.width + ", " + _ModListButtonArea.Size.height);
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

        internal void ShowError(AbstractMod m, Exception error, String where = "")
        {
            ShowError(m.ModName, m.ModVersion.ToString(), error, where);
        }

        internal void ShowError(String modName, String modVersion, Exception error, String where = "")
        {
            UnityEngine.Debug.Log("Showing Error");
            _debugWindow.Title = "Error - " + modName + (!String.IsNullOrEmpty(modVersion) ? (" (" + modVersion + ")") : "");
            GUI.TextArea log = (GUI.TextArea)_debugWindow.Items.First();

            String msg = "<color=red>";
            if (!String.IsNullOrEmpty(where))
                msg += "<b>" + modName + "</b> threw an error during the <i>" + where + " process</i>\n";
            else
                msg += "<b>" + modName + "</b> threw an error:\n";

            msg += ExceptionToMessage(error);

            msg += "</color>";

            log.Text = msg;

            _debugWindow.Visible = true;
        }

        private String ExceptionToMessage(Exception ex, int indentC = 0)
        {
            String indent = "";
            for (int i = 0; i < indentC; i++)
                indent += "  ";

            String str = indent + "<b>Exception Type:</b> " + ex.GetType().Name + "\n";
            str += indent + "<b>Message:</b> " + ex.Message + "\n";
            if(ex.Data != null && ex.Data.Count > 0)
            {
                str += indent + "<b>Data:</b>\n";
                foreach(Object Key in ex.Data.Keys)
                    str += indent + "  " + Key.ToString() + " = " + ex.Data[Key].ToString() + "\n";
            }

            if(!String.IsNullOrEmpty(ex.Source))
                str += indent + "<b>Source:</b> " + ex.Source + "\n";

            if (!String.IsNullOrEmpty(ex.StackTrace))
                str += indent + "<b>Stacktrace:</b> " + ex.StackTrace + "\n";

            if (ex.InnerException != null)
            {
                str += indent + "<b>Inner Exception:</b>\n";
                str += ExceptionToMessage(ex.InnerException, indentC + 1);
            }

            return str;
        }
    }
}

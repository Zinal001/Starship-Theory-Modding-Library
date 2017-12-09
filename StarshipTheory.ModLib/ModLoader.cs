using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarshipTheory.ModLib
{
    public sealed class ModLoader
    {
        internal static ModLoader Instance { get; private set; }

        private List<Mod> _Mods;

        public Mod[] Mods
        {
            get { return _Mods.ToArray(); }
        }

        private bool _ShowModList = false;
        private GUI.Area _ModListButtonArea;
        private GUI.Button _ToggleModListBtn;

        private GUI.Window _debugWindow;

        private List<ExceptionInfo> _errorBeforeLoad = new List<ExceptionInfo>();

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
            _Mods = new List<Mod>();

            SetupGUI();

            String ModsFolder = null;
            if (UnityEngine.Application.platform == UnityEngine.RuntimePlatform.WindowsPlayer)
            {
                ModsFolder = System.IO.Path.Combine(UnityEngine.Application.dataPath, "..");
                ModsFolder = System.IO.Path.Combine(ModsFolder, "Mods");
            }
            //TODO: Support Mac/Linux platform

            if (String.IsNullOrEmpty(ModsFolder))
                ModsFolder = "Mods";

            if (!System.IO.Directory.Exists(ModsFolder))
                System.IO.Directory.CreateDirectory(ModsFolder);
            
            RegisterMods(ModsFolder);

            foreach (Mod M in _Mods.Where(m => m.Enabled))
            {
                try
                {
                    M.OnInitialize();
                    UnityEngine.Debug.Log("Initialized " + M.Info.Name + " - " + M.Info.Version.ToString());
                }
                catch(Exception ex)
                {
                    ShowError(M, ex, "Initialization");
                    UnityEngine.Debug.LogError(ex);
                }
            }

        }

        /// <summary>
        /// Setup the Mods-GUI list on the main menu
        /// </summary>
        private void SetupGUI()
        {
            _ModListButtonArea = new GUI.Area(new UnityEngine.Rect(UnityEngine.Screen.width - 150, 0, 150, UnityEngine.Screen.height)) { MaxWidth = 200, MaxHeight = 400 };
            _ToggleModListBtn = new GUI.Button("Mods");
            _ToggleModListBtn.Clicked += ToggleModListBtn_Clicked;
            _ModListButtonArea.Items.Add(_ToggleModListBtn);
        }

        /// <summary>
        /// Register every Mod in the Mods directory
        /// </summary>
        /// <param name="ModsFolder"></param>
        private void RegisterMods(String ModsFolder)
        {
            Type AbstractModType = typeof(Mod);

            System.IO.DirectoryInfo ModsFolderInfo = new System.IO.DirectoryInfo(ModsFolder);

            List<Mod> PreliminaryMods = new List<Mod>();

            foreach(System.IO.DirectoryInfo modDir in ModsFolderInfo.GetDirectories("*", System.IO.SearchOption.TopDirectoryOnly))
            {
                if(System.IO.File.Exists(System.IO.Path.Combine(modDir.FullName, "Mod.json")))
                {
                    try
                    {
                        String modInfoJson = System.IO.File.ReadAllText(System.IO.Path.Combine(modDir.FullName, "Mod.json"));
                        ModInfo modInfo = Pathfinding.Serialization.JsonFx.JsonReader.Deserialize<ModInfo>(modInfoJson);
                        if (modInfo == null)
                            throw new Exception("Invalid Mod.json file");

                        try
                        {
                            String dllFile = System.IO.Path.Combine(modDir.FullName, modInfo.EntryDLL);
                            if (String.IsNullOrEmpty(modInfo.EntryDLL) || !System.IO.File.Exists(dllFile))
                                throw new System.IO.FileNotFoundException("Missing EntryDLL file");
                            
                            System.Reflection.Assembly ModAssembly = System.Reflection.Assembly.LoadFile(dllFile);
                            bool ModTypeFound = false;
                            foreach (Type T in ModAssembly.GetTypes().Where(t => t.IsSubclassOf(AbstractModType) && t.IsPublic && !t.IsInterface && !t.IsAbstract))
                            {
                                Mod M = (Mod)Activator.CreateInstance(T);
                                if (M != null)
                                {
                                    M.Info = modInfo;
                                    M.ModFolder = modDir.FullName;
                                    PreliminaryMods.Add(M);
                                    ModTypeFound = true;
                                }
                            }

                            if (!ModTypeFound)
                                UnityEngine.Debug.LogWarning("No mod found in file " + dllFile + " but it's designated as a mod");
                        }
                        catch(Exception innerEx)
                        {
                            ShowError(modInfo.Name, modInfo.Version.ToString(), innerEx, "Load");
                            UnityEngine.Debug.LogError("Failed to initialize mod from folder " + modInfo.Name + ": " + innerEx.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowError(modDir.Name, "", ex, "Load");
                        UnityEngine.Debug.LogError("Failed to initialize mod from folder " + modDir.Name + ": " + ex.Message);
                    }
                }
            }

            foreach(Mod M in PreliminaryMods)
            {
                foreach(ModDependency dep in M.Info.Dependencies)
                {
                    bool has = false;
                    foreach (Mod N in PreliminaryMods)
                    {
                        if (!N.Enabled)
                            continue;

                        if(dep.Name == N.Info.Name)
                        {
                            has = true;
                            if (dep.MinimumVersion != null && dep.MinimumVersion > N.Info.Version)
                                has = false;
                            else if (dep.MaximumVersion != null && dep.MaximumVersion < N.Info.Version)
                                has = false;
                        }
                    }

                    if (!has)
                    {
                        UnityEngine.Debug.LogWarning("Mod " + M.Info.Name + " is missing dependency " + dep.Name);
                        M.Enabled = false;
                        break;
                    }
                }

                if (M.Enabled)
                    _Mods.Add(M);
            }

            foreach (Mod M in _Mods)
            {
                M.ModWindow = new GUI.Window(ModGUI.GetWindowIndex(), M.Info.DisplayName)
                {
                    Visible = false,
                    IsDraggable = true,
                    IsResizeable = true,
                    _drawingMod = M,
                    MinWidth = 200,
                    MaxWidth = UnityEngine.Screen.width,
                    MinHeight = 20,
                    MaxHeight = UnityEngine.Screen.height
                };

                GUI.Button modBtn = new GUI.Button(M.Info.DisplayName) { Tag = M, Visible = false };
                modBtn.Clicked += ModBtn_Clicked;
                _ModListButtonArea.Items.Add(modBtn);

                M.OnInitialize();
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

                if(_errorBeforeLoad.Count > 0)
                    ShowError(_errorBeforeLoad[0].ModName, _errorBeforeLoad[0].ModVersion, _errorBeforeLoad[0].Error, _errorBeforeLoad[0].Where);

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
        }

        private void CloseDebugBtn_Clicked(GUI.GUIItem item)
        {
            _debugWindow.Visible = false;
        }

        private void ModBtn_Clicked(GUI.GUIItem item)
        {
            Mod M = item.Tag as Mod;
            M.ToggleModWindow();
        }

        private void ToggleModListBtn_Clicked(GUI.GUIItem item)
        {
            _ShowModList = !_ShowModList;
            if (_ShowModList)
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

        internal void ShowError(Mod m, Exception error, String where = "")
        {
            ShowError(m.Info.DisplayName, m.Info.Version.ToString(), error, where);
        }

        internal void ShowError(String modName, String modVersion, Exception error, String where = "")
        {
            if (_debugWindow == null)
            {
                _errorBeforeLoad.Add(new ExceptionInfo() { ModName = modName, ModVersion = modVersion, Error = error, Where = where });
                return;
            }

            String msg = "<color=red>";
            if (!String.IsNullOrEmpty(where))
                msg += "<b>" + modName + "</b> threw an error during the <i>" + where + " process</i>\n";
            else
                msg += "<b>" + modName + "</b> threw an error:\n";

            msg += ExceptionToMessage(error);

            msg += "</color>";

            _debugWindow.Title = "Error - " + modName + (!String.IsNullOrEmpty(modVersion) ? (" (" + modVersion + ")") : "");
            GUI.TextArea log = (GUI.TextArea)_debugWindow.Items.First();

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

            if(ex is TypeLoadException)
                str += indent + "<b>Type Name:</b> " + ((TypeLoadException)ex).TypeName + "\n";
            else if(ex is System.Reflection.ReflectionTypeLoadException)
            {
                System.Reflection.ReflectionTypeLoadException rex = (System.Reflection.ReflectionTypeLoadException)ex;

                if (rex == null)
                    str += indent + "<b><color=yellow>REFLECTION TYPE LOAD FAILED</color></b>";
                else
                {
                    if (rex.LoaderExceptions != null && rex.LoaderExceptions.Length > 0)
                    {
                        str += indent + "<b>Loader exceptions:</b>\n";
                        foreach (Exception subEx in rex.LoaderExceptions)
                        {
                            if(subEx != null)
                                str += ExceptionToMessage(subEx, indentC + 1);
                        }
                    }
                }
            }

            if (ex.InnerException != null)
            {
                str += indent + "<b>Inner Exception:</b>\n";
                str += ExceptionToMessage(ex.InnerException, indentC + 1);
            }

            return str;
        }

        private class ExceptionInfo
        {
            public String ModName { get; set; }
            public String ModVersion { get; set; }
            public Exception Error { get; set; }
            public String Where { get; set; }
        }

    }
}

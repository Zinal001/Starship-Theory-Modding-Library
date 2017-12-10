using StarshipTheory.ModLib;
using Events = StarshipTheory.ModLib.Events;
using GUI = StarshipTheory.ModLib.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using StarshipTheory.ModLib.Util;

namespace Scenarios
{
    public class ScenariosMod : Mod
    {
        private List<Scenario> _Scenarios;

        public Scenario[] Scenarios
        {
            get
            {
                return _Scenarios.ToArray();
            }
        }

        private Scenario _SelectedScenario = null;
        private Scenario _LoadingScenario = null;

        private GameObject mainMenuRight;

        private GUI.Window _ScenarioWindow;
        private GUI.Group lstScenarios;
        private GUI.Group _ScenarioInfo;

        private GUI.Button _LoadBtn;
        private GUI.Button _CancelBtn;

        private GUI.Label _Name;
        private GUI.TextArea _Desc;
        private GUI.Box _ThumbNail;

        private GUIStyle _LoadingStyle = null;
        private bool _IsLoading = false;

        public ScenariosMod()
        {
            _Scenarios = new List<Scenario>();
        }

        public override void OnInitialize()
        {
            this.CanShowModWindow = false;
            Events.MenuEvents.NewGamePanelOpened -= MenuEvents_NewGamePanelOpened; //In case this method is called multiple times.
            Events.MenuEvents.NewGamePanelOpened += MenuEvents_NewGamePanelOpened;

            Events.GameEvents.GameStarted -= GameEvents_GameStarted;
            Events.GameEvents.GameStarted += GameEvents_GameStarted;

            LoadScenarios();
        }

        public override void OnCreateGUI()
        {
            _LoadingStyle = new GUIStyle(UnityEngine.GUI.skin.box) {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 28
            };
        }

        private void GameEvents_GameStarted(object sender, EventArgs e)
        {
            if(_LoadingScenario != null)
            {
                GameObject.Find("Manager").GetComponent<ManagerJobs>().StartCoroutine(StartScenario(_LoadingScenario));
            }
        }

        private System.Collections.IEnumerator StartScenario(Scenario scenario)
        {
            Logger.Log("Starting Scenario " + scenario.Name);
            _IsLoading = true;

            yield return new WaitForRealSeconds(0.01f);

            TilesUtil.Import(scenario.Ship.Design, new Action<Exception>(ScenarioStarted));
        }

        private void ScenarioStarted(Exception error)
        {
            GameObject _Manager = GameObject.Find("Manager");

            Logger.Log("Scenario started: " + (error == null ? "No error" : error.Message));
            if (error != null)
                DisplayError(error);
            else
            {
                GameObject tileMap = GameObject.Find("TileMap");
                Crew shipCrew = tileMap.GetComponent<Crew>();
                Structures shipStructures = tileMap.GetComponent<Structures>();


                bool gameOverOnCrewDeath = _Manager.GetComponent<ManagerOptions>().deadCrewEndGame;
                _Manager.GetComponent<ManagerOptions>().deadCrewEndGame = false;


                GameObject[] crew = new GameObject[shipCrew.crewList.Count];
                shipCrew.crewList.CopyTo(crew);

                foreach (GameObject crewMember in crew)
                {
                    AI ai = crewMember.GetComponent<AI>();
                    ai.despawn();
                }

                foreach(Scenario.CrewMember crewMember in _LoadingScenario.Ship.Crew)
                {
                    shipCrew.spawnAI(true, Vector3.zero, crewMember.Role.ToString());
                    AI ai = shipCrew.crewList[shipCrew.crewList.Count - 1].GetComponent<AI>();
                    if(shipCrew.crewList.Count == 1)
                        ai.debugLog = true;
                    ai.manuallySetupCharacterCosmeticsAndSkills(crewMember.Agility - 1, crewMember.Endurance - 1, crewMember.Engineering - 1, crewMember.Intelligence - 1, crewMember.Combat - 1, (int)crewMember.Gender, crewMember.HairColor.GetColor(), crewMember.HeadColor.GetColor(), crewMember.HairType, crewMember.HeadType, crewMember.BodyType, crewMember.Name);
                }

                shipStructures.structure[0].cargoMax = _LoadingScenario.Ship.Cargo.CargoSpace;
                shipStructures.structure[0].reservesGold = _LoadingScenario.Ship.Cargo.Gold;
                shipStructures.structure[0].reservesMetal = _LoadingScenario.Ship.Cargo.Metal;
                shipStructures.structure[0].reservesSilicon = _LoadingScenario.Ship.Cargo.Silicon;
                shipStructures.structure[0].reservesWater = _LoadingScenario.Ship.Cargo.Water;
                shipStructures.structure[0].reservesFood = _LoadingScenario.Ship.Cargo.Food;
                shipStructures.structure[0].credits = _LoadingScenario.Ship.Cargo.Credits;
                shipStructures.structure[0].cargoCurrent = _LoadingScenario.Ship.Cargo.CurrentCargo;

                _Manager.GetComponent<ManagerOptions>().deadCrewEndGame = gameOverOnCrewDeath;

                foreach(TileData tile in _LoadingScenario.Ship.Design)
                {
                    shipStructures.stopLeakSource(tile.X, tile.Y);
                    shipStructures.powerStructure(tile.X, tile.Y, tile.TileType, true, "ExporterImporter.OnImportFinished");
                }

            }

            _IsLoading = false;
        }

        private void LoadScenarios()
        {
            _Scenarios.Clear();
            _SelectedScenario = null;

            DirectoryInfo scenariosDir = new DirectoryInfo(Path.Combine(ModFolder, "Scenarios"));
            if (!scenariosDir.Exists)
                scenariosDir.Create();
            else
            {
                List<String> scenarioErrors = new List<string>();

                foreach(FileInfo scenarioFile in scenariosDir.GetFiles("Scenario.json", SearchOption.AllDirectories))
                {
                    try
                    {
                        String json = File.ReadAllText(scenarioFile.FullName);

                        Scenario scenario = Pathfinding.Serialization.JsonFx.JsonReader.Deserialize<Scenario>(json);
                        if (scenario != null)
                        {
                            try
                            {
                                if (scenario.Validate())
                                {
                                    scenario.ScenarioFolder = scenarioFile.Directory.FullName;
                                    _Scenarios.Add(scenario);
                                }
                            }
                            catch(Exception scenarioException)
                            {
                                scenarioErrors.Add("\t" + scenario.Name + ": " + scenarioException.Message);
                                Logger.Log(LogType.Warning, "Failed to load scenario " + scenario.Name + ": " + scenarioException.Message);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException(ex);
                    }
                }

                if (scenarioErrors.Count > 0)
                    DisplayError(new Exception("Failed to load the following scenarios:\n" + String.Join("\n", scenarioErrors.ToArray())));
            }
        }

        private void MenuEvents_NewGamePanelOpened(object sender, Events.MenuEvents.MenuGameObjectEventArgs e)
        {
            GameObject _Manager = GameObject.Find("Manager");
            mainMenuRight = e.Parent;
            mainMenuRight.SetActive(false);

            SetupGUI(mainMenuRight);
            _ScenarioWindow.Visible = true;

            if (_MainMenuModeCoRoutine != null)
            {
                _Manager.GetComponent<ManagerMenu>().StopCoroutine(_MainMenuModeCoRoutine);
                _MainMenuModeCoRoutine = null;
            }

            _MainMenuModeCoRoutine = _Manager.GetComponent<ManagerMenu>().StartCoroutine(CheckMainMenuMode());
        }

        private Coroutine _MainMenuModeCoRoutine = null;

        private System.Collections.IEnumerator CheckMainMenuMode()
        {
            while(_ScenarioWindow.Visible && (ManagerMenu.mainMenuMode == null || ManagerMenu.mainMenuMode == "" || ManagerMenu.mainMenuMode == "Base" || ManagerMenu.mainMenuMode == "New"))
                yield return new WaitForRealSeconds(0.5f);

            _ScenarioWindow.Visible = false;
            mainMenuRight.SetActive(true);
        }

        private void SetupGUI(GameObject mainMenuRight)
        {
            _SelectedScenario = null;

            _ScenarioWindow = new GUI.Window(ModGUI.GetWindowIndex(), "Scenarios") {
                IsDraggable = false,
                IsResizeable = false,
                Rect = new Rect((Screen.width / 2) - 200, (Screen.height / 2) - 200, 400, 400),
                Visible = false,
                MinWidth = 200,
                MinHeight = 200
            };

            GUI.Group group = new GUI.Group(GUI.GUIItem.Direction.Vertical);
            _ScenarioWindow.Items.Add(group);

            GUI.Group group1 = new GUI.Group(GUI.GUIItem.Direction.Horizontal);
            group.Items.Add(group1);

            lstScenarios = new GUI.Group(GUI.GUIItem.Direction.Vertical);
            group1.Items.Add(lstScenarios);
            
            foreach(Scenario scenario in Scenarios)
            {
                GUI.Button btn = new GUI.Button(scenario.Name)
                {
                    Tag = scenario
                };
                btn.Clicked += ScenarioBtn_Clicked;
                lstScenarios.Items.Add(btn);
            }

            _ScenarioInfo = new GUI.Group(GUI.GUIItem.Direction.Vertical)
            {
                Visible = false
            };
            group1.Items.Add(_ScenarioInfo);

            _Name = new GUI.Label("");
            _ScenarioInfo.Items.Add(_Name);

            _Desc = new GUI.TextArea("") {
                IsEditable = false,
                IsRichText = true
            };
            _ScenarioInfo.Items.Add(_Desc);

            _ThumbNail = new GUI.Box("")
            {
                Visible = false,
                MaxWidth = 200,
                MaxHeight = 200
            };
            _ScenarioInfo.Items.Add(_ThumbNail);

            GUI.Group btnGroup = new GUI.Group(GUI.GUIItem.Direction.Horizontal);
            group.Items.Add(btnGroup);

            _CancelBtn = new GUI.Button("Cancel");
            _CancelBtn.Clicked += CancelBtn_Clicked;
            btnGroup.Items.Add(_CancelBtn);

            btnGroup.Items.Add(new GUI.FlexibleSpace());

            _LoadBtn = new GUI.Button("Load") { IsEnabled = false };
            _LoadBtn.Clicked += LoadBtn_Clicked;
            btnGroup.Items.Add(_LoadBtn);
        }

        private void CancelBtn_Clicked(GUI.GUIItem item)
        {
            _ScenarioWindow.Visible = false;
            mainMenuRight.SetActive(true);
        }

        private void LoadBtn_Clicked(GUI.GUIItem item)
        {
            if(_SelectedScenario != null)
            {
                GameObject _Manager = GameObject.Find("Manager");

                _LoadingScenario = _SelectedScenario;

                _ScenarioWindow.Visible = false;
                mainMenuRight.SetActive(true);

                ManagerMenu.renameShipString = _SelectedScenario.Ship.Name;
                ManagerMenu.nextLevelToLoad = 100;
                _Manager.GetComponent<ManagerOptions>().resetBoard();
            }
        }

        private void ScenarioBtn_Clicked(GUI.GUIItem item)
        {
            Scenario scenario = item.Tag as Scenario;
            _Name.Text = scenario.Name;
            _Desc.Text = scenario.Description;

            if (!String.IsNullOrEmpty(scenario.ThumbnailFile) && File.Exists(Path.Combine(scenario.ScenarioFolder, scenario.ThumbnailFile)))
            {
                Texture tex = LoadTexture(Path.Combine(scenario.ScenarioFolder, scenario.ThumbnailFile));
                if(tex != null)
                {
                    _ThumbNail.Visible = true;
                    _ThumbNail.Image = tex;
                }
            }
            else
            {
                _ThumbNail.Visible = false;
            }

            _SelectedScenario = scenario;
            _ScenarioInfo.Visible = true;
        }

        public override void OnGUI()
        {
            if (_ScenarioWindow != null && _ScenarioWindow.Visible)
            {
                _LoadBtn.IsEnabled = _SelectedScenario != null;
                _ScenarioWindow.Draw();
            }

            if (_IsLoading)
                UnityEngine.GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "Loading Scenario, Please wait...", _LoadingStyle);
        }

        private Texture2D LoadTexture(String filePath)
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            Texture2D tex = new Texture2D(2, 2);
            if (tex.LoadImage(fileData))
            {
                if(tex.width > 200)
                {
                    float ratio = (float)tex.width / (float)tex.height;

                    int newHeight = (int)Mathf.Floor(200f * ratio);
                    
                    TextureScale.Bilinear(tex, 200, newHeight);
                }
                return tex;
            }

            return null;
        }

    }
}

using System;
using System.Text;
using StarshipTheory.ModLib;
using GUI = StarshipTheory.ModLib.GUI;
using UnityEngine;
using System.Collections.Generic;

namespace ExporterImporter.Mod
{
    /// <summary>
    /// W.I.P
    /// </summary>
    public class ExporterImporter : AbstractMod
    {
        private GameObject _Manager;

        public override string ModName => "Exporter/Importer";

        public override Version ModVersion => new Version("1.0.0");

        public override string ModDescription => "Export/Import ship design";

        private bool _GameStarted = false;

        private bool _IsImporting = false;

        private GUIStyle _loadingBoxStyle;

        public override void OnInitialize()
        {
            _Manager = GameObject.Find("Manager");

        }

        public override void FirstGUIPass()
        {
            _loadingBoxStyle = new GUIStyle(UnityEngine.GUI.skin.box) {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 16
            };

            GUI.Group btnGroup = new GUI.Group(GUI.GUIItem.Direction.Horizontal);

            GUI.Button exportBtn = new GUI.Button("Export");
            exportBtn.Clicked += ExportBtn_Clicked;
            btnGroup.Items.Add(exportBtn);

            GUI.Button importBtn = new GUI.Button("Import");
            importBtn.Clicked += ImportBtn_Clicked;
            btnGroup.Items.Add(importBtn);

            ModWindow.Items.Add(btnGroup);
        }

        private void ImportBtn_Clicked(GUI.GUIItem item)
        {
            if (_GameStarted && _Manager != null)
            {
                MouseUI mouseUI = _Manager.GetComponent<MouseUI>();

                GameObject tileMap = mouseUI._ModGet_TileMap();
                Tiles shipTiles = tileMap.GetComponent<Tiles>();
                ManagerJobs managerJobs = _Manager.GetComponent<ManagerJobs>();
                ManagerMenu managerMenu = _Manager.GetComponent<ManagerMenu>();
                ManagerOptions managerOptions = _Manager.GetComponent<ManagerOptions>();

                if(shipTiles != null)
                {
                    String json = System.IO.File.ReadAllText(System.IO.Path.Combine(ModFolder, "Design.json"), Encoding.UTF8);

                    TileData[] tiles = Pathfinding.Serialization.JsonFx.JsonReader.Deserialize<TileData[]>(json);

                    List<TileData> hull = new List<TileData>();
                    List<TileData> floor = new List<TileData>();
                    TileData shipCore = null;
                    List<TileData> other = new List<TileData>();

                    foreach (TileData td in tiles)
                    {
                        List<TileData.Vec3> StructureParts = new List<TileData.Vec3>();
                        StructureParts.AddRange(td.StructureParts);
                        td.StructureParts = StructureParts;

                        if (td.TileType == "Hull" || td.TileType == "HullCorner")
                            hull.Add(td);
                        else if (td.TileType == "Floor")
                            floor.Add(td);
                        else if (td.TileType == "ShipCore")
                            shipCore = td;
                        else
                            other.Add(td);
                    }

                    if(shipCore == null)
                        throw new Exception("Ship Design is missing required tile ShipCore");

                    _IsImporting = true;
                    _Manager.GetComponent<ManagerMenu>().toggleMainMenu();
                    managerMenu.StartCoroutine(UpdateTiles(shipTiles, managerJobs, managerOptions, tileMap, hull, floor, shipCore, other));
                    managerMenu.StartCoroutine(OnImportFinished(tiles));
                }
            }
        }

        private System.Collections.IEnumerator OnImportFinished(TileData[] tiles)
        {
            while(true)
            {
                if(!_IsImporting)
                {
                    Tiles shipTiles = GameObject.Find("TileMap").GetComponent<Tiles>();
                    Structures shipStructures = GameObject.Find("TileMap").GetComponent<Structures>();
                    shipStructures.checkForLeaks = true;
                    foreach (TileData td in tiles)
                    {
                        shipTiles.tiles[td.X, td.Y].leakValue = 0;
                        shipStructures.stopLeakSource(td.X, td.Y);
                        shipStructures.updateLeakBalance1(td.X, td.Y);
                        shipStructures.powerStructure(td.X, td.Y, td.TileType, true, "ExporterImporter.OnImportFinished");
                    }

                    break;
                }

                yield return new WaitForSeconds(0.01f);
            }
        }

        private System.Collections.IEnumerator UpdateTiles(Tiles shipTiles, ManagerJobs managerJobs, ManagerOptions managerOptions, GameObject tileMap, List<TileData> hull, List<TileData> floor, TileData shipCore, List<TileData> other)
        {
            bool deadCrewEndGame = true;
            try
            {
                if (managerOptions != null)
                {
                    deadCrewEndGame = managerOptions.deadCrewEndGame;
                    managerOptions.deadCrewEndGame = false;
                }
            }
            catch(Exception ex)
            {
                this.DisplayError(ex);
                _Manager.GetComponent<ManagerMenu>().toggleMainMenu();
                _IsImporting = false;
                yield break;
            }
            yield return new WaitForSeconds(0.01f);

            Structures structures = null;
            int cargoGold = 0;
            int cargoMetal = 0;
            int cargoSilicon = 0;
            int cargoWater = 0;
            int cargoFood = 0;
            int cargoCredits = 0;

            try
            {
                structures = tileMap.GetComponent<Structures>();

                cargoGold = structures.structure[0].reservesGold;
                cargoMetal = structures.structure[0].reservesMetal;
                cargoSilicon = structures.structure[0].reservesSilicon;
                cargoWater = structures.structure[0].reservesWater;
                cargoFood = structures.structure[0].reservesFood;
                cargoCredits = structures.structure[0].credits;

                List<Vector2> tilesToRemove = new List<Vector2>();

                for (int x = shipTiles.tiles.GetLowerBound(0); x <= shipTiles.tiles.GetUpperBound(0); x++)
                {
                    for (int y = shipTiles.tiles.GetLowerBound(1); y <= shipTiles.tiles.GetUpperBound(1); y++)
                    {
                        if (String.IsNullOrEmpty(shipTiles.tiles[x, y].toBecome) && String.IsNullOrEmpty(shipTiles.tiles[x, y].tileType) && String.IsNullOrEmpty(shipTiles.tiles[x, y].structureType))
                            continue;

                        tilesToRemove.Add(new Vector2(x, y));
                    }
                }

                TileHelper.RemoveTiles(tilesToRemove);
            }
            catch (Exception ex)
            {
                this.DisplayError(ex);
                _Manager.GetComponent<ManagerMenu>().toggleMainMenu();
                _IsImporting = false;
                yield break;
            }
            
            yield return new WaitForSeconds(0.01f);


            if (hull.Count > 0)
            {
                try
                {
                    TileHelper.BuildTiles(hull);
                }
                catch (Exception ex)
                {
                    this.DisplayError(ex);
                    _Manager.GetComponent<ManagerMenu>().toggleMainMenu();
                    _IsImporting = false;
                    yield break;
                }
                yield return new WaitForSeconds(0.01f);
            }

            if (floor.Count > 0)
            {
                try
                {
                    TileHelper.BuildTiles(floor);
                }
                catch (Exception ex)
                {
                    this.DisplayError(ex);
                    _Manager.GetComponent<ManagerMenu>().toggleMainMenu();
                    _IsImporting = false;
                    yield break;
                }
                yield return new WaitForSeconds(0.01f);
            }

            if(shipCore != null)
            {
                try
                {
                    TileHelper.BuildTiles(shipCore);
                }
                catch (Exception ex)
                {
                    this.DisplayError(ex);
                    _Manager.GetComponent<ManagerMenu>().toggleMainMenu();
                    _IsImporting = false;
                    yield break;
                }
                yield return new WaitForSeconds(0.01f);
            }

            if (other.Count > 0)
            {
                try
                {
                    TileHelper.BuildTiles(other);
                }
                catch (Exception ex)
                {
                    this.DisplayError(ex);
                    _Manager.GetComponent<ManagerMenu>().toggleMainMenu();
                    _IsImporting = false;
                    yield break;
                }
                yield return new WaitForSeconds(0.01f);
            }

            try
            {
                shipTiles.updateTileColors();
                shipTiles.updateTileMesh("All");

                Crew crew = tileMap.GetComponent<Crew>();
                foreach (GameObject crewMember in crew.crewList)
                    crewMember.transform.position = new Vector3(shipCore.X, crewMember.transform.position.y, shipCore.Y);
            }
            catch(Exception ex)
            {
                this.DisplayError(ex);
                _Manager.GetComponent<ManagerMenu>().toggleMainMenu();
                _IsImporting = false;
                yield break;
            }
            yield return new WaitForSeconds(0.01f);

            try
            {
                if (managerOptions != null)
                    managerOptions.deadCrewEndGame = deadCrewEndGame;

                ManagerResources managerResources = _Manager.GetComponent<ManagerResources>();

                structures.structure[0].reservesGold = 0;
                structures.structure[0].reservesMetal = 0;
                structures.structure[0].reservesSilicon = 0;
                structures.structure[0].reservesWater = 0;
                structures.structure[0].reservesFood = 0;
                structures.structure[0].credits = 0;
                structures.structure[0].cargoCurrent = 0;


                managerResources.updateResourceReserves("Gold", cargoGold, tileMap, "");
                managerResources.updateResourceReserves("Metal", cargoMetal, tileMap, "");
                managerResources.updateResourceReserves("Silicon", cargoSilicon, tileMap, "");
                managerResources.updateResourceReserves("Water", cargoWater, tileMap, "");
                managerResources.updateResourceReserves("Food", cargoFood, tileMap, "");
                managerResources.updateCredits(cargoCredits, false);
            }
            catch(Exception ex)
            {
                this.DisplayError(ex);
                _Manager.GetComponent<ManagerMenu>().toggleMainMenu();
                _IsImporting = false;
                yield break;
            }
            yield return new WaitForSeconds(0.01f);

            _Manager.GetComponent<ManagerMenu>().toggleMainMenu();
            _IsImporting = false;
        }

        private void ExportBtn_Clicked(GUI.GUIItem item)
        {
            if(_GameStarted && _Manager != null)
            {
                List<String> allowedTiles = new List<string>() {
                    "Hull",
                    "HullCorner",
                    "Floor"
                };

                GameObject tileMap = GameObject.Find("TileMap");
                Tiles shipTiles = tileMap.GetComponent<Tiles>();
                Structures structures = tileMap.GetComponent<Structures>();
                if (shipTiles != null && structures != null)
                {
                    List<Vector2> ignoreTileData = new List<Vector2>();
                    List<TileData> tiles = new List<TileData>();

                    foreach(Vector2 pos in structures.structure[0].allTilesList)
                    {
                        TileInfo info = shipTiles.tiles[(int)pos.x, (int)pos.y];

                        if (ignoreTileData.Contains(pos))
                        {
                            if (!allowedTiles.Contains(info.structureType) && !allowedTiles.Contains(info.toBecome))
                                continue;
                        }

                        ignoreTileData.Add(pos);

                        List<TileData.Vec3> StructureParts = new List<TileData.Vec3>();
                        foreach (Vector3 vec in info.structureParts)
                            StructureParts.Add(TileData.Vec3.FromVector(vec));

                        TileData.TileRotation Rotation = TileData.TileRotation.UP;

                        if (info.structureParts.Count > 0)
                            Rotation = (TileData.TileRotation)(int)info.structureParts[0].z;

                        if(!String.IsNullOrEmpty(info.structureType))
                        {
                            TileData td2 = new TileData() {
                                TileType = info.structureType,
                                X = (int)pos.x,
                                Y = (int)pos.y,
                                Rotation = Rotation,
                                StructureParts = Extensions.Copy<TileData.Vec3>(StructureParts)
                            };

                            tiles.Add(td2);
                            StructureParts.Clear(); //StructureParts mostly likely belongs to the structure, not the tile! --Need confirmation.
                        }

                        TileData td = new TileData() {
                            TileType = info.tileType,
                            X = (int)pos.x,
                            Y = (int)pos.y,
                            Rotation = Rotation,
                            StructureParts = StructureParts
                        };

                        tiles.Add(td);
                    }

                    if(tiles.Count > 0)
                    {
                        String json = Pathfinding.Serialization.JsonFx.JsonWriter.Serialize(tiles.ToArray());
                        System.IO.File.WriteAllText(System.IO.Path.Combine(ModFolder, "Design.json"), json, Encoding.UTF8);
                    }
                }
            }
        }

        public override void OnGameLoad(int saveSlot)
        {
            base.OnGameLoad(saveSlot);
            _GameStarted = true;
        }

        public override void OnGameStarted()
        {
            base.OnGameStarted();
            _GameStarted = true;
        }

        public override void OnGUI()
        {
            if (_IsImporting)
            {
                UnityEngine.GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "Importing Design, Please wait...", _loadingBoxStyle);
            }

        }
    }
}

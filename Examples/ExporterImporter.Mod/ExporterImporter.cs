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

            TileHelper.Init(this.ModFolder);

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
                    #region testData
                    /*List<TileData> tiles = new List<TileData>();
                    for(int x = 50; x <= 60; x++)
                    {
                        for(int y = 50; y <= 60; y++)
                        {
                            if (x == 50 || x == 60 || y == 50 || y == 60)
                            {
                                if(x == 50 && y == 50)
                                    tiles.Add(new TileData(x, y, "HullCorner", BuildData.BuildDataRotation.LEFT));
                                else if(x == 60 && y == 50)
                                    tiles.Add(new TileData(x, y, "HullCorner", BuildData.BuildDataRotation.DOWN));
                                else if(x == 50 && y == 60)
                                    tiles.Add(new TileData(x, y, "HullCorner", BuildData.BuildDataRotation.UP));
                                else if(x == 60 && y == 60)
                                    tiles.Add(new TileData(x, y, "HullCorner", BuildData.BuildDataRotation.RIGHT));
                                else
                                    tiles.Add(new TileData(x, y, "Hull"));
                            }
                            else
                                tiles.Add(new TileData(x, y, "Floor"));

                            if (y == 50)
                            {
                                if (x == 51)
                                    tiles.Add(new TileData(x, y, "CargoRack"));
                                else if (x == 52)
                                    tiles.Add(new TileData(x, y, "Airlock"));
                                else if (x == 53)
                                    tiles.Add(new TileData(x, y, "SSolar"));
                                else if (x == 54)
                                    tiles.Add(new TileData(x, y, "CPUPanel"));
                            }
                            else if (x == 55 && y == 55)
                                tiles.Add(new TileData(x, y, "ShipCore"));

                            if (y == 50 && x == 55)
                                tiles.Add(new TileData(x, y, "MiniEngine", BuildData.BuildDataRotation.DOWN));
                        }
                    }*/
                    #endregion

                    List<TileData> hull = new List<TileData>();
                    List<TileData> floor = new List<TileData>();
                    TileData shipCore = null;
                    List<TileData> other = new List<TileData>();

                    foreach (TileData td in tiles)
                    {
                        List<TileData.Vec3> StructureParts = new List<TileData.Vec3>();
                        StructureParts.AddRange(td.StructureParts);
                        td.StructureParts = new List<TileData.Vec3>();

                        if (td.TileType == "Hull" || td.TileType == "HullCorner")
                            hull.Add(td);
                        else if (td.TileType == "Floor")
                            floor.Add(td);

                        if (td.StructureType == "ShipCore")
                            shipCore = new TileData() { X = td.X, Y = td.Y, TileType = td.StructureType, StructureParts = StructureParts };
                        else if (!String.IsNullOrEmpty(td.StructureType))
                            other.Add(new TileData() { X = td.X, Y = td.Y, TileType = td.StructureType, StructureParts = StructureParts });
                    }

                    if(shipCore == null)
                        throw new Exception("Ship Design is missing required tile ShipCore");

                    _IsImporting = true;
                    _Manager.GetComponent<ManagerMenu>().toggleMainMenu();
                    managerMenu.StartCoroutine(UpdateTiles(shipTiles, managerJobs, managerOptions, tileMap, hull, floor, shipCore, other));
                    managerMenu.StartCoroutine(FixPoweredTiles(other, tileMap.GetComponent<Structures>()));
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
                    foreach (TileData td in tiles)
                    {
                        shipTiles.tiles[td.X, td.Y].leakValue = 0;
                        shipStructures.stopLeakSource(td.X, td.Y);
                    }
                    break;
                }

                yield return new WaitForSeconds(0.01f);
            }
        }

        private System.Collections.IEnumerator FixPoweredTiles(List<TileData> tiles, Structures structures)
        {
            while(true)
            {
                if(!_IsImporting)
                {
                    foreach (TileData td in tiles)
                        structures.powerStructure((int)td.X, (int)td.Y, td.StructureType, true, "ExporterImporter.FixPoweredTiles");

                    break;
                }

                yield return new WaitForSeconds(0.01f);
            }
        }

        private System.Collections.IEnumerator UpdateTiles(Tiles shipTiles, ManagerJobs managerJobs, ManagerOptions managerOptions, GameObject tileMap, List<TileData> hull, List<TileData> floor, TileData shipCore, List<TileData> other)
        {
            bool deadCrewEndGame = true;
            if (managerOptions != null)
            {
                deadCrewEndGame = managerOptions.deadCrewEndGame;
                managerOptions.deadCrewEndGame = false;
            }
            yield return new WaitForSeconds(0.01f);
            
            Structures structures = tileMap.GetComponent<Structures>();
            
            int cargoGold = structures.structure[0].reservesGold;
            int cargoMetal = structures.structure[0].reservesMetal;
            int cargoSilicon = structures.structure[0].reservesSilicon;
            int cargoWater = structures.structure[0].reservesWater;
            int cargoFood = structures.structure[0].reservesFood;
            int cargoCredits = structures.structure[0].credits;

            List<Vector2> tilesToRemove = new List<Vector2>();

            for(int x = shipTiles.tiles.GetLowerBound(0); x <= shipTiles.tiles.GetUpperBound(0); x++)
            {
                for(int y = shipTiles.tiles.GetLowerBound(1); y <= shipTiles.tiles.GetUpperBound(1); y++)
                {
                    if (String.IsNullOrEmpty(shipTiles.tiles[x, y].toBecome) && String.IsNullOrEmpty(shipTiles.tiles[x, y].tileType) && String.IsNullOrEmpty(shipTiles.tiles[x, y].structureType))
                        continue;

                    tilesToRemove.Add(new Vector2(x, y));
                }
            }

            TileHelper.RemoveTiles(tilesToRemove);
            yield return new WaitForSeconds(0.01f);


            if (hull.Count > 0)
            {
                TileHelper.BuildTiles(hull);
                yield return new WaitForSeconds(0.01f);
            }

            if (floor.Count > 0)
            {
                TileHelper.BuildTiles(floor);
                yield return new WaitForSeconds(0.01f);
            }

            if(shipCore != null)
            {
                TileHelper.BuildTiles(shipCore);
                yield return new WaitForSeconds(0.01f);
            }

            if (other.Count > 0)
            {
                TileHelper.BuildTiles(other);
                yield return new WaitForSeconds(0.01f);
            }

            shipTiles.updateTileColors();
            shipTiles.updateTileMesh("All");

            Crew crew = tileMap.GetComponent<Crew>();
            foreach (GameObject crewMember in crew.crewList)
                crewMember.transform.position = new Vector3(shipCore.X, crewMember.transform.position.y, shipCore.Y);
            yield return new WaitForSeconds(0.01f);

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

                        /*TileData td = new TileData
                        {
                            Type = String.IsNullOrEmpty(info.tileType) ? info.toBecome : info.tileType,
                            X = (int)pos.x,
                            Y = (int)pos.y,
                            Rotation = BuildData.BuildDataRotation.UP,
                            NumParts = info.structureParts.Count
                        };*/

                        List<TileData.Vec3> StructureParts = new List<TileData.Vec3>();
                        foreach (Vector3 vec in info.structureParts)
                            StructureParts.Add(TileData.Vec3.FromVector(vec));

                        TileData td = new TileData() {
                            TileType = info.tileType,
                            X = (int)pos.x,
                            Y = (int)pos.y,
                            StructureType = info.structureType,
                            StructureParts = StructureParts
                        };

                        /*foreach(Vector3 part in info.structureParts)
                        {
                            if (part.x == pos.x && part.y == pos.y)
                                td.Rotation = (BuildData.BuildDataRotation)((int)part.z);

                            ignoreTileData.Add(new Vector2(part.x, part.y));
                        }*/

                        tiles.Add(td);

                        /*if (!String.IsNullOrEmpty(info.structureType))
                            tiles.Add(new TileData(td.X, td.Y, info.structureType, td.Rotation));*/
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

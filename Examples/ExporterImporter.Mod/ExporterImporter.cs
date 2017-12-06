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

        public override void OnInitialize()
        {
            _Manager = GameObject.Find("Manager");

            TileHelper.Init(this.ModFolder);

        }

        public override void FirstGUIPass()
        {
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
                    List<TileData> tiles = new List<TileData>();

                    for(int x = 50; x <= 60; x++)
                    {
                        for(int y = 50; y <= 60; y++)
                        {
                            if (x == 50 || x == 60 || y == 50 || y == 60)
                                tiles.Add(new TileData(x, y, "Hull"));
                            else
                                tiles.Add(new TileData(x, y, "Floor"));

                            if(y == 50)
                            {
                                if (x == 50)
                                    tiles.Add(new TileData(x, y, "CargoRack"));
                                else if (x == 51)
                                    tiles.Add(new TileData(x, y, "Airlock"));
                                else if (x == 52)
                                    tiles.Add(new TileData(x, y, "SSolar"));
                                else if (x == 53)
                                    tiles.Add(new TileData(x, y, "CPUPanel"));
                            }
                            else if(x == 55 && y == 55)
                                tiles.Add(new TileData(x, y, "ShipCore"));
                            else if(y == 50 && x == 55)
                                tiles.Add(new TileData(x, y, "MiniEngine"));
                        }
                    }

                    //Build queue: Hull, Floor, ShipCore, Structures
                    List<TileData> hull = new List<TileData>();
                    List<TileData> floor = new List<TileData>();
                    TileData shipCore = null;
                    List<TileData> other = new List<TileData>();

                    foreach (TileData td in tiles)
                    {
                        if (td.Type == "Hull")
                            hull.Add(td);
                        else if (td.Type == "Floor")
                            floor.Add(td);
                        else if (td.Type == "ShipCore")
                            shipCore = td;
                        else
                            other.Add(td);
                    }

                    if(shipCore == null)
                        throw new Exception("Ship Design is missing required tile ShipCore");

                    _IsImporting = true;
                    managerMenu.StartCoroutine(UpdateTiles(shipTiles, managerJobs, managerOptions, tileMap, hull, floor, shipCore, other));
                    managerMenu.StartCoroutine(FixPoweredTiles(other, tileMap.GetComponent<Structures>()));
                }
            }
        }

        private System.Collections.IEnumerator FixPoweredTiles(List<TileData> tiles, Structures structures)
        {
            while(true)
            {
                if(!_IsImporting)
                {
                    foreach (TileData td in tiles)
                        structures.powerStructure((int)td.X, (int)td.Y, td.Type, true, "ExporterImporter.FixPoweredTiles");

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
                managerOptions.deadCrewEndGame = false; //Make sure that the crew can't die while importing the ship.
            }
            yield return new WaitForSeconds(0.01f);
            
            Structures structures = tileMap.GetComponent<Structures>();
            
            int cargoGold = structures.structure[0].reservesGold;
            int cargoMetal = structures.structure[0].reservesMetal;
            int cargoSilicon = structures.structure[0].reservesSilicon;
            int cargoWater = structures.structure[0].reservesWater;
            int cargoFood = structures.structure[0].reservesFood;
            int cargoCredits = structures.structure[0].credits;

            Vector2[] tilesToRemove = new Vector2[structures.structure[0].allTilesList.Count];
            structures.structure[0].allTilesList.CopyTo(tilesToRemove, 0);

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

            _IsImporting = false;
        }

        private void ExportBtn_Clicked(GUI.GUIItem item)
        {
            if(_GameStarted && _Manager != null)
            {
                MouseUI mouseUI = _Manager.GetComponent<MouseUI>();

                Tiles shipTiles = mouseUI._ModGet_TileMap().GetComponent<Tiles>();
                Structures structures = mouseUI._ModGet_TileMap().GetComponent<Structures>();
                if (shipTiles != null && structures != null)
                {
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter("Design.txt", false, Encoding.UTF8))
                    {
                        for (int x = shipTiles.tiles.GetLowerBound(0); x <= shipTiles.tiles.GetUpperBound(0); x++)
                        {
                            for (int y = shipTiles.tiles.GetLowerBound(1); y <= shipTiles.tiles.GetUpperBound(1); y++)
                            {
                                if (String.IsNullOrEmpty(shipTiles.tiles[x, y].tileType) && String.IsNullOrEmpty(shipTiles.tiles[x, y].toBecome) && String.IsNullOrEmpty(shipTiles.tiles[x, y].structureType))
                                    continue;

                                TileData td = new TileData(x, y, String.IsNullOrEmpty(shipTiles.tiles[x, y].tileType) ? shipTiles.tiles[x, y].toBecome : shipTiles.tiles[x, y].tileType);

                                /*for (int sIndex = 0; sIndex < shipTiles.tiles[x, y].structureParts.Count; sIndex++)
                                    td.StructDatas.Add(new TileData.StructData(shipTiles.tiles[x, y].structureParts[sIndex].x, shipTiles.tiles[x, y].structureParts[sIndex].y, shipTiles.tiles[x, y].structureParts[sIndex].z));*/
                                
                                td.Structures.AddRange(GetStructuresAt2(structures, x, y));
                                
                                td.WriteTo(sw);
                            }
                        }
                    }
                }
            }
        }

        private void ParseDesignFile()
        {
            
        }

        private static readonly List<String> IgnoreExportingStructures = new List<string>() { "SCrate", "MCrate", "LCrate", "Floor", "Hull", "HullCorner" };

        private List<String> GetStructuresAt(Structures structures, float x, float y)
        {
            List<String> structs = new List<String>();
            foreach (StarshipTheory.ModLib.Resources.EntityCost cost in StarshipTheory.ModLib.Resources.Costs.GetEntityCosts())
            {
                if (IgnoreExportingStructures.Contains(cost.Internal_Name))
                    continue;

                foreach (Vector2 pos in structures.allStructuresOfType(cost.Internal_Name))
                {
                    //Format: X Y STRUCTURE_NAME
                    if(pos.x == x && pos.y == y)
                        structs.Add(String.Format("STRUCT {0} {1} {2}", pos.x, pos.y, cost.Internal_Name));
                }
            }

            return structs;
        }

        private List<StructureData> GetStructuresAt2(Structures structures, float x, float y)
        {
            List<StructureData> structs = new List<StructureData>();
            foreach (StarshipTheory.ModLib.Resources.EntityCost cost in StarshipTheory.ModLib.Resources.Costs.GetEntityCosts())
            {
                /*if (IgnoreExportingStructures.Contains(cost.Internal_Name))
                    continue;*/

                foreach (Vector2 pos in structures.allStructuresOfType(cost.Internal_Name))
                {
                    //Format: X Y STRUCTURE_NAME
                    if (pos.x == x && pos.y == y)
                        structs.Add(new StructureData(pos.x, pos.y, cost.Internal_Name));
                }
            }

            return structs;
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
                float x = (UnityEngine.Screen.width / 2) - 60f;
                float y = (UnityEngine.Screen.height / 2) - 60f;

                UnityEngine.GUI.Label(new Rect(x, y, 120, 120), "Importing...", new GUIStyle(UnityEngine.GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
            }

        }
    }
}

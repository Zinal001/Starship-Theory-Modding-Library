using System;
using System.Text;
using StarshipTheory.ModLib;
using GUI = StarshipTheory.ModLib.GUI;
using UnityEngine;
using System.Collections.Generic;

namespace ExporterImporter.Mod
{
    public class ExporterImporter : AbstractMod
    {
        private GameObject _Manager;

        public override string ModName => "Exporter/Importer";

        public override Version ModVersion => new Version("1.0.0");

        public override string ModDescription => "Export/Import ship design";

        private bool _GameStarted = false;

        private static Dictionary<String, BuildData> buildData;

        public override void OnInitialize()
        {
            _Manager = GameObject.Find("Manager");

            ExporterImporter.buildData = new Dictionary<string, BuildData>();

            if (System.IO.File.Exists(System.IO.Path.Combine(ModFolder, "BuildData.json")))
            {
                BuildData[] buildData;
                Debug.Log("Found BuildData.json file");
                try
                {
                    String json = System.IO.File.ReadAllText(System.IO.Path.Combine(ModFolder, "BuildData.json"));
                    buildData = Newtonsoft.Json.JsonConvert.DeserializeObject<BuildData[]>(json);

                    foreach (BuildData bd in buildData)
                        ExporterImporter.buildData[bd.Type] = bd;

                    Debug.Log("Loaded " + buildData.Length + " build data.");
                }
                catch(Exception ex)
                {
                    Debug.Log("Failed to load BuildData.json file: " + ex.Message);
                }
            }

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
                Structures structures = tileMap.GetComponent<Structures>();
                ManagerJobs managerJobs = _Manager.GetComponent<ManagerJobs>();
                ManagerMenu managerMenu = _Manager.GetComponent<ManagerMenu>();

                if(shipTiles != null && structures != null)
                {
                    List<TileData> tiles = new List<TileData>();

                    for(int x = 50; x <= 60; x++)
                    {
                        for(int y = 50; y <= 60; y++)
                        {
                            if (x == 50 || x == 60 || y == 50 || y == 60)
                                tiles.Add(new TileData(x, y, "Hull", ""));
                            else
                                tiles.Add(new TileData(x, y, "Floor", ""));

                            if(y == 50)
                            {
                                if (x == 50)
                                    tiles.Add(new TileData(x, y, "CargoRack", ""));
                                else if (x == 51)
                                    tiles.Add(new TileData(x, y, "Airlock", ""));
                                else if (x == 52)
                                    tiles.Add(new TileData(x, y, "SSolar", ""));
                                else if (x == 53)
                                    tiles.Add(new TileData(x, y, "CPUPanel", ""));
                            }
                            else if(x == 55 && y == 55)
                                tiles.Add(new TileData(x, y, "ShipCore", ""));
                            else if(y == 50 && x == 55)
                                tiles.Add(new TileData(x, y, "MiniEngine", ""));

                        }
                    }

                    for(int x = shipTiles.tiles.GetLowerBound(0); x <= shipTiles.tiles.GetUpperBound(0); x++)
                    {
                        for(int y = shipTiles.tiles.GetLowerBound(1); y <= shipTiles.tiles.GetUpperBound(1); y++)
                        {
                            if (String.IsNullOrEmpty(shipTiles.tiles[x, y].toBecome) && String.IsNullOrEmpty(shipTiles.tiles[x, y].tileType) && String.IsNullOrEmpty(shipTiles.tiles[x, y].structureType))
                                continue;

                            shipTiles.tiles[x, y].toBecome = "Remove";
                            managerJobs.completeJob(x, y, tileMap, false, false);

                            /*structures.clearSpecificTile(x, y, true, false);
                            shipTiles.clearTileGameObjects(x, y);*/
                        }
                    }

                    foreach(TileData td in tiles)
                    {
                        shipTiles.tiles[(int)td.X, (int)td.Y].toBecome = td.Type;
                        if(buildData.ContainsKey(td.Type))
                        {
                            foreach(BuildData.Part part in buildData[td.Type].Parts)
                            {
                                float rX = td.X + part.RelativeX;
                                float rY = td.Y + part.RelativeY;
                                shipTiles.tiles[(int)rX, (int)rY].toBecome = td.Type;
                                shipTiles.tiles[(int)rX, (int)rY].structureParts.Add(new Vector3(rX, rY, buildData[td.Type].Rotation));
                                managerJobs.completeJob((int)rX, (int)rY, tileMap, false, false);
                            }
                        }

                        managerJobs.completeJob((int)td.X, (int)td.Y, tileMap, false, false);
                    }

                    /*for(int x = 50; x <= 70; x++)
                    {
                        for(int y = 50; y <= 70; y++)
                        {
                            if (x == 50 || x == 70 || y == 50 || y == 70)
                                shipTiles.tiles[x, y].toBecome = "Hull";
                            else
                                shipTiles.tiles[x, y].toBecome = "Floor";

                            managerJobs.completeJob(x, y, tileMap, false, false);

                            if(y == 50)
                            {
                                if(x == 50)
                                {
                                    shipTiles.tiles[x, y].toBecome = "Airlock";
                                    shipTiles.tiles[x, y].structureParts.Add(new Vector3((float)x, (float)y, 0f));
                                    managerJobs.completeJob(x, y, tileMap, false, false);
                                }
                                else if (x == 51)
                                {
                                    shipTiles.tiles[x, y].toBecome = "CargoRack";
                                    shipTiles.tiles[x, y].structureParts.Add(new Vector3((float)x, (float)y, 0f));
                                    managerJobs.completeJob(x, y, tileMap, false, false);
                                }
                                else if (x == 52)
                                {
                                    shipTiles.tiles[x, y].toBecome = "SSolar";
                                    shipTiles.tiles[x, y].structureParts.Add(new Vector3((float)x, (float)y, 0f));
                                    managerJobs.completeJob(x, y, tileMap, false, false);
                                }
                                else if (x == 52)
                                {
                                    shipTiles.tiles[x, y].toBecome = "CPUPanel";
                                    shipTiles.tiles[x, y].structureParts.Add(new Vector3((float)x, (float)y, 0f));
                                    managerJobs.completeJob(x, y, tileMap, false, false);
                                }
                            }
                            else if(y == 70 && x == 60)
                            {
                                shipTiles.tiles[x, y].toBecome = "MiniEngine";
                                shipTiles.tiles[x, y].structureParts.Add(new Vector3((float)x, (float)y, 2f));
                                shipTiles.tiles[x, y].structureParts.Add(new Vector3((float)x, (float)(y - 1), 2f));
                                shipTiles.tiles[x, y].toBecome = "MiniEngine";
                                shipTiles.tiles[x, y - 1].structureParts.Add(new Vector3((float)x, (float)y, 2f));
                                shipTiles.tiles[x, y - 1].structureParts.Add(new Vector3((float)x, (float)(y - 1), 2f));
                                managerJobs.completeJob(x, y, tileMap, false, false);
                            }
                            else if(x == 60 && y == 60)
                            {
                                shipTiles.tiles[x, y].toBecome = "ShipCore";
                                shipTiles.tiles[x, y].structureParts.Add(new Vector3((float)x, (float)y, 0f));
                                managerJobs.completeJob(x, y, tileMap, false, false);
                            }

                        }
                    }*/

                    shipTiles.updateTileColors();

                    /*List<TileData> tiles = new List<TileData>();
                    using (System.IO.StreamReader sr = new System.IO.StreamReader("Design.txt", Encoding.UTF8))
                    {
                        while(!sr.EndOfStream)
                        {
                            TileData td = TileData.ReadFrom(sr);
                            if (td != null)
                                tiles.Add(td);
                        }
                    }

                    shipTiles.setUpDataStructure();
                    structures.setupStructureLists();

                    foreach(TileData td in tiles)
                    {
                        //shipTiles.tiles[(int)td.X, (int)td.Y].tileType = td.Type;
                        shipTiles.tiles[(int)td.X, (int)td.Y].toBecome = String.IsNullOrEmpty(td.Type) ? td.To_Become : td.Type;

                        foreach(TileData.StructData sd in td.StructDatas)
                            shipTiles.tiles[(int)td.X, (int)td.Y].structureParts.Add(new Vector3(sd.X, sd.Y, sd.Rotation));

                        managerJobs.completeJob((int)td.X, (int)td.Y, tileMap, false, false);
                    }

                    _Manager.GetComponent<Default>().loadDefaultColors(tileMap);
                    shipTiles.updateTileColors();
                    structures.updateAllStructureTextures();
                    shipTiles.updateTileMesh("All");
                    structures.updateShieldHP(0f);
                    structures.updateArmorMaxHP();
                    structures.updateArmorHP(structures.baseArmorHP);
                    structures.updateStructureMaxHP();
                    structures.updateStructureHP(structures.returnBaseStructureHP());
                    structures.updateStructureBoostText(0);*/

                }
            }
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



                                TileData td = new TileData(x, y, shipTiles.tiles[x, y].tileType, shipTiles.tiles[x, y].toBecome);

                                for (int sIndex = 0; sIndex < shipTiles.tiles[x, y].structureParts.Count; sIndex++)
                                    td.StructDatas.Add(new TileData.StructData(shipTiles.tiles[x, y].structureParts[sIndex].x, shipTiles.tiles[x, y].structureParts[sIndex].y, shipTiles.tiles[x, y].structureParts[sIndex].z));
                                
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
    }
}

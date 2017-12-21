using System;
using System.Text;
using StarshipTheory.ModLib;
using GUI = StarshipTheory.ModLib.GUI;
using Events = StarshipTheory.ModLib.Events;
using Utils = StarshipTheory.ModLib.Util;
using UnityEngine;
using System.Collections.Generic;
using StarshipTheory.ModLib.Util;

namespace ExporterImporter.Mod
{
    /// <summary>
    /// W.I.P
    /// </summary>
    public class ExporterImporter : StarshipTheory.ModLib.Mod
    {
        private GameObject _Manager;

        private bool _GameStarted = false;

        private bool _IsImporting = false;

        private TileData[] _ImportingTiles;

        private GUIStyle _loadingBoxStyle;

        public ExporterImporter()
        {
            Events.GameEvents.GameLoaded -= GameEvents_GameLoaded;
            Events.GameEvents.GameLoaded += GameEvents_GameLoaded;
            Events.GameEvents.GameStarted -= GameEvents_GameStarted;
            Events.GameEvents.GameStarted += GameEvents_GameStarted;
        }

        public override void OnInitialize()
        {
            _Manager = GameObject.Find("Manager");

        }

        public override void OnCreateGUI()
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
            _Manager = GameObject.Find("Manager");
            if (_GameStarted && _Manager != null)
            {
                String json = System.IO.File.ReadAllText(System.IO.Path.Combine(ModFolder, "Design.json"), Encoding.UTF8);

                TileData[] tiles = SerializationUtil.Deserialize<TileData[]>(json);

                _ImportingTiles = tiles;

                _IsImporting = true;
                TilesUtil.Import(GameObject.Find("TileMap"), tiles, new Action<Exception>(ImportFinished), true);
            }
        }

        private void ImportFinished(Exception error)
        {

            if (error != null)
                DisplayError(error);
            else
            {
                Tiles shipTiles = GameObject.Find("TileMap").GetComponent<Tiles>();
                Structures shipStructures = GameObject.Find("TileMap").GetComponent<Structures>();
                shipStructures.checkForLeaks = true;
                foreach (TileData td in _ImportingTiles)
                {
                    shipTiles.tiles[td.X, td.Y].leakValue = 0;
                    shipStructures.stopLeakSource(td.X, td.Y);
                    shipStructures.updateLeakBalance1(td.X, td.Y);
                }
            }

            _IsImporting = false;
            _ImportingTiles = null;
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

                        List<Vector3> StructureParts = new List<Vector3>();
                        foreach (Vector3 vec in info.structureParts)
                            StructureParts.Add(vec);

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
                                StructureParts = Helper.Copy<Vector3>(StructureParts)
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
                        String json = SerializationUtil.Serialize(tiles.ToArray());
                        System.IO.File.WriteAllText(System.IO.Path.Combine(ModFolder, "Design.json"), json, Encoding.UTF8);
                    }
                }
            }
        }

        private void GameEvents_GameStarted(object sender, EventArgs e)
        {
            _GameStarted = true;
        }

        private void GameEvents_GameLoaded(object sender, Events.GameEvents.SaveLoadEventArgs e)
        {
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

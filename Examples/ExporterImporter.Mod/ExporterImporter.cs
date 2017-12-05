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

        public override void OnInitialize()
        {
            _Manager = GameObject.Find("Manager");
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

                Tiles shipTiles = mouseUI._ModGet_TileMap().GetComponent<Tiles>();
                Structures structures = mouseUI._ModGet_TileMap().GetComponent<Structures>();

                if(shipTiles != null && structures != null)
                {
                    List<TileData> tiles = new List<TileData>();
                    using (System.IO.StreamReader sr = new System.IO.StreamReader("Export.txt", Encoding.UTF8))
                    {
                        while(!sr.EndOfStream)
                        {
                            TileData td = TileData.ReadFrom(sr);
                            if (td != null)
                                tiles.Add(td);
                        }
                    }

                    shipTiles.setUpDataStructure();

                    foreach(TileData td in tiles)
                    {
                        shipTiles.tiles[(int)td.X, (int)td.Y].tileType = td.Type;
                        shipTiles.tiles[(int)td.X, (int)td.Y].toBecome = td.To_Become;

                        foreach(TileData.StructData sd in td.StructDatas)
                            shipTiles.tiles[(int)td.X, (int)td.Y].structureParts.Add(new Vector3(sd.X, sd.Y, sd.Rotation));
                    }

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
                                if (String.IsNullOrEmpty(shipTiles.tiles[x, y].tileType) && String.IsNullOrEmpty(shipTiles.tiles[x, y].toBecome))
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

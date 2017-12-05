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

        }

        private void ExportBtn_Clicked(GUI.GUIItem item)
        {
            if(_GameStarted && _Manager != null)
            {
                MouseUI mouseUI = _Manager.GetComponent<MouseUI>();

                Tiles shipTiles = mouseUI._ModGet_TileMap().GetComponent<Tiles>();
                Structures structures = mouseUI._ModGet_TileMap().GetComponent<Structures>();
                if (shipTiles != null)
                {
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter("Export.txt", false, Encoding.UTF8))
                    {
                        for (int x = shipTiles.tiles.GetLowerBound(0); x <= shipTiles.tiles.GetUpperBound(0); x++)
                        {
                            for (int y = shipTiles.tiles.GetLowerBound(1); y <= shipTiles.tiles.GetUpperBound(1); y++)
                            {
                                if (String.IsNullOrEmpty(shipTiles.tiles[x, y].tileType) && String.IsNullOrEmpty(shipTiles.tiles[x, y].toBecome))
                                    continue;

                                //Format: X Y TYPE TO_BECOME NUM_STRUCTURE_PARTS
                                sw.WriteLine(String.Format("TILE {0} {1} {2} {3} {4}", x, y, shipTiles.tiles[x, y].tileType, shipTiles.tiles[x, y].toBecome, shipTiles.tiles[x, y].structureParts.Count));

                                if(shipTiles.tiles[x, y].structureParts.Count > 0)
                                {
                                    //Format: X Y Rotation
                                    for (int sIndex = 0; sIndex < shipTiles.tiles[x, y].structureParts.Count; sIndex++)
                                        sw.WriteLine(String.Format("DATA {0} {1} {2}", shipTiles.tiles[x, y].structureParts[sIndex].x, shipTiles.tiles[x, y].structureParts[sIndex].y, shipTiles.tiles[x, y].structureParts[sIndex].z));

                                    if(structures != null)
                                    {
                                        foreach (String structStr in GetStructuresAt(structures, (float)x, (float)y))
                                            sw.WriteLine(structStr);
                                    }

                                }
                            }
                        }
                    }
                }
            }
        }

        private void ParseDesignFile()
        {
            
        }

        private List<String> GetStructuresAt(Structures structures, float x, float y)
        {
            List<String> structs = new List<String>();
            foreach (StarshipTheory.ModLib.Resources.EntityCost cost in StarshipTheory.ModLib.Resources.Costs.GetEntityCosts())
            {
                foreach (Vector2 pos in structures.allStructuresOfType(cost.Internal_Name))
                {
                    //Format: X Y STRUCTURE_NAME
                    if(pos.x == x && pos.y == y)
                        structs.Add(String.Format("STRUCT {0} {1} {2}", pos.x, pos.y, cost.Internal_Name));
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

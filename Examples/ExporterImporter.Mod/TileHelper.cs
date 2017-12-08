using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ExporterImporter.Mod
{
    public static class TileHelper
    {
        public static void RemoveTile(Vector2 position, GameObject ship)
        {
            int X = (int)position.x;
            int Y = (int)position.y;

            Tiles shipTiles = ship.GetComponent<Tiles>();
            Structures shipStructures = ship.GetComponent<Structures>();

            TileInfo tile = shipTiles.tiles[X, Y];

            if (tile.structureType != null && tile.isPowered)
            {
                shipStructures.powerStructure(X, Y, tile.structureType, false, "TileHelper.RemoveTile");
                Debug.Log("Powering down " + tile.structureType + " (" + X + ", " + Y + ")");
            }

            if (tile.structureType == null)
                shipStructures.removeFromStructureList(X, Y, tile.tileType, "TileHelper.RemoveTile");
            else
            {
                if (tile.structureParts.Count > 0)
                {
                    Vector3 partPos = tile.structureParts[0];
                    shipStructures.removeFromStructureList((int)partPos.x, (int)partPos.y, tile.structureType, "TileHelper.RemoveTile");
                    Debug.Log("Remove structure " + tile.structureType + " (" + partPos.x + ", " + partPos.y + ")");
                }
                else
                    shipStructures.removeFromStructureList(X, Y, tile.structureType, "TileHelper.RemoveTile");
            }
            
            if(ship.name == "TileMap")
            {
                if (tile.structureParts.Count > 0)
                {
                    Vector3 partPos = tile.structureParts[0];
                    GameObject.Find("Manager").GetComponent<MouseUI>().removeStructureFromControlGroup((int)partPos.x, (int)partPos.y);
                }
                else
                    GameObject.Find("Manager").GetComponent<MouseUI>().removeStructureFromControlGroup(X, Y);
            }

            if(tile.structureType != null)
            {
                if(tile.structureParts.Count > 0)
                {
                    foreach(Vector3 pos in tile.structureParts)
                    {
                        shipTiles.tiles[(int)pos.x, (int)pos.y].structureType = null;
                        shipTiles.tiles[(int)pos.x, (int)pos.y].toBecome = null;
                    }
                }
                else
                    shipTiles.tiles[X, Y].structureType = null;
            }

            shipTiles.tiles[X, Y].structureType = null;
            shipTiles.tiles[X, Y].tileType = null;
            shipTiles.tiles[X, Y].toBecome = null;
            shipTiles.tiles[X, Y].letterValue = null;
            shipTiles.tiles[X, Y].removeJob = true;
            GameObject.Find("Manager").GetComponent<ManagerJobs>().removeJob(X, Y, ship);
            GameObject.Find("Manager").GetComponent<ManagerJobs>().removeJobSettings(X, Y, ship, true);
            shipTiles.tiles[X, Y].removeJob = false;

            shipTiles.tiles[X, Y].structureParts.Clear();

            shipTiles.updateTexture(X, Y, true, "TileHelper.RemoveTile");
            shipTiles.updateTileMesh("All");
        }

        public static void RemoveTiles(IEnumerable<Vector2> positions)
        {
            RemoveTiles(positions.ToArray());
        }

        public static void RemoveTiles(params Vector2[] positions)
        {
            GameObject tileMap = GameObject.Find("TileMap");
            Tiles shipTiles = tileMap.GetComponent<Tiles>();
            ManagerJobs managerJobs = GameObject.Find("Manager").GetComponent<ManagerJobs>();

            foreach (Vector2 pos in positions)
                RemoveTile(pos, tileMap);
        }

        public static void BuildTiles(IEnumerable<TileData> tiles)
        {
            BuildTiles(tiles.ToArray());
        }

        public static void BuildTiles(params TileData[] tiles)
        {
            GameObject tileMap = GameObject.Find("TileMap");
            Tiles shipTiles = tileMap.GetComponent<Tiles>();
            ManagerJobs managerJobs = GameObject.Find("Manager").GetComponent<ManagerJobs>();

            foreach (TileData td in tiles)
            {
                shipTiles.tiles[td.X, td.Y].toBecome = td.TileType;

                if(td.StructureParts.Count > 0)
                {
                    List<Vector3> partsList = new List<Vector3>();
                    List<Vector2> tilesList = new List<Vector2>();

                    foreach(TileData.Vec3 vec in td.StructureParts)
                    {
                        Vector2 tilePos = new Vector2(vec.X, vec.Y);
                        if (!tilesList.Contains(tilePos))
                            tilesList.Add(tilePos);

                        Vector3 partData = vec.GetVector();
                        if (!partsList.Contains(partData))
                            partsList.Add(partData);
                    }

                    if(partsList.Count > 0)
                    {
                        foreach(Vector2 tilePos in tilesList)
                        {
                            shipTiles.tiles[(int)tilePos.x, (int)tilePos.y].toBecome = td.TileType;

                            foreach (Vector3 part in partsList)
                                shipTiles.tiles[(int)tilePos.x, (int)tilePos.y].structureParts.Add(part);
                        }
                    }
                }

                managerJobs.completeJob((int)td.X, (int)td.Y, tileMap, false, false);
                shipTiles.tiles[td.X, td.Y].leakValue = 0;
            }
        }
    }
}

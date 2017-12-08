using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ExporterImporter.Mod
{
    public static class TileHelper
    {
        private static Dictionary<String, BuildData> buildData;

        public static void Init(String ModFolder)
        {
            buildData = new Dictionary<string, BuildData>();
            if (System.IO.File.Exists(System.IO.Path.Combine(ModFolder, "BuildData.json")))
            {
                Debug.Log("Found BuildData.json file");
                try
                {
                    String json = System.IO.File.ReadAllText(System.IO.Path.Combine(ModFolder, "BuildData.json"));
                    BuildData[] data = Pathfinding.Serialization.JsonFx.JsonReader.Deserialize<BuildData[]>(json);

                    foreach (BuildData bd in data)
                    {
                        buildData[bd.Type] = bd;
                        Debug.Log("Found " + bd.Type + " buil data");
                    }

                    Debug.Log("Found " + buildData.Count + " build data");
                }
                catch (Exception ex)
                {
                    Debug.Log("Failed to load BuildData.txt file: " + ex.Message);
                }
            }
        }

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
            {
                RemoveTile(pos, tileMap);

                /*shipTiles.tiles[(int)pos.x, (int)pos.y].toBecome = "Remove";
                managerJobs.completeJob((int)pos.x, (int)pos.y, tileMap, false, false);*/
            }
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
                if(!buildData.ContainsKey(td.TileType))
                {
                    Debug.LogWarning("Missing Build Data for type " + td.TileType);
                    continue;
                }

                shipTiles.tiles[td.X, td.Y].toBecome = td.TileType;

                if (buildData[td.TileType].Parts.Length > 0)
                {
                    List<Vector3> partsList = new List<Vector3>();
                    List<Vector2> tilesList = new List<Vector2>();

                    for(int i = 0; i < buildData[td.TileType].Parts.Length; i++)
                    {
                        BuildData.Part part = buildData[td.TileType].Parts[i];
                        int rX = td.X;
                        int rY = td.Y;
                        
                        if (buildData[td.TileType].AllowRotation)
                        {
                            rX += part.RelativeX;
                            rY += part.RelativeY;
                        }

                        Vector2 tilePos = new Vector2(rX, rY);
                        if(!tilesList.Contains(tilePos))
                            tilesList.Add(tilePos);
                        
                        Vector3 partData = new Vector3(rX, rY, (int)td.Rotation);

                        if(!partsList.Contains(partData))
                            partsList.Add(partData);
                    }

                    if(partsList.Count > 0)
                    {
                        String str = "Parts for " + td.TileType + "(" + td.X + ", " + td.Y + "):\n";

                        foreach (Vector2 tilePos in tilesList)
                        {
                            shipTiles.tiles[(int)tilePos.x, (int)tilePos.y].toBecome = td.TileType;

                            str += String.Format("\tPos: {0}, {1}, {2}\n", tilePos.x, tilePos.y, shipTiles.tiles[(int)tilePos.x, (int)tilePos.y].toBecome);

                            foreach (Vector3 part in partsList)
                            {
                                shipTiles.tiles[(int)tilePos.x, (int)tilePos.y].structureParts.Add(part);
                                str += String.Format("Part: {0}, {1}, {2}\n", part.x, part.y, part.z);
                            }
                        }

                        str += "\n";
                        Debug.Log(str);
                    }


                    /*for(int i = 0; i < buildData[td.TileType].Parts.Length; i++)
                    {
                        BuildData.Part part = buildData[td.TileType].Parts[i];
                        int rX = td.X;
                        int rY = td.Y;

                        if(buildData[td.TileType].AllowRotation)
                        {
                            rX += part.RelativeX;
                            rY += part.RelativeY;
                        }

                        shipTiles.tiles[rX, rY].toBecome = td.TileType;

                        for (int j = 0; j < buildData[td.TileType].Parts.Length; j++)
                        {
                            BuildData.Part part2 = buildData[td.TileType].Parts[j];
                            int dX = td.X;
                            int dY = td.Y;

                            if (buildData[td.TileType].AllowRotation)
                            {
                                dX += part2.RelativeX;
                                dY += part2.RelativeY;
                            }

                            int Rotation = 0;
                            if (td.TileType == "MiniEngine")
                                Rotation = 2;

                            shipTiles.tiles[rX, rY].structureParts.Add(new Vector3(dX, dY, Rotation));
                        }

                        String str = "StructurePart for " + td.TileType + "(" + rX + ", " + rY + ")\n";

                        foreach (Vector3 vec in shipTiles.tiles[rX, rY].structureParts)
                            str += vec.x + ", " + vec.y + ", " + vec.z + "\n";

                        str += "\n";
                        Debug.Log(str);
                    }*/

                }

                managerJobs.completeJob((int)td.X, (int)td.Y, tileMap, false, false);
                shipTiles.tiles[td.X, td.Y].leakValue = 0;
            }
        }

        private static Vector3 OffsetTile(Vector3 vect, BuildData.BuildDataRotation rotation, int length)
        {
            if (length == 0)
                return vect;

            int dX = (int)vect.x;
            int dY = (int)vect.y;

            switch(rotation)
            {
                case BuildData.BuildDataRotation.UP:
                    dY += length;
                    break;
                case BuildData.BuildDataRotation.RIGHT:
                    dX += length;
                    break;
                case BuildData.BuildDataRotation.DOWN:
                    dY -= length;
                    break;
                case BuildData.BuildDataRotation.LEFT:
                    dX -= length;
                    break;
            }

            return new Vector3(dX, dY, vect.z);

        }
        

    }
}

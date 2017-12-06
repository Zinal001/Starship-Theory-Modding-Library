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
                shipTiles.tiles[(int)pos.x, (int)pos.y].toBecome = "Remove";
                managerJobs.completeJob((int)pos.x, (int)pos.y, tileMap, false, false);
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
                if(!buildData.ContainsKey(td.Type))
                {
                    Debug.LogWarning("Missing Build Data for type " + td.Type);
                    continue;
                }

                shipTiles.tiles[(int)td.X, (int)td.Y].toBecome = td.Type;
                foreach (BuildData.Part part in buildData[td.Type].Parts)
                {
                    float rX = td.X + part.RelativeX;
                    float rY = td.Y + part.RelativeY;

                    //Add part to THIS tile
                    shipTiles.tiles[(int)td.X, (int)td.Y].structureParts.Add(new Vector3(rX, rY, buildData[td.Type].Rotation));

                    if(part.RelativeX != 0f && part.RelativeY != 0f)
                    {
                        shipTiles.tiles[(int)rX, (int)rY].toBecome = td.Type;

                        foreach(Vector3 vect in shipTiles.tiles[(int)td.X, (int)td.Y].structureParts)
                        {
                            if (vect.x == rX && vect.y == rY && vect.z == buildData[td.Type].Rotation)
                                continue;

                            shipTiles.tiles[(int)rX, (int)rY].structureParts.Add(vect);
                        }

                        shipTiles.tiles[(int)rX, (int)rY].structureParts.Add(new Vector3(rX, rY, buildData[td.Type].Rotation));
                    }
                    



                    //managerJobs.completeJob((int)rX, (int)rY, tileMap, false, false);
                }

                managerJobs.completeJob((int)td.X, (int)td.Y, tileMap, false, false);
            }
        }


    }
}

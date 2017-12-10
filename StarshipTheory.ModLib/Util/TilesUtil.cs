using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StarshipTheory.ModLib.Util
{
    public static class TilesUtil
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
                shipStructures.powerStructure(X, Y, tile.structureType, false, "TilesUtil.RemoveTile");
            }

            if (tile.structureType == null)
                shipStructures.removeFromStructureList(X, Y, tile.tileType, "TilesUtil.RemoveTile");
            else
            {
                if (tile.structureParts.Count > 0)
                {
                    Vector3 partPos = tile.structureParts[0];
                    shipStructures.removeFromStructureList((int)partPos.x, (int)partPos.y, tile.structureType, "TilesUtil.RemoveTile");
                }
                else
                    shipStructures.removeFromStructureList(X, Y, tile.structureType, "TilesUtil.RemoveTile");
            }

            if (ship.name == "TileMap")
            {
                if (tile.structureParts.Count > 0)
                {
                    Vector3 partPos = tile.structureParts[0];
                    GameObject.Find("Manager").GetComponent<MouseUI>().removeStructureFromControlGroup((int)partPos.x, (int)partPos.y);
                }
                else
                    GameObject.Find("Manager").GetComponent<MouseUI>().removeStructureFromControlGroup(X, Y);
            }

            if (tile.structureType != null)
            {
                if (tile.structureParts.Count > 0)
                {
                    foreach (Vector3 pos in tile.structureParts)
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

        public static void RemoveTiles(IEnumerable<Vector2> positions, Action<Exception> completed)
        {
            RemoveTiles(positions.ToArray(), completed);
        }

        public static void RemoveTiles(Vector2[] positions, Action<Exception> completed)
        {
            GameObject.Find("Manager").GetComponent<ManagerJobs>().StartCoroutine(_RemoveTilesCR(positions, completed));
        }

        private static System.Collections.IEnumerator _RemoveTilesCR(Vector2[] positions, Action<Exception> completed)
        {
            GameObject tileMap = GameObject.Find("TileMap");
            Tiles shipTiles = tileMap.GetComponent<Tiles>();
            ManagerJobs managerJobs = GameObject.Find("Manager").GetComponent<ManagerJobs>();

            foreach (Vector2 pos in positions)
            {
                try
                {
                    RemoveTile(pos, tileMap);
                }
                catch(Exception ex)
                {
                    completed?.Invoke(ex);
                    yield break;
                }
                yield return new WaitForRealSeconds(0.01f);
            }

            completed?.Invoke(null);
        }

        public static void BuildTile(TileData tile, GameObject ship)
        {
            Tiles shipTiles = ship.GetComponent<Tiles>();
            Structures shipStructures = ship.GetComponent<Structures>();

            shipTiles.tiles[tile.X, tile.Y].toBecome = tile.TileType;

            if (tile.StructureParts.Count > 0)
            {
                List<Vector3> partsList = new List<Vector3>();
                List<Vector2> tilesList = new List<Vector2>();

                foreach (TileData.Vec3 vec in tile.StructureParts)
                {
                    Vector2 tilePos = new Vector2(vec.X, vec.Y);
                    if (!tilesList.Contains(tilePos))
                        tilesList.Add(tilePos);

                    Vector3 partData = vec.GetVector();
                    if (!partsList.Contains(partData))
                        partsList.Add(partData);
                }

                if (partsList.Count > 0)
                {
                    foreach (Vector2 tilePos in tilesList)
                    {
                        shipTiles.tiles[(int)tilePos.x, (int)tilePos.y].toBecome = tile.TileType;

                        foreach (Vector3 part in partsList)
                            shipTiles.tiles[(int)tilePos.x, (int)tilePos.y].structureParts.Add(part);
                    }
                }
            }

            GameObject.Find("Manager").GetComponent<ManagerJobs>().completeJob(tile.X, tile.Y, ship, false, false);
            shipTiles.tiles[tile.X, tile.Y].leakValue = 0;
        }

        public static void BuildTiles(IEnumerable<TileData> tiles, Action<Exception> completed)
        {
            BuildTiles(tiles.ToArray(), completed);
        }

        public static void BuildTiles(TileData[] tiles, Action<Exception> completed)
        {
            GameObject.Find("Manager").GetComponent<ManagerJobs>().StartCoroutine(_BuildTilesCR(tiles, completed));
        }

        private static System.Collections.IEnumerator _BuildTilesCR(TileData[] tiles, Action<Exception> completed)
        {
            GameObject tileMap = GameObject.Find("TileMap");
            foreach (TileData td in tiles)
            {
                try
                {
                    BuildTile(td, tileMap);
                }
                catch(Exception ex)
                {
                    completed?.Invoke(ex);
                }
                yield return new WaitForRealSeconds(0.01f);
            }

            completed?.Invoke(null);
        }

        public static void Import(TileData[] tiles, Action<Exception> completed, bool pauseGame = true)
        {
            GameObject.Find("Manager").GetComponent<ManagerJobs>().StartCoroutine(_ImportCR(tiles, completed, pauseGame));
        }

        private static System.Collections.IEnumerator _ImportCR(TileData[] tiles, Action<Exception> completed, bool pauseGame)
        {
            GameObject tileMap = GameObject.Find("TileMap");
            Tiles shipTiles = tileMap.GetComponent<Tiles>();
            Structures shipStructures = tileMap.GetComponent<Structures>();

            if(pauseGame)
                GameObject.Find("Manager").GetComponent<ManagerOptions>().pauseGame("Pause");

            bool checkForLeaks = shipStructures.checkForLeaks;
            shipStructures.checkForLeaks = false;

            List<TileData> hull = new List<TileData>();
            List<TileData> floor = new List<TileData>();
            TileData shipCore = null;
            List<TileData> other = new List<TileData>();
            
            Vector2 crewPos = Vector2.zero;

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

            if (shipCore != null)
            {
                other.Insert(0, shipCore);
                crewPos = new Vector2(shipCore.X, shipCore.Y);
            }

            if (shipTiles != null && shipStructures != null)
            {
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

                try
                {
                    foreach (Vector2 pos in tilesToRemove)
                        RemoveTile(pos, tileMap);
                }
                catch(Exception ex)
                {
                    completed?.Invoke(ex);
                    yield break;
                }
                yield return new WaitForRealSeconds(0.01f);

                try
                {
                    foreach (TileData tile in hull)
                        BuildTile(tile, tileMap);
                }
                catch (Exception ex)
                {
                    completed?.Invoke(ex);
                    yield break;
                }
                yield return new WaitForRealSeconds(0.01f);
                
                try
                {
                    foreach (TileData tile in floor)
                        BuildTile(tile, tileMap);
                }
                catch(Exception ex)
                {
                    completed?.Invoke(ex);
                    yield break;
                }
                yield return new WaitForRealSeconds(0.01f);

                try
                {
                    foreach (TileData tile in other)
                        BuildTile(tile, tileMap);
                }
                catch (Exception ex)
                {
                    completed?.Invoke(ex);
                    yield break;
                }
                yield return new WaitForRealSeconds(0.01f);

                try
                {
                    shipTiles.updateTileColors();
                    shipTiles.updateTileMesh("All");

                    if (crewPos == Vector2.zero)
                    {
                        shipStructures.updateShield("TilesUtil.Import", true);
                        crewPos = new Vector2(shipStructures.structure[0].shipCenter.x, shipStructures.structure[0].shipCenter.z);
                    }

                    Crew crew = tileMap.GetComponent<Crew>();
                    foreach (GameObject crewMember in crew.crewList)
                        crewMember.transform.position = new Vector3((int)crewPos.x, crewMember.transform.position.y, (int)crewPos.y);
                }
                catch(Exception ex)
                {
                    completed?.Invoke(ex);
                    yield break;
                }

                yield return new WaitForRealSeconds(0.01f);

                shipStructures.checkForLeaks = checkForLeaks;

                if (pauseGame)
                    GameObject.Find("Manager").GetComponent<ManagerOptions>().pauseGame("Play");


                completed?.Invoke(null);
                yield break;
            }

            completed?.Invoke(new Exception("Class Tiles or Structures are null"));
        }
    }

    public class TileData
    {
        public int X { get; set; }
        public int Y { get; set; }
        public TileRotation Rotation { get; set; }
        public String TileType { get; set; }

        public List<Vec3> StructureParts { get; set; }

        public class Vec3
        {
            public float X { get; set; }
            public float Y { get; set; }
            public float Z { get; set; }

            public Vec3()
            {

            }

            public static Vec3 FromVector(Vector3 vec)
            {
                return new Vec3() { X = vec.x, Y = vec.y, Z = vec.z };
            }

            public Vector3 GetVector()
            {
                return new Vector3(X, Y, Z);
            }
        }


        public enum TileRotation : int
        {
            UP = 0,
            RIGHT = 1,
            DOWN = 2,
            LEFT = 3
        }
    }
}

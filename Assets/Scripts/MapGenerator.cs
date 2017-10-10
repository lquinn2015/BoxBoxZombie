using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    public Transform tilePrefab;
    public Transform obstaclePrefeb;
    public Vector2 mapSize;

    [Range(0,1)]
    public float outlinePercent;

    List<Coord> allTileCoords;
    Queue<Coord> shuffledTileCoords;

    [Range(0,1)]
    public float obstaclePercent = 0.1f;
    Coord mapCenter;

    public int seed = 3;

    public void GenerateMap()
    {
        allTileCoords = new List<Coord>();
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                allTileCoords.Add(new Coord(x, y));
            }
        }

        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), seed));
        mapCenter = new Coord((int)mapSize.x / 2,(int)mapSize.y / 2);

        string holderName = "Generated Map";
        if (transform.Find(holderName))
        {
            DestroyImmediate(GameObject.Find(holderName));
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = this.transform;

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                Vector3 tilePosition = CoordToPosition(x, y);
                Transform newTile = (Transform) Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90));
                newTile.parent = mapHolder;
                newTile.localScale = Vector3.one * outlinePercent;
            }
        }

        bool[,] obstacleMap = new bool[(int)mapSize.x, (int)mapSize.y];

        int obstacleCount = (int)(obstaclePercent*mapSize.x*mapSize.y);
        int currObstacleCount = 0;

        for (int i = 0; i < obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currObstacleCount++;

            if (!randomCoord.Equals(mapCenter) && MapIsConnected(obstacleMap, currObstacleCount))
            {
                Vector3 obstaclePos = CoordToPosition(randomCoord.x, randomCoord.y);

                Transform newObstacle = (Transform)Instantiate(obstaclePrefeb, obstaclePos + Vector3.up * 1 / 2, Quaternion.identity);
                newObstacle.parent = mapHolder;
            }
            else {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currObstacleCount--;
            }

            
            
        }

    }

    bool MapIsConnected(bool[,] map, int currentObstacleCount)
    {
        bool[,] visited = new bool[map.GetLength(0), map.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        int accessible = 1;

        queue.Enqueue(mapCenter);
        visited[mapCenter.x, mapCenter.y] = true;
        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int neighbourX = tile.x + x;
                    int neighbourY = tile.y + y;
                    if (x == 0 || y == 0)
                    {
                        if (neighbourX >= 0 && neighbourX < map.GetLength(0)
                            && neighbourY >= 0 && neighbourY < map.GetLength(1))
                        {
                            if (visited[neighbourX, neighbourY] == false && !map[neighbourX, neighbourY])
                            {
                                visited[neighbourX, neighbourY] = true;
                                queue.Enqueue(new Coord(neighbourX, neighbourY));
                                accessible++;
                            }
                        }
                    }

                }
            }

        }
        int target = (int) (mapSize.x * mapSize.y - currentObstacleCount);
        return target == accessible;
    }

    Vector3 CoordToPosition(int x, int y)
    {
        return new Vector3(-mapSize.x / 2 + .5f + x, 0, -mapSize.y / 2 + .5f + y);
    }


    public void Start()
    {
        GenerateMap();
    }

    public Coord GetRandomCoord()
    {
        Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }

    public struct Coord {
        public int x;
        public int y;

        public Coord(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public static bool operator ==(Coord c1, Coord c2)
        {
            return c1.x == c2.x && c1.y == c2.y;
        }

        public static bool operator !=(Coord c1, Coord c2)
        {
            return !(c1.x == c2.x || c1.y == c2.y);
        }


    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Map[] maps;
    public int mapIndex;
    
    public Transform tilePrefab;
    public Transform obstaclePrefab;
    public Transform navmeshFloor;
    public Vector2 maxMapSize;
    public GameObject enemy;

    [Range(0,1)]
    public float outlinePercent;

    List<Coord> allTileCoords;
    List<Coord> availableCoords;
    Queue<Coord> shuffledTileCoords;
    List<GameObject> enemies;

    public float tileSize;
    public float distanceBetweenEnemyAndPlayer = 3.5f;

    Map currentMap;

    void Start(){
        GenerateMap();
    }

    public void GenerateMap()
    {
        availableCoords = new List<Coord>();

        currentMap = maps[mapIndex];
        System.Random prng = new System.Random(currentMap.seed);
        GetComponent<BoxCollider>().size = new Vector3(currentMap.mapSize.x * tileSize, 0.5f, currentMap.mapSize.y * tileSize);

        // Generating coords
        allTileCoords = new List<Coord>();
        for(int x=0; x<currentMap.mapSize.x;x++){
            for(int y=0; y<currentMap.mapSize.y; y++){
                allTileCoords.Add(new Coord(x, y));
            }
        }
        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), currentMap.seed));

        // Creating map holder
        string holderName = "Generated Map";
        if(transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform mapHolder = new GameObject (holderName).transform;
        mapHolder.parent = transform;

        // Spawn floor tiles
        for(int x = 0; x < currentMap.mapSize.x; x ++){
            for(int y = 0; y < currentMap.mapSize.y; y ++){
                Vector3 tilePosition = CoordToPosition(x, y);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90))as Transform;
                newTile.gameObject.tag = "Tile";
                newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
                newTile.parent = mapHolder;
            }
        }

        // Spawn obstacles
        bool[,] obstacleMap = new bool[(int)currentMap.mapSize.x, (int)currentMap.mapSize.y];
        
        int obstacleCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obstaclePercent);
        int currentObstacleCount = 0;
        for(int i = 0; i < obstacleCount; i ++)
        {
            Coord randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount ++;

            if(randomCoord != currentMap.mapCenter && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                // Height setting of obstacles
                float obstacleHeight = Mathf.Lerp(currentMap.minObstacleHeight, currentMap.maxObstacleHeight, (float)prng.NextDouble());
                Vector3 obstaclePositon = CoordToPosition(randomCoord.x, randomCoord.y);            

                // Spawning
                Transform newObstacle = Instantiate(obstaclePrefab, obstaclePositon + Vector3.up * obstacleHeight / 2, Quaternion.identity) as Transform;
                newObstacle.localScale = new Vector3((1-outlinePercent) * tileSize, obstacleHeight,(1-outlinePercent) * tileSize);
                newObstacle.parent = mapHolder;
                newObstacle.gameObject.tag = "Obstacle";
                newObstacle.gameObject.layer = 11;

                //Material setting of obstacles
                Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
                Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
                float colorPercent = randomCoord.y / (float)currentMap.mapSize.y;
                obstacleMaterial.color = Color.Lerp(currentMap.foregroundColor, currentMap.backgroundColor, colorPercent);
                obstacleRenderer.sharedMaterial = obstacleMaterial;
            }
            else
            {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount --;
            }
        }
        
        // Spawn Enemies
        int enemyNum = GameObject.Find("GameManager").GetComponent<GameManager>().GetLevel() + 3;
        GameObject.Find("GameManager").GetComponent<GameManager>().SetLeftEnemiesCount(enemyNum);
        Debug.Log(enemyNum);
        RandomSpawn(obstacleMap, enemy, enemyNum);

        navmeshFloor.localScale = new Vector3(maxMapSize.x, maxMapSize.y) * tileSize;
    }

    //塗りつぶしアルゴリズム
    bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount)
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(currentMap.mapCenter);
        mapFlags[currentMap.mapCenter.x, currentMap.mapCenter.y] = true;

        int accessibleTileCount = 1;

        while(queue.Count > 0)
        {              
            Coord tile = queue.Dequeue();

            for(int x = -1; x <= 1; x ++)
            {
                for(int y = -1; y <= 1; y ++)
                {
                    int neighbourX = tile.x + x;
                    int neighbourY = tile.y + y;
                    if(x == 0 || y == 0)
                    {
                        if(neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1))
                        {
                            if(!mapFlags[neighbourX, neighbourY] && !obstacleMap[neighbourX, neighbourY])
                            {
                                mapFlags[neighbourX, neighbourY] = true;
                                queue.Enqueue(new Coord(neighbourX, neighbourY));
                                accessibleTileCount ++;
                            }
                        }
                    }
                }
            }
        }
        int targetAccessibleTileCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y - currentObstacleCount);
        return targetAccessibleTileCount == accessibleTileCount;
    }

    Vector3 CoordToPosition(int x, int y)
    {
        return new Vector3(-currentMap.mapSize.x / 2f + 0.5f + x, 0,-currentMap.mapSize.y / 2f + 0.5f + y) * tileSize;
    }

    List<Coord> GetAvailableCoords(bool[,] obstacleMap)//获取可用tile的列表
    {
        availableCoords.Clear();

        if (currentMap != null)
        {
            for (int x = 0; x < obstacleMap.GetLength(0); x++)
            {
                for (int y = 0; y < obstacleMap.GetLength(1); y++)
                {
                    if (obstacleMap[x, y] == false)
                    {
                        availableCoords.Add(new Coord(x, y));
                    }
                }
            }
            return availableCoords;
        }else
        return null;
    }

    void RandomSpawn(bool[,] obstacleMap, GameObject gameObject, int num)
    {
        Debug.Log("hhhhhhhh");
        for (int i = 0; i < num; i++)
        {
            Coord randomCoord = RandomCoord(obstacleMap);
            Vector3 randomPosition = CoordToPosition(randomCoord.x, randomCoord.y);

            // RaycastHit hit;
            // if(Physics.Raycast(randomPosition, (GameObject.Find("OVRPlayerController").transform.position - randomPosition).normalized, out hit, 50f))
            // {
            //     Debug.Log(hit.transform.name);
            //     if(hit.transform != GameObject.Find("OVRPlayerController").transform)
            //     {
            //         GameObject newObject = Instantiate(gameObject, randomPosition, Quaternion.identity);
            //     }
            // }

            // if((GameObject.Find("OVRPlayerController").transform.position - randomPosition).magnitude > 7.0f)
            // {
            //     GameObject newObject = Instantiate(gameObject, randomPosition, Quaternion.identity);
            // }
            GameObject newObject = Instantiate(gameObject, randomPosition, Quaternion.identity);
        }
    }

    //返回一个随机的可用空间并将其从list删除
    Coord RandomCoord(bool[,] obstacleMap)
    {
        availableCoords = GetAvailableCoords(obstacleMap);

        int randomIndex = Random.Range(0, availableCoords.Count);
        Coord randomPosition = availableCoords[randomIndex];
        availableCoords.RemoveAt(randomIndex);

        while((new Vector2(randomPosition.x, randomPosition.y) - new Vector2(8, 4.5f)).magnitude < distanceBetweenEnemyAndPlayer){
            randomIndex = Random.Range(0, availableCoords.Count);
            randomPosition = availableCoords[randomIndex];
            availableCoords.RemoveAt(randomIndex);
        }

        return randomPosition;
    }

    public Coord GetRandomCoord()
    {
        Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }

    [System.Serializable]
    public struct Coord
    {
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
            return !(c1 == c2);
        }
    }

    [System.Serializable]
    public class Map
    {
        public Coord mapSize;
        [Range(0, 1)]
        public float obstaclePercent;
        public int seed;
        public float minObstacleHeight;
        public float maxObstacleHeight;
        public Color foregroundColor;
        public Color backgroundColor;

        public Coord mapCenter
        {
            get
            {
                return new Coord(mapSize.x/2, mapSize.y/2);
            }
        }
    }
}

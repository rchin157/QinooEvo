using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    [SerializeField]
    private int rows = 5;
    [SerializeField]
    private int cols = 8;
    [SerializeField]
    private Camera cam;

    [SerializeField]
    private float magnificationX = 7f;   // try between 4f and 20f
    [SerializeField]
    private float magnificationZ = 7f;   // try between 4f and 20f
    [SerializeField]
    private int mapOffsetX = 0;
    [SerializeField]
    private int mapOffsetZ = 0;

    [Range(0f, 2f)]
    public float treeDensity = 1f;
    [Range(0f, 10f)]
    public float bushDensity = 1f;

    Dictionary<int, GameObject> tileset;
    Dictionary<int, GameObject> tileGroups;

    Dictionary<int, GameObject> entityset;
    Dictionary<int, GameObject> entityGroups;

    [Header("WorldGen Prefabs")]
    public GameObject pfbRavine;
    public GameObject pfbWater;
    public GameObject pfbGround;
    public GameObject pfbMountain;
    public GameObject pfbWall;
    public GameObject pfbTree;
    public GameObject pfbBush;


    List<List<int>> noiseGrid = new List<List<int>>();
    List<List<GameObject>> tileGrid = new List<List<GameObject>>();

    List<List<int>> entityGrid = new List<List<int>>();
    Dictionary<int, float[,]> entityNoiseGrids;

    float randNoiseStart;

    int levelCapLength = 15;

    // Start is called before the first frame update
    void Start()
    {
        randNoiseStart = UnityEngine.Random.Range(0f, 10000f);
        SetupScene();
        GenerateTerrain();
        GenerateEntities();
        positionCamera();
    }

    private void SetupScene()
    {
        CreateTileset();
        CreateTileGroups();
        CreateEntityDict();
        CreateEntityGroups();
    }

    private void GenerateTerrain()
    {
        InitTerrainGrid();
        GenerateStart();
        GenerateMap();
        GenerateEnd();
        CreateWalls();
    }

    private void GenerateEntities()
    {
        InitEntityNoiseGrids();
        InitEntityGrid();
        GenerateTrees();
        GenerateBushes();
    }

    private void CreateTileset()
    {
        tileset = new Dictionary<int, GameObject>();
        tileset.Add(0, pfbRavine);
        tileset.Add(1, pfbWater);
        tileset.Add(2, pfbGround);
        tileset.Add(3, pfbMountain);
    }

    private void CreateEntityDict()
    {
        entityset = new Dictionary<int, GameObject>();
        entityset.Add(0, pfbTree);
        entityset.Add(1, pfbBush);
    }

    private void CreateTileGroups()
    {
        tileGroups = new Dictionary<int, GameObject>();
        foreach (KeyValuePair<int, GameObject> prefab_pair in tileset)
        {
            GameObject tile_group = new GameObject(prefab_pair.Value.name);
            tile_group.transform.parent = transform;
            tile_group.transform.localPosition = new Vector3(0, 0, 0);
            tileGroups.Add(prefab_pair.Key, tile_group);
        }

        GameObject wallGroup = new GameObject(pfbWall.name);
        wallGroup.transform.parent = transform;
        wallGroup.transform.localPosition = new Vector3(0, 0, 0);
        tileGroups.Add(-1, wallGroup);
    }

    private void CreateEntityGroups()
    {
        entityGroups = new Dictionary<int, GameObject>();
        foreach (KeyValuePair<int, GameObject> prefabPair in entityset)
        {
            GameObject entityGroup = new GameObject(prefabPair.Value.name);
            entityGroup.transform.parent = transform;
            entityGroup.transform.localPosition = new Vector3(0, 0, 0);
            entityGroups.Add(prefabPair.Key, entityGroup);
        }
    }

    private void positionCamera()
    {
        camController camCont = cam.GetComponent<camController>();
        camCont.alignCameraToLevel(rows);
    }

    private void InitTerrainGrid()
    {
        for (int row = 0; row < rows; row++)
        {
            noiseGrid.Add(new List<int>());
            tileGrid.Add(new List<GameObject>());
        }
    }

    private void InitEntityGrid()
    {
        for (int row = 0; row < rows; row++)
        {
            entityGrid.Add(new List<int>());
            for (int col = 0; col < cols + 2 * levelCapLength; col++)
            {
                if (col < 2 * levelCapLength / 3 || col > cols + levelCapLength + levelCapLength / 3)
                    entityGrid[row].Add(-2);    // invalid
                else
                    entityGrid[row].Add(-1);    // free
            }
        }
    }

    private void InitEntityNoiseGrids()
    {
        int offsetCount = 1;
        entityNoiseGrids = new Dictionary<int, float[,]>();
        foreach (KeyValuePair<int, GameObject> prefabPair in entityset)
        {
            if (prefabPair.Key == 0)
                continue;   // skip trees
            entityNoiseGrids.Add(prefabPair.Key, new float[rows, cols + 2 * levelCapLength]);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols + 2 * levelCapLength; j++)
                {
                    entityNoiseGrids[prefabPair.Key][i, j] = GetScaledPerlin(i, j, 10f, offsetCount * 100);
                }
            }
            offsetCount++;
        }
    }

    private void GenerateMap()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = levelCapLength; col < cols + levelCapLength; col++)
            {
                int tileId = GetPerlinId(row, col);
                noiseGrid[row].Add(tileId);
                CreateTile(tileId, row, col);
            }
        }
    }

    private int GetPerlinId(int x, int z)
    {
        float upperBound = tileset.Count * 2;
        float scaledPerlin = GetScaledPerlin(x, z, upperBound, 0f);
        if (scaledPerlin < 0.25 * upperBound)
        {
            return 0; // ravine
        } else if (scaledPerlin < 0.42 * upperBound)
        {
            return 1; // water
        } else if (scaledPerlin < 0.75 * upperBound)
        {
            return 2; // ground
        } else
        {
            return 3; // mountain
        }
    }

    private float GetScaledPerlin(int x, int z, float upperBound, float noiseOffset)
    {
        float rawPerlin = Mathf.PerlinNoise(
            (x - mapOffsetX) / magnificationX + randNoiseStart + noiseOffset,
            (z - mapOffsetZ) / magnificationZ + randNoiseStart + noiseOffset
        );

        float clampedPerlin = Mathf.Clamp(rawPerlin, 0f, 1f);
        float scaledPerlin = clampedPerlin * upperBound;

        return scaledPerlin;
    }

    private void CreateTile(int tileId, int row, int col)
    {
        GameObject tilePrefab = tileset[tileId];
        GameObject tg = tileGroups[tileId];
        GameObject tile = Instantiate(tilePrefab, tg.transform);

        float posX = col + 0.5f;
        float posZ = row - 0.5f;

        tile.name = string.Format("tileRow{0}Col{1}", row, col);

        tile.transform.position = tileId switch
        {
            0 => new Vector3(posX, -0.5f, posZ),
            1 => new Vector3(posX, -0.1f, posZ),
            3 => new Vector3(posX, 1, posZ),
            _ => new Vector3(posX, 0, posZ)
        };

        tileGrid[row].Add(tile);
    }

    private void GenerateStart()
    {
        // reminder: add boat or something
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < levelCapLength; col++)
            {
                if (col < levelCapLength / 3)
                {
                    noiseGrid[row].Add(1);  // add water
                    CreateTile(1, row, col);
                } else
                {
                    noiseGrid[row].Add(2);  // add ground
                    CreateTile(2, row, col);
                }
            }
        }
    }

    private void GenerateEnd()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = cols + levelCapLength; col < cols + levelCapLength * 2; col++)
            {
                if (col < cols + levelCapLength + 2 * levelCapLength / 3)
                {
                    noiseGrid[row].Add(2);  // add ground
                    CreateTile(2, row, col);
                }
                else
                {
                    noiseGrid[row].Add(1);  // add water
                    CreateTile(1, row, col);
                }
            }
        }
        // reminder: add boat or something
    }

    private void GenerateTrees()
    {
        int row = 0, col = 0;
        foreach (List<int> noiserow in noiseGrid)
        {
            foreach (int val in noiserow)
            {
                if (entityGrid[row][col] == -1 && val == 2 && GetScaledPerlin(row, col, 2f, 0f) >= 2f - treeDensity)
                {
                    CreateEntity(0, row, col);
                }
                col++;
            }
            row++;
            col = 0;
        }
    }

    private void GenerateBushes()
    {
        int row = 0, col = 0;
        foreach (List<int> noiserow in noiseGrid)
        {
            foreach (int val in noiserow)
            {
                if (entityGrid[row][col] == -1 && val == 2 && entityNoiseGrids[1][row, col] >= 10f - bushDensity)
                {
                    CreateEntity(1, row, col);
                }
                col++;
            }
            row++;
            col = 0;
        }
    }

    private void CreateEntity(int entityId, int row, int col)
    {
        GameObject entityPrefab = entityset[entityId];
        GameObject eg = entityGroups[entityId];
        GameObject entity = Instantiate(entityPrefab, eg.transform);

        float posX = col + 0.5f;
        float posZ = row - 0.5f;

        entity.name = string.Format("entityRow{0}Col{1}", row, col);

        entity.transform.position = entityId switch
        {
            1 => new Vector3(posX, 0.5f, posZ),
            _ => new Vector3(posX, 1.2f, posZ)
        };

        entityGrid[row][col] = entityId;
    }

    private void CreateWalls()
    {
        GameObject tg = tileGroups[-1];
        float upperZ = rows - 0.5f;
        float lowerZ = -1.5f;
        for (int i = 0; i < cols + 2 * levelCapLength; i++)
        {
            GameObject wallUpper = Instantiate(pfbWall, tg.transform);
            GameObject wallLower = Instantiate(pfbWall, tg.transform);
            wallUpper.name = string.Format("UpperWall{0}", i);
            wallLower.name = string.Format("LowerWall{0}", i);
            wallUpper.transform.localPosition = new Vector3(i + 0.5f, 1, upperZ);
            wallLower.transform.localPosition = new Vector3(i + 0.5f, 1, lowerZ);
        }
    }
}

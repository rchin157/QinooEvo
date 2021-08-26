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
    private float tileSize = 1;
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

    List<List<int>> noiseGrid = new List<List<int>>();
    List<List<GameObject>> tileGrid = new List<List<GameObject>>();

    float randNoiseStart;

    int levelCapLength = 15;

    // Start is called before the first frame update
    void Start()
    {
        randNoiseStart = UnityEngine.Random.Range(0f, 10000f);
        CreateTileset();
        CreateTileGroups();
        CreateEntityDict();
        CreateEntityGroups();
        GenerateStart();
        GenerateMap();
        GenerateEnd();
        GenerateTrees();
        CreateWalls();
        positionCamera();
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

    private void GenerateMap()
    {
        for (int row = 0; row < rows; row++)
        {
            noiseGrid.Add(new List<int>());
            tileGrid.Add(new List<GameObject>());
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
        float scaledPerlin = GetScaledPerlin(x, z, upperBound);
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

    private float GetScaledPerlin(int x, int z, float upperBound)
    {
        float rawPerlin = Mathf.PerlinNoise(
            (x - mapOffsetX) / magnificationX + randNoiseStart,
            (z - mapOffsetZ) / magnificationZ + randNoiseStart
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
            noiseGrid.Add(new List<int>());
            tileGrid.Add(new List<GameObject>());
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
            noiseGrid.Add(new List<int>());
            tileGrid.Add(new List<GameObject>());
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
                if (col > 2 * levelCapLength / 3 && val == 2 && GetScaledPerlin(row, col, 2f) >= 2f - treeDensity)
                {
                    CreateEntity(0, row, col);
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
            _ => new Vector3(posX, 1.2f, posZ)
        };
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator2d : MonoBehaviour
{

    [SerializeField]
    private int rows = 5;
    [SerializeField]
    private int cols = 8;
    [SerializeField]
    private float tileSize = 1;

    // Start is called before the first frame update
    void Start()
    {
        Generate();
    }

    private void Generate()
    {
        GameObject referenceTile = (GameObject)Instantiate(Resources.Load("Tile"));
        tileSize = referenceTile.transform.localScale.x;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                GameObject tile = (GameObject)Instantiate(referenceTile, transform);

                float posX = col * tileSize;
                float posY = row * -tileSize;

                tile.transform.position = new Vector3(posX, posY, 0);
            }
        }

        Destroy(referenceTile);
    }
}

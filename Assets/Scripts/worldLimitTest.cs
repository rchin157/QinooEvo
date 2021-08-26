using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class worldLimitTest : MonoBehaviour
{

    private float xpos = 1;
    private float elapsedTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (elapsedTime > 1)
        {
            transform.position = new Vector3(xpos *= 2, 0, 0);
            Debug.Log(transform.position);
        }
        elapsedTime += Time.deltaTime;
    }
}

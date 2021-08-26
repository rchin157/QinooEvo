using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pushable : MonoBehaviour
{

    private GameObject captor = null;

    public Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setCaptor(GameObject captorObj)
    {
        captor = captorObj;
    }

    public GameObject getCaptor()
    {
        return captor;
    }

    public void movePushable(Vector3 movement)
    {
        rb.MovePosition(transform.position + movement);
    }

    private void OnTriggerEnter(Collider hit)
    {
        Debug.Log("trigger entered");
        GameObject other = hit.gameObject;
        Tile tile = other.GetComponent<Tile>();
        Pushable pushable = other.GetComponent<Pushable>();
        Placeable placeable = other.GetComponent<Placeable>();

        Pusher pusher = captor.GetComponent<Pusher>();

        if (tile == null && pushable == null && placeable == null)
        {
            
            return;
        }
    }
}

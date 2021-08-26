using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pusher : MonoBehaviour
{

    private List<GameObject> capturedPushables = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void addPushable(GameObject pushable)
    {
        capturedPushables.Add(pushable);
        Pushable ps = pushable.GetComponent<Pushable>();
        ps.setCaptor(gameObject);
    }

    public void removePushable(GameObject pushable)
    {
        capturedPushables.Remove(pushable);
        Pushable ps = pushable.GetComponent<Pushable>();
        ps.setCaptor(null);
    }

    public void clearPushables()
    {
        foreach (GameObject pushable in capturedPushables)
        {
            Pushable ps = pushable.GetComponent<Pushable>();
            ps.setCaptor(null);
        }
        capturedPushables.Clear();
    }

    public void moveCaptured(Vector3 movement)
    {
        foreach (GameObject pushable in capturedPushables)
        {
            Pushable ps = pushable.GetComponent<Pushable>();
            ps.movePushable(movement);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camController : MonoBehaviour
{

    [SerializeField]
    private Camera cam;
    [SerializeField]
    private float scrollSpeed = 0.3f;

    public void alignCameraToLevel(int rows)
    {
        float fov = Mathf.Atan2((float)(rows + 4), 10f) * Mathf.Rad2Deg;
        cam.fieldOfView = fov;
        float rot = 90f - (fov / 2f);
        Vector3 newEulerangles = new Vector3(rot, transform.eulerAngles.y, transform.eulerAngles.z);
        transform.eulerAngles = newEulerangles;
        transform.position = new Vector3(rows / 2, 10, -2);
    }

    private void FixedUpdate()
    {
        transform.position += new Vector3(scrollSpeed * Time.deltaTime, 0, 0);
    }
}

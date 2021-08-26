using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class charControllerScript : MonoBehaviour
{

    [Header("Input Settings")]
    public PlayerInput playerInput;
    public float movementSpeed = 3f;
    public float pushPower = 2.0f;
    private Vector3 rawInput;

    [Header("Comp References")]
    public CharacterController cc;
    //public Pusher pusher;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 movement = rawInput * movementSpeed * Time.deltaTime;
        cc.Move(movement);
        //pusher.moveCaptured(movement);
        //Debug.Log(cc.velocity);
    }

    public void OnMovement(InputAction.CallbackContext value)
    {
        Vector2 inputMovement = value.ReadValue<Vector2>();
        rawInput = new Vector3(inputMovement.x, 0, inputMovement.y);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //Rigidbody rb = hit.collider.attachedRigidbody;
        //GameObject otherObject = hit.gameObject;

        //// non rigid bodies
        //if (rb == null)
        //{
        //    return;
        //}

        //if (hit.moveDirection.y < -0.3)
        //{
        //    return;
        //}

        //float vx = hit.moveDirection.x;
        //float vz = hit.moveDirection.z;

        //float distx = Mathf.Abs(otherObject.transform.position.x - transform.position.x);
        //float distz = Mathf.Abs(otherObject.transform.position.z - transform.position.z);

        //if (distz <= distx)
        //    vz = 0.0f;
        //else
        //    vx = 0.0f;

        //Vector3 pushDir = new Vector3(vx, 0, vz);

        
    }
}

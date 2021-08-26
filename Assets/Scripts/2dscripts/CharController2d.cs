using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharController2d : MonoBehaviour
{

    [Header("Component Reference")]
    public Rigidbody2D playerRB;

    [Header("Input Settings")]
    public PlayerInput playerInput;
    public float movementSpeed = 3f;
    private Vector3 rawInput;
    private Vector3 smoothInputMovement;

    private Vector2 velocity;
    private Vector2 lastPosition;

    private int pushAxis = -1;   // 0 = x axis, 1 = y axis, -1 = free

    // Start is called before the first frame update
    void Start()
    {
        lastPosition = new Vector2(transform.position.x, transform.position.y);
    }

    void FixedUpdate()
    {
        Vector2 currentPosition = new Vector2(transform.position.x, transform.position.y);
        velocity = (lastPosition - currentPosition) / Time.deltaTime;
        lastPosition = currentPosition;
        Debug.Log(velocity);

        float currentx = Mathf.Abs(velocity.x);
        float currenty = Mathf.Abs(velocity.y);

        if (pushAxis == -1)
        {
            if (currentx > currenty)
                pushAxis = 0;
            else if (currenty > currentx)
                pushAxis = 1;
        } else if (pushAxis == 0)
        {
            if (currentx == 0.0f)
                pushAxis = -1;
        } else if (pushAxis == 1)
        {
            if (currenty == 0.0f)
                pushAxis = -1;
        }

        if (currentx != 0 && currenty != 0)
            pushAxis = -1;

        float correction = 1.0f;
        if (pushAxis == -1)
            correction = 1.0f;
        else if (pushAxis == 0)
            correction = movementSpeed / Mathf.Clamp(currentx, 1f, movementSpeed);
        else if (pushAxis == 1)
            correction = movementSpeed / Mathf.Clamp(currenty, 1f, movementSpeed);
        
        Vector3 movement = rawInput * movementSpeed * Time.deltaTime;
        //playerRB.MovePosition(transform.position + movement);
        playerRB.AddForce(movement);
    }

    public void OnMovement(InputAction.CallbackContext value)
    {
        Vector2 inputMovement = value.ReadValue<Vector2>();
        rawInput = new Vector3(inputMovement.x, inputMovement.y, 0);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{

    [Header("Component Reference")]
    public Rigidbody playerRB;

    [Header("Input Settings")]
    public PlayerInput playerInput;
    public float movementSpeed = 3f;
    private Vector3 rawInput;
    private Vector3 smoothInputMovement;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 movement = rawInput * movementSpeed * Time.deltaTime;
        playerRB.MovePosition(transform.position + movement);
    }

    public void OnMovement(InputAction.CallbackContext value)
    {
        Vector2 inputMovement = value.ReadValue<Vector2>();
        rawInput = new Vector3(inputMovement.x, 0, inputMovement.y);
    }
}

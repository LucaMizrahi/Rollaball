using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Speed of the player
    public float speed = 0;

    // Rigidbody component
    private Rigidbody rb;

    // Movement variables along X and Y axis
    private float movementX;
    private float movementY;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Functon to move the player when a move input is detected
    void OnMove(InputValue value)
    {
        // Get the movement vector from the input value
        Vector2 movementVector = value.Get<Vector2>();

        // Store the X and Y components of the movement vector
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    void FixedUpdate() 
    {
        // Create a 3D movement vector using X and Y inputs
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);

        // Apply the movement vector to the player
        rb.AddForce(movement*speed);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
        }
    }
}

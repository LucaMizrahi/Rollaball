using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    // Speed of the player
    public float speed = 0;

    // Text component
    public TextMeshProUGUI scoreText;

    // Win text component
    public GameObject winTextObject;

    // Rigidbody component
    private Rigidbody rb;

    // Score variable
    private int score;

    // Movement variables along X and Y axis
    private float movementX;
    private float movementY;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        score = 0;

        // Set the score text to 0
        SetScoreText();

        // Set the win text to inactive
        winTextObject.SetActive(false);
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

    // Function to update the score text
    void SetScoreText()
    {
        scoreText.text = "Score: " + score.ToString();

        // If the score is 12 or more, display the win text
        if (score >= 12)
        {
            winTextObject.SetActive(true);
        }
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
            score = score + 1;

            SetScoreText();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5.0f;
    public float rotationSpeed = 100.0f;
    public float jumpForce = 5.0f;
    public float gravity = 20.0f;
    
    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    
    void Start()
    {
        // Get or add a CharacterController
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            controller = gameObject.AddComponent<CharacterController>();
        }
        
        // Create ground check if it doesn't exist
        if (groundCheck == null)
        {
            GameObject check = new GameObject("GroundCheck");
            check.transform.parent = transform;
            check.transform.localPosition = new Vector3(0, -0.9f, 0); // Slightly below the player
            groundCheck = check.transform;
            Debug.Log("Created ground check object");
        }
        
        // Make sure player has the Player tag
        gameObject.tag = "Player";
        
        Debug.Log("SimplePlayerMovement initialized");
    }
    
    void Update()
    {
        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small downward force when grounded
        }
        
        // Get input
        float horizontal = Input.GetAxis("Horizontal"); // A and D keys
        float vertical = Input.GetAxis("Vertical");     // W and S keys
        
        // Calculate movement direction (relative to the player's orientation)
        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        
        // Move the player
        controller.Move(move * moveSpeed * Time.deltaTime);
        
        // Jumping
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * 2f * gravity);
            Debug.Log("Jump!");
        }
        
        // Apply gravity
        velocity.y -= gravity * Time.deltaTime;
        
        // Move the player vertically (for jumping and gravity)
        controller.Move(velocity * Time.deltaTime);
        
        // Debug movement
        if (move.magnitude > 0.1f)
        {
            Debug.Log("Moving: " + move);
        }
    }
    
    // Draw gizmos for debugging
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}
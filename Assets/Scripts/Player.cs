using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Properties")]
    public int health = 100;
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    
    [Header("Combat")]
    public int damage = 20;
    public float attackRange = 2f;
    public float attackCooldown = 1f;
    public LayerMask enemyLayerMask;
    
    private float attackTimer = 0f;
    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private float gravity = 20f;
    
    void Start()
    {
        // Get or add a CharacterController
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            characterController = gameObject.AddComponent<CharacterController>();
            characterController.center = new Vector3(0, 1, 0);
            characterController.height = 2f;
            characterController.radius = 0.5f;
        }
        
        // Make sure the player has the "Player" tag
        gameObject.tag = "Player";
    }
    
    void Update()
    {
        // Cooldown timer
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
        
        // Movement
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        // Calculate movement direction in world space
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        
        // Project vectors onto the horizontal plane
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();
        
        // Calculate the move direction relative to the camera
        moveDirection = (forward * vertical + right * horizontal).normalized;
        
        // Apply movement speed
        moveDirection *= moveSpeed;
        
        // Apply gravity
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        else
        {
            moveDirection.y = -0.5f; // Small downward force when grounded
        }
        
        // Move the character
        characterController.Move(moveDirection * Time.deltaTime);
        
        // Rotate the character to face the movement direction
        if (moveDirection.x != 0 || moveDirection.z != 0)
        {
            Vector3 lookDirection = new Vector3(moveDirection.x, 0, moveDirection.z);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), rotationSpeed * Time.deltaTime);
        }
        
        // Attack input
        if (Input.GetMouseButtonDown(0) && attackTimer <= 0)
        {
            Attack();
        }
    }
    
    void Attack()
    {
        Debug.Log("Player attacked!");
        
        // Check for enemies in attack range
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + transform.forward, attackRange, enemyLayerMask);
        
        foreach (Collider hitCollider in hitColliders)
        {
            Enemy enemy = hitCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log("Hit enemy for " + damage + " damage");
            }
        }
        
        // Set cooldown
        attackTimer = attackCooldown;
    }
    
    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Player took " + damage + " damage. Health: " + health);
        
        if (health <= 0)
        {
            Die();
        }
    }
    
    void Die()
    {
        Debug.Log("Player defeated!");
        // You could respawn the player or show a game over screen
        // For now, just disable controls
        this.enabled = false;
    }
    
    // Draw the attack range in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward, attackRange);
    }
}
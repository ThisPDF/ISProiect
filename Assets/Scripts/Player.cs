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
    public float attackCooldown = 1f;
    public float projectileForce = 20f;
    public GameObject projectilePrefab;

    private float attackTimer = 0f;
    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private float gravity = 20f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            characterController = gameObject.AddComponent<CharacterController>();
            characterController.center = new Vector3(0, 1, 0);
            characterController.height = 2f;
            characterController.radius = 0.5f;
        }

        gameObject.tag = "Player";
    }

    void Update()
    {
        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        moveDirection = (forward * vertical + right * horizontal).normalized * moveSpeed;

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        else
        {
            moveDirection.y = -0.5f;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        if (moveDirection.x != 0 || moveDirection.z != 0)
        {
            Vector3 lookDirection = new Vector3(moveDirection.x, 0, moveDirection.z);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), rotationSpeed * Time.deltaTime);
        }

        if (Input.GetMouseButtonDown(0) && attackTimer <= 0)
        {
            Attack();
        }
    }

    void Attack()
    {
        Debug.Log("Player fired projectile!");

        if (projectilePrefab != null)
        {
            Vector3 spawnPos = transform.position + transform.forward + Vector3.up * 1f;
            GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(transform.forward * projectileForce, ForceMode.Impulse);
            }

            // Setează damage pe proiectil
            Projectile projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.damage = damage;
            }
        }

        attackTimer = attackCooldown;
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        Debug.Log("Player took " + amount + " damage. Health: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player defeated!");
        this.enabled = false;
    }
}

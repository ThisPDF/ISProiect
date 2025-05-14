using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Properties")]
    public int health = 50;
    public int damage = 10;
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float playerDetectionRange = 10f;
    public float switchTargetCooldown = 5f;
    
    private Transform currentTarget;
    private float attackTimer = 0f;
    private float targetSwitchTimer = 0f;
    private NavMeshAgent agent;
    
    void Start()
    {
        // Get NavMeshAgent component
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = moveSpeed;
        }
        
        // Default target is the castle
        GameObject castle = GameObject.FindGameObjectWithTag("Castle");
        if (castle != null)
        {
            currentTarget = castle.transform;
        }
    }
    
    void Update()
    {
        // Cooldown timers
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
        
        if (targetSwitchTimer > 0)
        {
            targetSwitchTimer -= Time.deltaTime;
        }
        else
        {
            // Time to potentially switch targets
            FindTarget();
            targetSwitchTimer = switchTargetCooldown;
        }
        
        // Move towards target if we have one
        if (currentTarget != null)
        {
            if (agent != null && agent.isActiveAndEnabled)
            {
                agent.SetDestination(currentTarget.position);
            }
            else
            {
                // Manual movement if no NavMeshAgent
                Vector3 direction = (currentTarget.position - transform.position).normalized;
                transform.position += direction * moveSpeed * Time.deltaTime;
                transform.LookAt(new Vector3(currentTarget.position.x, transform.position.y, currentTarget.position.z));
            }
            
            // Check if in attack range
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
            if (distanceToTarget <= attackRange && attackTimer <= 0)
            {
                Attack();
            }
        }
    }
    
    void FindTarget()
    {
        // Default to castle if no players are nearby
        GameObject castle = GameObject.FindGameObjectWithTag("Castle");
        Transform bestTarget = castle != null ? castle.transform : null;
        
        // Look for players in detection range
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float closestDistance = playerDetectionRange;
        
        foreach (GameObject player in players)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                bestTarget = player.transform;
            }
        }
        
        // Set the target
        currentTarget = bestTarget;
    }
    
    void Attack()
    {
        // Deal damage to the target
        if (currentTarget.CompareTag("Player"))
        {
            Player player = currentTarget.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage);
                Debug.Log("Enemy attacked player for " + damage + " damage");
            }
        }
        else if (currentTarget.CompareTag("Castle"))
        {
            Castle castle = currentTarget.GetComponent<Castle>();
            if (castle != null)
            {
                castle.TakeDamage(damage);
                Debug.Log("Enemy attacked castle for " + damage + " damage");
            }
        }
        
        // Set cooldown
        attackTimer = attackCooldown;
    }
    
    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Enemy took " + damage + " damage. Health: " + health);
        
        if (health <= 0)
        {
            Die();
        }
        else
        {
            // When hit, target the attacker if it's a player
            // This makes the enemy aggressive towards players who attack it
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5f);
            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Player"))
                {
                    currentTarget = hitCollider.transform;
                    break;
                }
            }
        }
    }
    
    void Die()
    {
        Debug.Log("Enemy defeated!");
        Destroy(gameObject);
    }
}
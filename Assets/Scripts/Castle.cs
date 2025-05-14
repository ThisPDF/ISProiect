using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Castle : MonoBehaviour
{
    [Header("Castle Properties")]
    public int maxHealth = 1000;
    public int currentHealth;
    
    void Start()
    {
        currentHealth = maxHealth;
        
        // Make sure the castle has the "Castle" tag
        gameObject.tag = "Castle";
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Castle took " + damage + " damage. Health: " + currentHealth + "/" + maxHealth);
        
        if (currentHealth <= 0)
        {
            GameOver();
        }
    }
    
    void GameOver()
    {
        Debug.Log("Castle has fallen! Game Over!");
        // You could show a game over screen here
    }
}
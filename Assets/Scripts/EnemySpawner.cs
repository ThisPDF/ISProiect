using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public int numberOfEnemies;
        public GameObject[] enemyPrefabs;
        public float spawnInterval = 2f;
        public int pointsPerEnemy = 10;
    }

    [Header("Spawn Settings")]
    public Wave[] waves;
    public Transform[] spawnPoints;
    public float minDistanceFromPlayer = 10f;
    public bool randomizeSpawnPoints = true;
    
    [Header("Wave Settings")]
    public float timeBetweenWaves = 5f;
    public bool autoStartNextWave = true;
    public bool infiniteWaves = false;
    
    [Header("References")]
    public GameObject castle;
    
    private int currentWaveIndex = 0;
    private int enemiesRemainingToSpawn;
    private int enemiesRemainingAlive;
    private float nextSpawnTime;
    private bool isSpawning = false;
    
    void Start()
    {
        // Find the castle if not assigned
        if (castle == null)
        {
            castle = GameObject.FindGameObjectWithTag("Castle");
        }
        
        // Find spawn points if not assigned
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points assigned to EnemySpawner. Using spawner position.");
            spawnPoints = new Transform[1];
            spawnPoints[0] = transform;
        }
        
        // Start the first wave
        StartNextWave();
    }
    
    void Update()
    {
        if (!isSpawning) return;
        
        // Check if it's time to spawn the next enemy
        if (Time.time >= nextSpawnTime && enemiesRemainingToSpawn > 0)
        {
            SpawnEnemy();
            enemiesRemainingToSpawn--;
            
            // Set the time for the next spawn
            if (currentWaveIndex < waves.Length)
            {
                nextSpawnTime = Time.time + waves[currentWaveIndex].spawnInterval;
            }
        }
        
        // Check if the wave is complete
        if (enemiesRemainingToSpawn == 0 && enemiesRemainingAlive == 0)
        {
            EndWave();
            
            // Start the next wave if auto-start is enabled
            if (autoStartNextWave)
            {
                Invoke("StartNextWave", timeBetweenWaves);
            }
        }
    }
    
    void StartNextWave()
    {
        // Check if there are more waves
        if (currentWaveIndex < waves.Length || infiniteWaves)
        {
            // If using infinite waves, loop back to the first wave
            if (infiniteWaves && currentWaveIndex >= waves.Length)
            {
                currentWaveIndex = 0;
            }
            
            // Set up the wave
            Wave currentWave = waves[currentWaveIndex];
            enemiesRemainingToSpawn = currentWave.numberOfEnemies;
            enemiesRemainingAlive = currentWave.numberOfEnemies;
            nextSpawnTime = Time.time; // Spawn first enemy immediately
            
            Debug.Log("Starting Wave: " + (currentWaveIndex + 1) + " - " + currentWave.waveName);
            
            isSpawning = true;
        }
        else
        {
            // All waves completed
            Debug.Log("All waves completed!");
        }
    }
    
    void SpawnEnemy()
    {
        if (currentWaveIndex >= waves.Length) return;
        
        Wave currentWave = waves[currentWaveIndex];
        
        // Select a random enemy prefab from the current wave
        GameObject enemyPrefab = currentWave.enemyPrefabs[Random.Range(0, currentWave.enemyPrefabs.Length)];
        
        // Select a spawn point
        Transform spawnPoint = GetSpawnPoint();
        
        // Spawn the enemy
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        
        // Set up the enemy
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null && castle != null)
        {
            // Randomize enemy stats slightly for variety
            enemyScript.health = Mathf.RoundToInt(enemyScript.health * Random.Range(0.8f, 1.2f));
            enemyScript.damage = Mathf.RoundToInt(enemyScript.damage * Random.Range(0.9f, 1.1f));
            enemyScript.moveSpeed = enemyScript.moveSpeed * Random.Range(0.9f, 1.1f);
            
            // Add a listener for when the enemy dies
            StartCoroutine(CheckEnemyAlive(enemy));
        }
        
        Debug.Log("Spawned enemy at " + spawnPoint.position);
    }
    
    IEnumerator CheckEnemyAlive(GameObject enemy)
    {
        // Wait until the enemy is destroyed
        while (enemy != null)
        {
            yield return new WaitForSeconds(0.5f);
        }
        
        // Enemy was destroyed
        enemiesRemainingAlive--;
    }
    
    Transform GetSpawnPoint()
    {
        if (!randomizeSpawnPoints || spawnPoints.Length == 1)
        {
            return spawnPoints[0];
        }
        
        // Try to find a spawn point that's far enough from all players
        for (int attempts = 0; attempts < 5; attempts++)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            bool isFarEnough = true;
            
            // Check distance from all players
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                if (Vector3.Distance(spawnPoint.position, player.transform.position) < minDistanceFromPlayer)
                {
                    isFarEnough = false;
                    break;
                }
            }
            
            if (isFarEnough)
            {
                return spawnPoint;
            }
        }
        
        // If we couldn't find a good spawn point, just pick a random one
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }
    
    void EndWave()
    {
        isSpawning = false;
        currentWaveIndex++;
        Debug.Log("Wave completed!");
    }
    
    // Public method to manually start the next wave
    public void ManualStartNextWave()
    {
        if (!isSpawning)
        {
            StartNextWave();
        }
    }
    
    // Public method to get the current wave number (for UI)
    public int GetCurrentWaveNumber()
    {
        return currentWaveIndex + 1;
    }
    
    // Public method to get the total number of waves
    public int GetTotalWaveCount()
    {
        return waves.Length;
    }
    
    // Public method to check if spawning is active
    public bool IsSpawning()
    {
        return isSpawning;
    }
    
    // Public method to get enemies remaining in current wave
    public int GetEnemiesRemaining()
    {
        return enemiesRemainingAlive;
    }
}
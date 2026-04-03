using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public Transform player;                // Reference to the player
    public GameObject enemyPrefab;          // Enemy prefab to spawn
    public int maxEnemiesOnScene = 10;      // Max number of enemies alive
    public float spawnRadius = 10f;         // Distance from player to spawn enemies

    [Header("Wave Settings")]
    public int enemiesPerWave = 3;          // How many enemies spawn per wave
    public float waveCooldown = 5f;         // Time between waves
    private float nextWaveTime;

    private int currentEnemies = 0;
    private PlayerHealth playerHealth;

    private void Start()
    {
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }

        nextWaveTime = Time.time + 2f; // initial delay before first wave
    }

    private void Update()
    {
        // Stop spawning if player is dead
        if (playerHealth != null && playerHealth.isDead) return;

        // Spawn waves
        if (Time.time >= nextWaveTime && currentEnemies < maxEnemiesOnScene)
        {
            SpawnWave();
            nextWaveTime = Time.time + waveCooldown;
        }
    }

    void SpawnWave()
    {
        int spawnCount = Mathf.Min(enemiesPerWave, maxEnemiesOnScene - currentEnemies);

        for (int i = 0; i < spawnCount; i++)
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        if (player == null || enemyPrefab == null) return;

        // Random position around player
        Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
        randomOffset.y = 0f; // Keep on the ground

        Vector3 spawnPosition = player.position + randomOffset;

        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        // Assign player to enemy
        EnemyFollow enemyScript = enemy.GetComponent<EnemyFollow>();
        if (enemyScript != null)
            enemyScript.player = player;

        currentEnemies++;

        // Track enemy death to decrement count
        EnemyTracker tracker = enemy.AddComponent<EnemyTracker>();
        tracker.spawner = this;
    }

    public void EnemyDied()
    {
        currentEnemies--;
    }
}
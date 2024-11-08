using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<GameObject> enemyPrefabs; // List of enemy prefabs to spawn
    public Transform spawnPoint; // Location where new enemies will spawn
    public Combatant player; // Reference to the player combatant

    private int currentEnemyIndex = 0; // Track which enemy is next
    private GameObject currentEnemyInstance; // The currently active enemy

    void Start()
    {
        SpawnNextEnemy();
    }

    public void SpawnNextEnemy()
    {
        // Check if we still have enemies left to spawn
        if (currentEnemyIndex < enemyPrefabs.Count)
        {
            // Instantiate the next enemy at the spawn point
            currentEnemyInstance = Instantiate(enemyPrefabs[currentEnemyIndex], spawnPoint.position, Quaternion.identity);
            Combatant enemyCombatant = currentEnemyInstance.GetComponent<Combatant>();

            // Set the player's opponent to this new enemy
            player.opponent = enemyCombatant;

            // Set the enemy's opponent to the player
            enemyCombatant.opponent = player;

            // Register the enemy manager so the enemy can notify it on death
            enemyCombatant.enemyManager = this;

            // Move to the next enemy in the list for future spawns
            currentEnemyIndex++;
        }
        else
        {
            Debug.Log("All enemies defeated!");
            // Clear the player's opponent to stop attacking
            player.opponent = null;
        }
    }

    public void EnemyDied()
    {
        // Clear the reference to the dead enemy instance
        currentEnemyInstance = null;

        // Try to spawn the next enemy
        SpawnNextEnemy();
    }
}
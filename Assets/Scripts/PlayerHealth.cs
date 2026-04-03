using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    public int currentHealth;

    [Header("Face Meshes")]
    public MeshFilter playerMeshFilter; // Drag your player's Mesh Filter here in the Inspector
    public Mesh face3Hearts;            // Drag your Super Happy face mesh here
    public Mesh face2Hearts;            // Drag a Neutral face mesh here
    public Mesh face1Heart;             // Drag a Sad/Hurt face mesh here

    void Start()
    {
        // Start the game with full health
        currentHealth = maxHealth;
        UpdateFace();
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        
        // Prevent health from dropping below 0
        if (currentHealth < 0) 
        {
            currentHealth = 0;
        }

        Debug.Log("Player took damage! Current health: " + currentHealth);
        UpdateFace();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateFace()
    {
        // Swap the 3D mesh based on current health
        if (playerMeshFilter != null)
        {
            if (currentHealth == 3) playerMeshFilter.mesh = face3Hearts;
            else if (currentHealth == 2) playerMeshFilter.mesh = face2Hearts;
            else if (currentHealth == 1) playerMeshFilter.mesh = face1Heart;
        }
    }

    void Die()
    {
        Debug.Log("Player has run out of hearts!");
        // Add your game over logic here later (like reloading the scene)
    }
}
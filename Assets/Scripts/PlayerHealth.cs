using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    private int currentHealth;
    public bool isDead = false;

    [Header("Face Meshes")]
    public MeshFilter playerMeshFilter;
    public Mesh face3Hearts;
    public Mesh face2Hearts;
    public Mesh face1Heart;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateFace(); // Set the happy face right away
    }

    // Notice this now takes an 'int' instead of a 'float'
    public void TakeDamage(int amount) 
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log("Player took damage: " + amount + " | Health: " + currentHealth);

        UpdateFace(); // Change the face whenever we get hurt

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateFace()
    {
        if (playerMeshFilter != null)
        {
            if (currentHealth >= 3) playerMeshFilter.mesh = face3Hearts;
            else if (currentHealth == 2) playerMeshFilter.mesh = face2Hearts;
            else if (currentHealth == 1) playerMeshFilter.mesh = face1Heart;
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("Player Died! Game Over.");

        // --- Your teammate's death logic below ---

        // Disable shooting and movement scripts
        var shoot = GetComponent<PlayerAutoShoot>();
        if (shoot != null) shoot.enabled = false;

        var controller = GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;

        // Call GameManager to handle scene reload
        GameManager.Instance.GameOver(1f);

        // Destroy player so it doesn't block UI
        Destroy(gameObject);
    }
}
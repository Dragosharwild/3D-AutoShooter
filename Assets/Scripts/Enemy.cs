using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    public Transform playerTarget; // Drag your Player object here in the Inspector
    public float moveSpeed = 3f;
    public int damageToGive = 1;

    void Update()
    {
        // If the player exists, move towards them
        if (playerTarget != null)
        {
            // Calculate the step size based on speed and time
            float step = moveSpeed * Time.deltaTime;
            
            // Move the enemy's position towards the player's position
            transform.position = Vector3.MoveTowards(transform.position, playerTarget.position, step);
            
            // Make the enemy face the player
            transform.LookAt(playerTarget);
        }
    }

    // This triggers when the Enemy's collider hits another collider
    void OnCollisionEnter(Collision collision)
    {
        // Check if the object we hit has the tag "Player"
        if (collision.gameObject.CompareTag("Player"))
        {
            // Try to find the PlayerHealth script on the object we hit
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            
            if (playerHealth != null)
            {
                // Deal the damage
                playerHealth.TakeDamage(damageToGive);
                
                // Destroy the enemy so it doesn't instantly drain all 3 hearts in one second!
                Destroy(gameObject); 
            }
        }
    }
}
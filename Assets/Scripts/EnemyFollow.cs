using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public Transform player;
    public float speed = 3f;
    
    // Changed to int, taking 1 heart per hit
    public int damage = 1; 

    public float attackCooldown = 1.5f;
    private float lastAttackTime;

    private PlayerHealth playerHealth;

    private void Start()
    {
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }
    }

    private void Update()
    {
        if (player == null || playerHealth == null) return;

        // Stop if player is dead
        if (playerHealth.isDead) return;

        // Move toward player
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        transform.LookAt(player);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (playerHealth == null || playerHealth.isDead) return;

            // Cooldown check (Wait 1.5s between attacks)
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                playerHealth.TakeDamage(damage);
                lastAttackTime = Time.time;
            }
        }
    }
}
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    public bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log("Player took damage: " + amount + " | Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("Player Died! Game Over.");

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



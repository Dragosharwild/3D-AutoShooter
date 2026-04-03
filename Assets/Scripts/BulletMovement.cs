using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    public float bulletSpeed = 20f;        // Speed of bullet
    public float bulletLifetime = 3f;      // Auto destroy
    public float damage = 20f;             // Damage to enemies

    private Vector3 moveDirection;

    void Start()
    {
        Destroy(gameObject, bulletLifetime); // Auto destroy after lifetime
    }

    void Update()
    {
        if (moveDirection != Vector3.zero)
        {
            transform.position += moveDirection * bulletSpeed * Time.deltaTime;
        }
    }

    // Called by PlayerAutoShoot to set the shooting direction
    public void SetDirection(Vector3 dir)
    {
        moveDirection = dir.normalized;
        transform.rotation = Quaternion.LookRotation(moveDirection);
    }

    void OnTriggerEnter(Collider other)
    {
        // Only hit objects tagged "Enemy"
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            Destroy(gameObject); // Destroy bullet on hit
        }
    }
}
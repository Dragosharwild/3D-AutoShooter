using UnityEngine;

public class PlayerAutoShoot : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject bulletPrefab;       // Bullet prefab
    public Transform firePoint;           // Where bullets spawn
    public float fireRate = 0.2f;         // Seconds between shots

    [Header("Camera Reference")]
    public Camera playerCamera;           // Camera to shoot where it faces

    private float nextFireTime;

    void Update()
    {
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null || playerCamera == null) return;

        // Get camera forward direction
        Vector3 direction = playerCamera.transform.forward;

        // Remove vertical component to make horizontal
        direction.y = 0;
        direction.Normalize(); // normalize after changing Y

        // Spawn bullet
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(direction));

        // Set bullet movement
        BulletMovement bulletScript = bullet.GetComponent<BulletMovement>();
        if (bulletScript != null)
        {
            bulletScript.SetDirection(direction);
        }
    }
}
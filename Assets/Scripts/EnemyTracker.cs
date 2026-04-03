using UnityEngine;

public class EnemyTracker : MonoBehaviour
{
    public EnemySpawner spawner;

    private void OnDestroy()
    {
        if (spawner != null)
        {
            spawner.EnemyDied();
        }
    }
}

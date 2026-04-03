using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // Singleton instance to make it accessible from anywhere
    public static GameManager Instance;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // persists between scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Call this when the player dies
    public void GameOver(float delay = 1f)
    {
        StartCoroutine(GameOverCoroutine(delay));
    }

    private IEnumerator GameOverCoroutine(float delay)
    {
        // Optional: wait a little before restarting
        yield return new WaitForSeconds(delay);

        // Unlock cursor so menu can be clicked
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Load the first scene (Scene 0)
        SceneManager.LoadScene(0);
    }
}
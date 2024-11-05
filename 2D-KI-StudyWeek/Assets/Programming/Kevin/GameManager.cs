using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int playerScore = 0;

    private void Awake()
    {
        // Singleton-Pattern, um sicherzustellen, dass nur eine Instanz des GameManagers existiert
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Methode zum Setzen der Spielerpunktzahl
    public void UpdatePlayerScore(int cardValue)
    {
        playerScore += cardValue;
    }

    // Methode zum Abrufen der aktuellen Punktzahl
    public int GetPlayerScore()
    {
        return playerScore;
    }

    // Optional: Spielerpunktzahl zurücksetzen (für neuen Durchlauf)
    public void ResetPlayerScore()
    {
        playerScore = 0;
    }
}

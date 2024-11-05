using UnityEngine;

public class PlayerRound : MonoBehaviour
{
    public void UpdatePlayerScore()
    {
        // Setzt die Punktzahl auf null zurück, um neu zu berechnen
        GameManager.Instance.playerScore = 0;
        int aceCount = 0;

        // Summieren aller Kartenwerte
        foreach (int value in GameManager.Instance.PlayerCardValues)
        {
            if (value == 11) aceCount++; // Zählt die Asse
            GameManager.Instance.playerScore += value;
        }

        // Ass-Regel anwenden: Ändert 11 in 1, falls notwendig, um Bust zu verhindern
        while (GameManager.Instance.playerScore > 21 && aceCount > 0)
        {
            GameManager.Instance.playerScore -= 10; // Ein Ass von 11 auf 1 umwandeln
            aceCount--;
        }
    }
}

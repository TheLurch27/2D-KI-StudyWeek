using UnityEngine;

public class PlayerRound : MonoBehaviour
{
    public void UpdatePlayerScore()
    {
        GameManager.Instance.playerScore = 0;
        int aceCount = 0;

        foreach (int value in GameManager.Instance.PlayerCardValues)
        {
            if (value == 11) aceCount++;
            GameManager.Instance.playerScore += value;
        }

        while (GameManager.Instance.playerScore > 21 && aceCount > 0)
        {
            GameManager.Instance.playerScore -= 10;
            aceCount--;
        }
    }
}

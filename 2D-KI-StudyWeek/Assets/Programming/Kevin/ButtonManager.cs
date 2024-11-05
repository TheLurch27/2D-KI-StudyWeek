using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public Button hitButton;
    public Button standButton;

    private void Start()
    {
        // Buttons den Methoden zuweisen
        hitButton.onClick.AddListener(OnHitButton);
        standButton.onClick.AddListener(OnStandButton);
    }

    private void OnHitButton()
    {
        GameManager.Instance.DrawCard();

        // �berpr�fen, ob der Spieler Bust geht
        if (GameManager.Instance.playerScore > 21) // Direkt auf playerScore zugreifen
        {
            EndPlayerTurn();
            Debug.Log("Bust! Player's turn ends.");
        }
    }

    private void OnStandButton()
    {
        EndPlayerTurn();
        Debug.Log("Player stands.");
    }

    private void EndPlayerTurn()
    {
        // Deaktiviert die Buttons, um den Zug zu beenden
        hitButton.interactable = false;
        standButton.interactable = false;

        // Weitere Logik f�r den Zug des Croupiers kann hier hinzugef�gt werden
    }

    public void EnableButtons()
    {
        // Aktiviert die Buttons f�r den n�chsten Zug
        hitButton.interactable = true;
        standButton.interactable = true;
    }
}

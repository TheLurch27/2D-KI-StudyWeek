using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerBlackJackRound : MonoBehaviour
{
    [SerializeField] private List<GameObject> cardPrefabs; // Liste aller Karten-Prefabs
    [SerializeField] private Transform playerCardSlot1; // Slot für die erste Karte
    [SerializeField] private TextMeshProUGUI playerScoreText; // Textfeld für die Punktzahl
    [SerializeField] private AudioSource audioSource; // Audioquelle zum Abspielen der Zahlen-AudioClips

    private void Start()
    {
        DrawRandomCard();
    }

    private void DrawRandomCard()
    {
        // Zufällige Karte aus der Liste ziehen
        int randomIndex = Random.Range(0, cardPrefabs.Count);
        GameObject drawnCardPrefab = cardPrefabs[randomIndex];

        // Karte im Slot platzieren
        GameObject cardInstance = Instantiate(drawnCardPrefab, playerCardSlot1.position, Quaternion.identity, playerCardSlot1);

        // Wert der Karte aus dem Namen erkennen
        int cardValue = GetCardValueFromName(drawnCardPrefab.name);
        playerScoreText.text = cardValue.ToString();

        // Punktzahl im GameManager speichern
        GameManager.Instance.UpdatePlayerScore(cardValue);

        // AudioClip abspielen
        PlayCardValueAudio(cardValue);
    }

    private int GetCardValueFromName(string cardName)
    {
        // Wert basierend auf dem Namen festlegen
        if (cardName.Contains("Ace"))
        {
            return 1; // Kann als 1 oder 11 gezählt werden
        }
        else if (cardName.Contains("King") || cardName.Contains("Queen") || cardName.Contains("Jack"))
        {
            return 10;
        }
        else
        {
            // Extrahiere die Zahl aus dem Namen (z.B., "Clubs 2" -> 2)
            string[] nameParts = cardName.Split(' ');
            if (int.TryParse(nameParts[1], out int value))
            {
                return value;
            }
        }
        return 0; // Fallback, falls keine Zahl erkannt wird
    }

    private void PlayCardValueAudio(int cardValue)
    {
        string audioClipName = cardValue.ToString();
        AudioClip clip = Resources.Load<AudioClip>($"Audio/Voices/{audioClipName}");

        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"AudioClip für '{audioClipName}' nicht gefunden.");
        }
    }
}

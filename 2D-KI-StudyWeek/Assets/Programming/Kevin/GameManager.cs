using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player Settings")]
    public List<GameObject> cardDeck; // Alle Karten-Prefabs im Deck
    public GameObject cardBackPrefab; // Das Prefab der verdeckten Karte
    public Transform cardDeckSpawn; // Der Ort, wo Karten gespawnt werden
    public Transform playerCardSlot1; // Slot für die erste Karte und ungerade Positionen
    public Transform playerCardSlot2; // Slot für die zweite Karte und gerade Positionen
    public TMP_Text playerScoreText; // UI-Text zur Anzeige des Punktestands

    [Header("Card Movement Settings")]
    public float moveSpeed = 1.1f; // Geschwindigkeit der Kartenbewegung, im Inspector anpassbar

    [Header("Audio Settings")]
    public List<AudioClip> voiceClips; // Liste der AudioClips für die Zahlen 1-30
    private AudioSource audioSource; // AudioSource zum Abspielen der Clips

    private Dictionary<int, AudioClip> audioClipDictionary = new Dictionary<int, AudioClip>();

    public int playerScore = 0; // Die aktuelle Punktzahl des Spielers
    private int cardCount = 0; // Hält die Anzahl der gezogenen Karten
    private List<int> playerCardValues = new List<int>(); // Speichert die Werte der gezogenen Karten für die Berechnung

    // Öffentlich zugängliche Property für playerCardValues
    public List<int> PlayerCardValues
    {
        get { return playerCardValues; }
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Start()
    {
        InitializeAudioClips(); // Initialisiert das Dictionary für die AudioClips
        DrawCard(); // Teilt die erste Karte aus, sobald die Szene geladen wird
        UpdateScoreUI(); // Zeigt den Punktestand an, wenn das Spiel startet
    }

    private void InitializeAudioClips()
    {
        // Mappt jeden Punktestand (1-30) auf den entsprechenden AudioClip
        foreach (AudioClip clip in voiceClips)
        {
            string clipName = clip.name.ToLower(); // Namen in Kleinbuchstaben konvertieren für eine einfachere Zuordnung
            int score = ConvertClipNameToScore(clipName);
            if (score != -1)
            {
                audioClipDictionary[score] = clip;
            }
        }
    }

    private int ConvertClipNameToScore(string clipName)
    {
        // Wandelt den ausgeschriebenen Namen in eine Zahl um
        Dictionary<string, int> numberMapping = new Dictionary<string, int>()
        {
            { "one", 1 }, { "two", 2 }, { "three", 3 }, { "four", 4 }, { "five", 5 },
            { "six", 6 }, { "seven", 7 }, { "eight", 8 }, { "nine", 9 }, { "ten", 10 },
            { "eleven", 11 }, { "twelve", 12 }, { "thirteen", 13 }, { "fourteen", 14 }, { "fifteen", 15 },
            { "sixteen", 16 }, { "seventeen", 17 }, { "eighteen", 18 }, { "nineteen", 19 }, { "twenty", 20 },
            { "twenty-one", 21 }, { "twenty-two", 22 }, { "twenty-three", 23 }, { "twenty-four", 24 },
            { "twenty-five", 25 }, { "twenty-six", 26 }, { "twenty-seven", 27 }, { "twenty-eight", 28 },
            { "twenty-nine", 29 }, { "thirty", 30 }
        };

        return numberMapping.ContainsKey(clipName) ? numberMapping[clipName] : -1;
    }

    public void DrawCard()
    {
        if (cardDeck.Count == 0) return; // Keine Karten mehr im Deck

        GameObject drawnCard = Instantiate(cardBackPrefab, cardDeckSpawn.position, Quaternion.identity);
        drawnCard.layer = 3;

        Transform targetSlot = (cardCount % 2 == 0) ? playerCardSlot1 : playerCardSlot2;

        StartCoroutine(MoveCardToSlot(drawnCard, targetSlot.position));

        cardCount++;
    }

    private IEnumerator MoveCardToSlot(GameObject card, Vector3 targetPosition)
    {
        while (Vector3.Distance(card.transform.position, targetPosition) > 0.01f)
        {
            card.transform.position = Vector3.MoveTowards(card.transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        card.transform.position = targetPosition;

        yield return StartCoroutine(FlipCard(card));
    }

    private IEnumerator FlipCard(GameObject card)
    {
        float flipSpeed = 300f;
        float flipAngle = 0f;

        while (flipAngle < 90f)
        {
            float rotationStep = flipSpeed * Time.deltaTime;
            flipAngle += rotationStep;
            card.transform.Rotate(0, rotationStep, 0);
            yield return null;
        }

        int randomIndex = Random.Range(0, cardDeck.Count);
        GameObject newCard = Instantiate(cardDeck[randomIndex], card.transform.position, Quaternion.identity);
        newCard.transform.rotation = card.transform.rotation;
        newCard.layer = 3;
        Destroy(card);
        cardDeck.RemoveAt(randomIndex);

        while (flipAngle < 180f)
        {
            float rotationStep = flipSpeed * Time.deltaTime;
            flipAngle += rotationStep;
            newCard.transform.Rotate(0, rotationStep, 0);
            yield return null;
        }

        newCard.transform.rotation = Quaternion.Euler(0, 0, 0);

        int cardValue = GetCardValue(newCard.name);
        playerCardValues.Add(cardValue);
        UpdatePlayerScore();
        UpdateScoreUI();
    }

    public int GetCardValue(string cardName)
    {
        cardName = cardName.Replace("(Clone)", "").Trim();

        if (cardName.Contains("Joker"))
        {
            Debug.LogWarning("Joker-Karte hat keinen Wert und wird ignoriert.");
            return 0;
        }

        string[] parts = cardName.Split(' ');
        if (parts.Length > 1 && int.TryParse(parts[1], out int cardValue))
        {
            if (cardValue == 1)
                return 11;
            else if (cardValue >= 11 && cardValue <= 13)
                return 10;
            else
                return cardValue;
        }

        Debug.LogWarning("Kartenwert konnte nicht bestimmt werden: " + cardName);
        return 0;
    }

    public void UpdatePlayerScore()
    {
        playerScore = 0;
        int aceCount = 0;

        foreach (int value in playerCardValues)
        {
            if (value == 11) aceCount++;
            playerScore += value;
        }

        while (playerScore > 21 && aceCount > 0)
        {
            playerScore -= 10;
            aceCount--;
        }
    }

    private void UpdateScoreUI()
    {
        playerScoreText.text = playerScore.ToString();
        PlayScoreAudio(playerScore); // Spielt den passenden Audio-Clip ab
    }

    private void PlayScoreAudio(int score)
    {
        if (audioClipDictionary.TryGetValue(score, out AudioClip clipToPlay) && clipToPlay != null)
        {
            audioSource.PlayOneShot(clipToPlay);
        }
        else
        {
            Debug.LogWarning("Kein passender Audio-Clip für den Score gefunden oder fehlt im Dictionary: " + score);
        }
    }
}

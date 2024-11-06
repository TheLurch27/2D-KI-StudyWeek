using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<GameObject> cardDeck;
    public GameObject cardBackPrefab;
    public Transform cardDeckSpawn;
    public Transform playerCardSlot1;
    public Transform playerCardSlot2;
    public TMP_Text playerScoreText;

    public float moveSpeed = 1.1f;

    public List<AudioClip> voiceClips;
    private AudioSource audioSource;

    private Dictionary<int, AudioClip> audioClipDictionary = new Dictionary<int, AudioClip>();

    public int playerScore = 0;
    private int cardCount = 0;
    private List<int> playerCardValues = new List<int>();

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
        InitializeAudioClips();
        DrawCard();
        UpdateScoreUI();
    }

    private void InitializeAudioClips()
    {
        foreach (AudioClip clip in voiceClips)
        {
            string clipName = clip.name.ToLower();
            int score = ConvertClipNameToScore(clipName);
            if (score != -1)
            {
                audioClipDictionary[score] = clip;
            }
        }
    }

    private int ConvertClipNameToScore(string clipName)
    {
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
        if (cardDeck.Count == 0) return;

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
        PlayScoreAudio(playerScore);
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

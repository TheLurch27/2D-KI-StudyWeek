using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "BlackJack/Card")]
public class CardData : ScriptableObject
{
    public Sprite cardImage; // Bild der Karte
    public int primaryValue; // Hauptpunktzahl (z.B., 2-10, oder 10 f�r Bildkarten)
    public int secondaryValue; // Alternative Punktzahl f�r Ass (1 oder 11)
}

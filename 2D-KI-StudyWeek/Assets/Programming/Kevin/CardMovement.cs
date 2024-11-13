using UnityEngine;
using System.Collections;

public class CardMovement : MonoBehaviour
{
    public Transform spawnPoint;       // Der Punkt, an dem die Karte gespawnt wird
    public Transform playerSlot_1;     // Zielslot für die Karte
    public float moveSpeed = 5f;       // Geschwindigkeit der Bewegung

    private void Start()
    {
        // Setze die Position der Karte auf den Spawnpunkt und starte die Bewegung zur Slot-Position
        transform.position = spawnPoint.position;
        StartCoroutine(MoveToSlot());
    }

    private IEnumerator MoveToSlot()
    {
        while (Vector3.Distance(transform.position, playerSlot_1.position) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerSlot_1.position, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }
}

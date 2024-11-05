using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CardScriptableObjectCreator : EditorWindow
{
    [SerializeField]
    private List<GameObject> cardPrefabs = new List<GameObject>();

    [MenuItem("Tools/Card ScriptableObject Creator")]
    public static void ShowWindow()
    {
        GetWindow<CardScriptableObjectCreator>("Card ScriptableObject Creator");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Drag all card prefabs here:", EditorStyles.boldLabel);
        SerializedObject serializedObject = new SerializedObject(this);
        SerializedProperty cardPrefabsProperty = serializedObject.FindProperty("cardPrefabs");
        EditorGUILayout.PropertyField(cardPrefabsProperty, true);
        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Create Card ScriptableObjects"))
        {
            CreateCardScriptableObjects();
        }
    }

    private void CreateCardScriptableObjects()
    {
        foreach (GameObject prefab in cardPrefabs)
        {
            if (prefab == null) continue;

            string cardName = prefab.name;
            int primaryValue = 0;
            int secondaryValue = 0;

            // Punktewerte basierend auf dem Namen setzen
            if (cardName.Contains("Ace"))
            {
                primaryValue = 1;
                secondaryValue = 11;
            }
            else if (int.TryParse(cardName.Split(' ')[1], out int value))
            {
                primaryValue = value;
            }
            else
            {
                primaryValue = 10; // Für Bildkarten wie Jack, Queen, King
            }

            // ScriptableObject erstellen und Werte setzen
            CardData newCardData = CreateInstance<CardData>();
            newCardData.name = cardName;
            newCardData.cardImage = prefab.GetComponent<SpriteRenderer>().sprite; // Annahme: Prefab hat SpriteRenderer
            newCardData.primaryValue = primaryValue;
            newCardData.secondaryValue = secondaryValue;

            // Speichern des ScriptableObjects
            AssetDatabase.CreateAsset(newCardData, $"Assets/ScriptableObjects/Cards/{cardName}.asset");
            AssetDatabase.SaveAssets();
        }

        Debug.Log("Alle Karten-ScriptableObjects wurden erstellt.");
    }
}

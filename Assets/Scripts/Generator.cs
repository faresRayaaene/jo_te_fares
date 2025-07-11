using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public GameObject[] cardPrefabs;  // Group of different card prefabs
    public int rows = 2;
    public int columns = 3;
    public float spacingX = 2f;
    public float spacingY = 2.5f;

    void Start()
    {
        CreateMatchingCardGrid();
    }

    void CreateMatchingCardGrid()
    {
        int totalCards = rows * columns;

        // Ensure even number of cards
        if (totalCards % 2 != 0)
        {
            Debug.LogError("Total number of cards must be even!");
            return;
        }

        int pairCount = totalCards / 2;
        List<GameObject> selectedPairs = new List<GameObject>();

        // Select and duplicate card prefabs
        for (int i = 0; i < pairCount; i++)
        {
            GameObject randomCard = cardPrefabs[Random.Range(0, cardPrefabs.Length)];
            selectedPairs.Add(randomCard); // first of the pair
            selectedPairs.Add(randomCard); // second of the pair
        }

        // Shuffle the list
        for (int i = 0; i < selectedPairs.Count; i++)
        {
            int rand = Random.Range(i, selectedPairs.Count);
            GameObject temp = selectedPairs[i];
            selectedPairs[i] = selectedPairs[rand];
            selectedPairs[rand] = temp;
        }

        // Instantiate cards in grid and assign cardID
        int index = 0;
        int cardID = 0;
        HashSet<int> assigned = new HashSet<int>(); // track which pair index has already been assigned

        Dictionary<GameObject, int> cardPrefabIDs = new Dictionary<GameObject, int>();

        for (int i = 0; i < selectedPairs.Count; i++)
        {
            GameObject prefab = selectedPairs[i];

            // Check if prefab has already been assigned an ID
            if (!cardPrefabIDs.ContainsKey(prefab))
            {
                cardPrefabIDs[prefab] = cardID;
                cardID++;
            }

            int row = index / columns;
            int col = index % columns;
            Vector3 spawnPosition = new Vector3(col * spacingX, 0, -row * spacingY);

            GameObject cardInstance = Instantiate(prefab, spawnPosition, Quaternion.identity, transform);
            card cardScript = cardInstance.GetComponent<card>();
            if (cardScript == null)
            {
                cardScript = cardInstance.AddComponent<card>();
            }

            cardScript.cardID = cardPrefabIDs[prefab];
            index++;
        }
    }

}

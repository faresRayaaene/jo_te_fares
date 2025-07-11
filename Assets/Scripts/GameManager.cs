using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
  public int score = 0;

    private card firstSelected = null;
    private card secondSelected = null;

    public void OnCardSelected(card selectedCard)
    {
        if (firstSelected == null)
        {
            firstSelected = selectedCard;
        }
        else if (secondSelected == null && selectedCard != firstSelected)
        {
            secondSelected = selectedCard;
            CheckMatch();
        }
    }

    void CheckMatch()
    {
        if (firstSelected.cardID == secondSelected.cardID)
        {
            score += 1;
            Debug.Log("Match! Score: " + score);
        }
        else
        {
            Debug.Log("No match.");
        }

        // Reset selection after short delay
        Invoke(nameof(ResetSelection), 0.5f);
    }

    void ResetSelection()
    {
        firstSelected = null;
        secondSelected = null;
    }
}

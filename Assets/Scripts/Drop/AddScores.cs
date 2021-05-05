using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AddScores : Drop
{
    public int ScoresAmount = 1;

    protected override void Pickup()
    {
        GameManager.Instance._gameScores += ScoresAmount;
        UIManager.Instance.UpdateScoresText();
    }
}

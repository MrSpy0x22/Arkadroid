using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Multiball : Drop
{
    protected override void Pickup()
    {
        foreach (var ball in BallsManager.Instance.BallsCollection.ToList())
        {
            BallsManager.Instance.AddBalls(ball.gameObject.transform.position , 2);
        }
    }
}

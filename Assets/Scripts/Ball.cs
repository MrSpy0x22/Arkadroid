using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public static event Action<Ball> OnBallDie;

    public void Die()
    {
        OnBallDie?.Invoke(this);
        Destroy(gameObject , 1);
    }

    // Ignore ball to ball collisions
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ball"))
        {
            Physics2D.IgnoreCollision(collision.collider , this.gameObject.GetComponent<Collider2D>() , true);
        }
    }
}

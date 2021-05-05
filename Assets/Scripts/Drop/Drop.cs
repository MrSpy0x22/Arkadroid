using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Drop : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Platform"))
        {
            this.Pickup();
            Destroy(this.gameObject);
        }
        else if (collision.CompareTag("DeathWall"))
        {
            Destroy(this.gameObject);
        }
    }

    protected abstract void Pickup();
}

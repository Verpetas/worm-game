using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Powerup : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player1" || other.tag == "Player2")
        {
            AudioManager.Instance.PlaySound("Pickup");
            Collect(other.GetComponent<WormController>());
            Destroy(gameObject);
        }
    }

    public abstract void Collect(WormController controller);
}

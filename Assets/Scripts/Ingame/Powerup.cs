using UnityEngine;
using System.Collections;

public class Powerup : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        Player _player = other.GetComponent<Player>();
        if (_player != null)
        {
            if (_player.Mode == Player.Modes.OUTOFCONTROL)
            {
                Pickup();
            }
        }
    }

    public virtual void Pickup()
    {
        Disappear();
    }

    public void Disappear()
    {
        Destroy(gameObject);
    }
}

using UnityEngine;
using System.Collections;

public class Wind : MonoBehaviour
{
   public float Force = 50;

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.GetComponent<Rigidbody2D>() != null)
        {
            bool _canAffect = true;

            Player _player = other.GetComponent<Player>();
            if (_player != null)
            {
                if (_player.Mode == Player.Modes.INACTIVE)
                { _canAffect = false; }
            }

            if (_canAffect)
            {
                other.GetComponent<Rigidbody2D>().AddForce(transform.up * Force);
            }
        }
    }
}

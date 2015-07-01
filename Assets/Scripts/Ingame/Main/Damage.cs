using UnityEngine;
using System.Collections;

public class Damage : MonoBehaviour
{
    public int Quantity = 999;

    public int Knockback = 0;

    void OnTriggerEnter2D(Collider2D other)
    {
        Monster _monster = other.GetComponent<Monster>();
        if (_monster != null)
        {
            _monster.Damage(Quantity);
            return;
        }

        Player _player = other.GetComponent<Player>();
        if (_player != null)
        {
            if (Quantity != 0)
            {
                _player.Damage(Quantity);
            }

            if (Knockback != 0)
            {
                _player.Knockback((_player.transform.position - transform.position).normalized * Knockback);
            }

            return;
        }

        if (other.GetComponent<Rigidbody2D>() != null && other.GetComponent<Bullet>() == null)
        {
            //Other things: destroy them immediately
            GameManager.Instance.Effects.Smoke(other.transform.position);
            Destroy(other.gameObject);
        }
    }
}

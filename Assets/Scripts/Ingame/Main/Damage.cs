using UnityEngine;
using System.Collections;

public class Damage : MonoBehaviour
{
    public int Quantity = 999;

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
            _player.Damage(Quantity);
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

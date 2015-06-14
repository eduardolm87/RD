using UnityEngine;
using System.Collections;

public class Bullet : Damage
{
    public enum Affiliations { Player, Enemy, Other };

    [HideInInspector]
    public GameObject Owner;

    public Affiliations Affiliation = Affiliations.Enemy;
    public bool DamageAllies = false;
    public bool IgnoreObstacles = true;
    public bool DestroyOnTouch = true;
    public float Duration = 2;
    public float Speed = 2;

    void Start()
    {
        Invoke("Die", Duration);
    }

    void Die()
    {
        Destroy(gameObject);
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == Owner)
            return;

        Monster _monster = other.GetComponent<Monster>();
        if (_monster != null)
        {
            if (Affiliation != Affiliations.Enemy || DamageAllies)
            {
                _monster.Damage(Quantity);
                if (DestroyOnTouch) Destroy(gameObject);
            }
            return;
        }

        Player _player = other.GetComponent<Player>();
        if (_player != null)
        {
            if (Affiliation != Affiliations.Player)
            {
                _player.Damage(Quantity);
                if (DestroyOnTouch) Destroy(gameObject);
            }
            return;
        }

        if (other.GetComponent<Rigidbody2D>() != null && !IgnoreObstacles)
        {
            //Other things: destroy them immediately
            GameManager.Instance.Effects.Smoke(other.transform.position);
            Destroy(other.gameObject);
            if (DestroyOnTouch) Destroy(gameObject);
        }
    }
}

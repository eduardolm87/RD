﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Monster : MonoBehaviour
{
    public MonsterAttributes Attributes = new MonsterAttributes();

    public List<Reward> Rewards = new List<Reward>();

    [HideInInspector]
    public bool AttackedByPlayer = false;


    [HideInInspector]
    public CharacterSprite CharSprite;
    [HideInInspector]
    public Brain Brain;
    [HideInInspector]
    public Rigidbody2D Rigidbody;
    [HideInInspector]
    public Collider2D Collider;
    [HideInInspector]
    public NPCGUI UI;

    private bool charging = false;
    public bool Charging
    {
        get { return charging; }
        set
        {
            charging = value;
            if (charging)
            {
                Invoke("ChargeTimer", 1.5f);
            }
            else
            {
                CancelInvoke("ChargeTimer");
            }
        }

    }





    void ChargeTimer()
    {
        if (Charging)
        {
            Charging = false;
        }
    }




    void Awake()
    {
        CharSprite = GetComponentInChildren<CharacterSprite>();
        Brain = GetComponentInChildren<Brain>();
        Rigidbody = GetComponentInChildren<Rigidbody2D>();
        Collider = GetComponentInChildren<Collider2D>();
        UI = NPCGUI.CreateNPCGUI(this);
    }

    void Start()
    {
        Attributes.Restore();
    }

    public void Damage(int quantity, GameObject source = null)
    {
        bool _resistDamage = Attributes.Alterations.Contains(Alteration.DamageResistant) && source != null;

        if (!_resistDamage)
            Attributes.HP = Mathf.Clamp(Attributes.HP - quantity, 0, Attributes.HPmax);

        if (Charging)
        {
            Charging = false;
        }

        if (Attributes.HP < 1)
        {
            Die();
        }
        else
        {
            GameManager.Instance.SoundManager.Play("DamageToEntity");
            UI.Refresh();
            CharSprite.WinkInColor(GameManager.Instance.Effects.DamagedColor);

            if (!_resistDamage)
                GameManager.Instance.Effects.FloatingDmgText(quantity.ToString(), Color.white, transform.position, gameObject);

            if (Brain != null)
            {
                InvokeRepeating("RestoreAnimation", 0.25f, 0.25f);
            }
        }
    }

    public void Die()
    {
        if (GameManager.Instance.currentlyLoadedStage.Goal == Stage.StageGoals.KillCertainEnemy)
        {
            if (GameManager.Instance.currentlyLoadedStage.EnemyToKill == this)
            {
                GameManager.Instance.currentPlayer.targetEnemyKilled = true;
            }
        }

        GameManager.Instance.SoundManager.Play("EnemyDie");
        GameManager.Instance.Effects.Smoke(transform.position);
        DropReward();
        Destroy(gameObject);
    }

    public void DisableAnimation()
    {
        if (Brain == null)
            return;

        if (Attributes.Alterations.Contains(Alteration.NonstopAnimation))
            return;

        Brain.enabled = false;
        AttackedByPlayer = true;
    }

    void RestoreAnimation()
    {
        if (GetComponent<Rigidbody2D>().velocity.sqrMagnitude < 2)
        {
            Brain.enabled = true;
            AttackedByPlayer = false;

            CancelInvoke("RestoreAnimation");
        }
    }

    public void PauseForAMoment(float time)
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        Brain.enabled = false;
        Invoke("RestoreFromPause", time);
    }

    void RestoreFromPause()
    {
        Brain.enabled = true;
    }

    public void OnCollisionEnter2D(Collision2D col)
    {
        Monster _monster = col.collider.GetComponent<Monster>();
        if (_monster != null)
        {
            if (col.relativeVelocity.sqrMagnitude > 100)
            {
                OnCollisionWithMonster(_monster);
            }
        }
        else if (col.collider.GetComponent<Player>() != null)
        {
            OnCollisionWithPlayer();
        }
        else
        {
            Rigidbody2D colRigidbody = col.collider.GetComponent<Rigidbody2D>();
            if (colRigidbody != null)
            {
                if (col.relativeVelocity.sqrMagnitude > 100)
                {
                    OnCollisionWithRigidbody(colRigidbody, col.relativeVelocity);
                }
            }
            else
            {
                OnCollisionWithSolid(col.collider);
            }

        }
    }

    void OnCollisionWithMonster(Monster zMonster)
    {
        //Monster hits other monsters in chain effect
        if (AttackedByPlayer)
        {
            zMonster.Damage(1, zMonster.gameObject);
        }
    }

    void OnCollisionWithPlayer()
    {

    }

    void OnCollisionWithRigidbody(Rigidbody2D zRigidbody, Vector2 zRelativeVelocity)
    {
        //Obstacles hit me
        if (zRigidbody.velocity.sqrMagnitude > 2)
        {
            int _damage = Mathf.Clamp(Mathf.RoundToInt(zRelativeVelocity.sqrMagnitude / 80), 1, 3);
            Damage(_damage, zRigidbody.gameObject);
        }
    }

    void OnCollisionWithSolid(Collider2D zCollider)
    {

    }

    void DropReward()
    {
        List<GameObject> _dropped = new List<GameObject>();

        foreach (Reward _rw in Rewards)
        {
            if (_rw.Chance == Reward.Chances.Never)
                continue;
            else if (_rw.Chance == Reward.Chances.Always)
                _dropped.Add(_rw.Item);
            else
            {
                if (Random.Range(1, 6) < (int)_rw.Chance)
                {
                    _dropped.Add(_rw.Item);
                }
            }
        }

        foreach (GameObject _reward in _dropped)
        {

            Vector2 _direction = (transform.position - GameManager.Instance.currentPlayer.transform.position).normalized;

            GameObject _rwd = Instantiate(_reward, (Vector2)transform.position + _direction * 0.5f, Quaternion.identity) as GameObject;


        }
    }
}

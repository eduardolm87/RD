using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Monster : MonoBehaviour
{
    public Attributes Attributes = new Attributes();

    public List<Reward> Rewards = new List<Reward>();

    public NPCGUI UI;

    public CharacterSprite CharSprite;

    AIArtifact AIArtifact;


    [HideInInspector]
    public bool CollidedWithPlayer = false;
    [HideInInspector]
    public bool AttackedByPlayer = false;


    void Start()
    {
        Attributes.Restore();

        if (UI == null)
        {
            UI = GetComponentInChildren<NPCGUI>();
        }

        if (CharSprite == null)
        {
            CharSprite = GetComponentInChildren<CharacterSprite>();
        }

        UI.ShowHP(Attributes.HP);
        AIArtifact = GetComponentInChildren<AIArtifact>();
    }

    public void Damage(int quantity, GameObject source = null)
    {
        bool _resistDamage = Attributes.Alterations.Contains(Alteration.DamageResistant) && source != null;


        if (!_resistDamage)
            Attributes.HP = Mathf.Clamp(Attributes.HP - quantity, 0, Attributes.HPmax);

        if (Attributes.HP < 1)
        {
            Die();
        }
        else
        {
            GameManager.Instance.SoundManager.Play("hit");
            UI.ShowHP(Attributes.HP);
            CharSprite.WinkInColor(GameManager.Instance.Effects.DamagedColor);

            if (!_resistDamage)
                GameManager.Instance.Effects.Minitext(quantity.ToString(), Color.white, ((Vector2)Camera.main.WorldToScreenPoint(transform.position) + Vector2.up * 20), gameObject);

            if (AIArtifact != null)
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

        GameManager.Instance.SoundManager.Play("enemydie");
        GameManager.Instance.Effects.Smoke(transform.position);
        DropReward();
        Destroy(gameObject);
    }

    public void DisableAnimation()
    {
        if (AIArtifact == null)
            return;

        if (Attributes.Alterations.Contains(Alteration.NonstopAnimation))
            return;

        AIArtifact.enabled = false;
        AttackedByPlayer = true;
    }

    void RestoreAnimation()
    {
        if (GetComponent<Rigidbody2D>().velocity.sqrMagnitude < 2)
        {
            AIArtifact.enabled = true;
            AttackedByPlayer = false;

            CancelInvoke("RestoreAnimation");
        }
    }

    public void PauseForAMoment(float time)
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        AIArtifact.enabled = false;
        Invoke("RestoreFromPause", time);
    }

    void RestoreFromPause()
    {
        AIArtifact.enabled = true;
    }

    public void AttackPlayer()
    {
        CollidedWithPlayer = true;
        Invoke("RestoreAttackPlayer", 1 + (Attributes.Alterations.Contains(Alteration.WaitMoreUntilAttackAgain) ? 1.5f : 0));
    }

    void RestoreAttackPlayer()
    {
        CollidedWithPlayer = false;
    }

    public void OnCollisionEnter2D(Collision2D col)
    {
        Monster _monster = col.collider.GetComponent<Monster>();
        if (_monster != null)
        {
            if (AttackedByPlayer)
            {
                if (col.relativeVelocity.sqrMagnitude > 100)
                {
                    _monster.Damage(1, _monster.gameObject);
                }
            }
        }
        else
        {
            if (col.collider.GetComponent<Player>() == null)
            {
                if (!AttackedByPlayer && !CollidedWithPlayer)
                {
                    if (col.collider.GetComponent<Rigidbody2D>() != null)
                    {
                        if (col.relativeVelocity.sqrMagnitude > 100)
                        {
                            if (col.collider.GetComponent<Rigidbody2D>().velocity.sqrMagnitude > 2)
                            {
                                int _damage = Mathf.Clamp(Mathf.RoundToInt(col.relativeVelocity.sqrMagnitude / 80), 1, 3);
                                Damage(_damage, col.collider.gameObject);
                            }
                        }
                    }
                }
            }
        }
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

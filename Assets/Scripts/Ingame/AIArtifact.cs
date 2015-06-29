using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AIArtifact : MonoBehaviour
{
    #region Behaviors

    [Serializable]
    public class Behavior
    {
        public enum Types { Wait = 0, Continue, NormalMovement, FollowPlayer, Random, Shoot };
        public Types Type;

        public Vector2 Direction_Nrml_Rnd = Vector2.zero;
        public float Speed_Fllw_Rnd = 1;
        public Bullet Bullet_Shoot = null;
    }


    #endregion

    public float totalTime = 1;
    public float visionDistance = 50;
    public List<Behavior> Behaviors = new List<Behavior>();

    #region internal
    float _reserve_bhvTime = 0;
    float _bhvTimer = 0;
    int _currentBehavior = 0;
    float AnimationSpeed = 1;
    bool _alreadyShot = false;
    Rigidbody2D rigidbody;
    #endregion

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        _bhvTimer = Time.time - _reserve_bhvTime;
    }

    void OnDisable()
    {
        _reserve_bhvTime = Time.time - _bhvTimer;
    }

    void Update()
    {
        AnimationSpeed = Behaviors.Count;

        if (Time.time - _bhvTimer > (totalTime / Behaviors.Count))
        {
            //Next bhv
            _currentBehavior++;
            if (_currentBehavior >= Behaviors.Count)
                _currentBehavior = 0;

            if (_currentBehavior >= Behaviors.Count)
                return;

            if (Behaviors[_currentBehavior].Type == Behavior.Types.Random)
                Behaviors[_currentBehavior].Direction_Nrml_Rnd = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));

            _alreadyShot = false;
            _bhvTimer = Time.time;
        }
        else if (PlayerInVisionRange())
        {
            ChangeMusicIfBoss();

            switch (Behaviors[_currentBehavior].Type)
            {
                case Behavior.Types.Continue:
                    //do nothing
                    break;

                case Behavior.Types.NormalMovement:
                    rigidbody.velocity = (Behaviors[_currentBehavior].Direction_Nrml_Rnd * AnimationSpeed);
                    break;

                case Behavior.Types.FollowPlayer:
                    if (GameManager.Instance.currentPlayer != null)
                    {
                        rigidbody.velocity = (GameManager.Instance.currentPlayer.transform.position - transform.position).normalized * Behaviors[_currentBehavior].Speed_Fllw_Rnd;
                    }
                    break;

                case Behavior.Types.Wait:
                    rigidbody.velocity = Vector2.zero;
                    break;

                case Behavior.Types.Random:
                    rigidbody.velocity = (Behaviors[_currentBehavior].Direction_Nrml_Rnd * Behaviors[_currentBehavior].Speed_Fllw_Rnd);
                    break;

                case Behavior.Types.Shoot:
                    if (!_alreadyShot)
                    {
                        Shoot(Behaviors[_currentBehavior].Bullet_Shoot);
                        _alreadyShot = true;
                    }
                    break;
            }

        }
    }


    void Shoot(Bullet projectilePrefab)
    {
        GameObject _proj = Instantiate(projectilePrefab.gameObject, transform.position, Quaternion.identity) as GameObject;
        Bullet _bullet = _proj.GetComponent<Bullet>();
        _bullet.transform.parent = GameManager.Instance.GameWindow.transform;
        _bullet.Owner = gameObject;

        //Target
        Vector2 _dir = (GameManager.Instance.currentPlayer.transform.position - transform.position).normalized;
        _bullet.GetComponent<Rigidbody2D>().velocity = _dir * _bullet.Speed;
    }

    bool PlayerInVisionRange()
    {
        if (GameManager.Instance.currentPlayer != null)
            return (GameManager.Instance.currentPlayer.transform.position - transform.position).sqrMagnitude < visionDistance;
        else
            return false;
    }

    bool _musicChanged = false;
    void ChangeMusicIfBoss()
    {
        if (_musicChanged)
            return;

        bool isBoss = false;
        Monster monster = GetComponent<Monster>();
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (monster != null && renderer != null)
        {
            if (monster.Attributes.Alterations.Contains(Alteration.Boss) && renderer.isVisible)
                isBoss = true;
        }

        if (isBoss)
        {
            GameManager.Instance.SoundManager.PlayMusic(GameManager.Instance.SoundManager.GetCurrentMusicClip(), 1.5f);
            _musicChanged = true;
        }
    }
}

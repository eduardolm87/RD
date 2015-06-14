﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    public enum Modes { AIMING, OUTOFCONTROL, INACTIVE };
    enum InputState { NOTPRESSED = 0, FIRSTTOUCH, CONTINUETOUCH, ENDTOUCH, OVERUI };

    public static Vector2? aimingFrom = null, aimingTo = null;


    public AimHUD AimHUD;

    public CharacterSprite CharSprite;

    private Hero HeroData;

    [HideInInspector]
    public float AccumulatedPower = 0;

    float AccumulationPowerRate = 0.06f;

    float refreshHUDFreq = 0.1f;

    float damageWithSpeedFactor = 80;


    #region internal

    float lowspeedThreshold = 3;
    float msUnderLowspeedTreshold = 0.25f;
    float lowspdTr_time = 0;
    float combatMinForceThreshold = 10;
    List<Monster> _monstersCollidedWith = new List<Monster>();

    [HideInInspector]
    public Modes Mode = Modes.OUTOFCONTROL;
    Modes PreviousMode = Modes.OUTOFCONTROL;
    public enum EndStageConditions { NONE = 0, WIN, LOSE };
    bool reachedGoal = false;
    [HideInInspector]
    public bool targetEnemyKilled = false;

    [HideInInspector]
    public PlayerAttributes Attributes = new PlayerAttributes();

    [HideInInspector]
    public bool pressedPauseButton = false;

    InputState previousState = InputState.NOTPRESSED;


    #endregion

    void Start()
    {
        ChangeMode(Modes.AIMING);
        InvokeRepeating("RefreshHUD", 0, refreshHUDFreq);
    }

    void Update()
    {
        if (Mode != Modes.INACTIVE)
        {
            CheckPauseButton();
            AutoWinCheat();
        }

        switch (Mode)
        {
            case Modes.AIMING:
                AimingControl();
                break;

            case Modes.OUTOFCONTROL:
                OutOfControl();
                break;

            case Modes.INACTIVE:
                break;
        }

    }

    void OnCollisionEnter2D(Collision2D other)
    {

        Monster _monster = other.collider.GetComponent<Monster>();
        if (_monster != null)
        {
            if (other.relativeVelocity.sqrMagnitude > combatMinForceThreshold)
            {
                if (!_monstersCollidedWith.Contains(_monster))
                {
                    //Attack to monster
                    Combat(_monster, DamageBonusOfSpeed(other.relativeVelocity.sqrMagnitude));
                }
            }
            else if (!_monster.CollidedWithPlayer)
            {
                //Receive damage from monster
                _monster.AttackPlayer();
                _monster.PauseForAMoment(1);
                GetComponent<Rigidbody2D>().velocity = (_monster.transform.position - transform.position);
                Mode = Modes.OUTOFCONTROL;

                int _damageReceived = 1;
                if (_monster.Attributes.Alterations.Contains(Alteration.ClumsyAttack) && Random.Range(1, 10) > 7)
                    _damageReceived = 0;

                Damage(_damageReceived);
                AimHUD.SetDirection(_monster.transform.position);
            }
        }
        else
            SolidBump(other.collider);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (GameManager.Instance.currentlyLoadedStage.Goal == Stage.StageGoals.ReachToCertainPoint)
        {
            if (other == GameManager.Instance.currentlyLoadedStage.PlaceToReach)
            {
                reachedGoal = true;
                DragStop();
            }
        }
    }


    public void LoadHero(Hero zHero)
    {
        HeroData = zHero;

        Attributes = HeroData.Attributes;
        Attributes.Restore();

        CharSprite.Sprites.ToList().ForEach(s => s = HeroData.Graphic);
        
        //Upgrades (later)
    }


    float DamageBonusOfSpeed(float relativeSpeed)
    {
        return Mathf.Clamp(relativeSpeed / damageWithSpeedFactor, 0.6f, 3);
    }

    void AimingControl()
    {
        switch (GetCurrentInputState())
        {
            case InputState.OVERUI:
                break;

            case InputState.FIRSTTOUCH:
                aimingFrom = Input.mousePosition;
                break;

            case InputState.CONTINUETOUCH:
                if (!EventSystem.current.IsPointerOverGameObject())
                    aimingTo = Input.mousePosition;
                break;

            case InputState.ENDTOUCH:
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    //Reduce STA when charging
                    if (Attributes.Alterations.Contains(Alteration.DamageWhenMoving))
                    {
                        Attributes.Stamina = Mathf.Clamp(Attributes.Stamina - 1, 0, Attributes.Stamina_max);
                    }

                    Charge(AimHUD.transform.rotation.eulerAngles.z, Attributes.Impulse);
                }
                break;

            default:
                AccumulatedPower = 0;
                aimingFrom = null;
                aimingTo = null;
                break;
        }
    }

    InputState GetCurrentInputState()
    {
        InputState result = InputState.NOTPRESSED;

        if (Input.GetMouseButton(0))
        {
            if (previousState == InputState.NOTPRESSED)
                result = InputState.FIRSTTOUCH;
            else
                result = InputState.CONTINUETOUCH;
        }
        else if (previousState == InputState.CONTINUETOUCH || previousState == InputState.FIRSTTOUCH)
            result = InputState.ENDTOUCH;


        previousState = result;

        return result;
    }


    void OutOfControl()
    {
        if (GetComponent<Rigidbody2D>().velocity.sqrMagnitude < lowspeedThreshold)
        {
            if (Time.time - lowspdTr_time > 0.5)
            {
                DragStop();
            }
        }
        else
        {
            lowspdTr_time = Time.time;
        }
    }

    void Charge(float angle, float strength)
    {
        GetComponent<Rigidbody2D>().velocity = AimHUD.GetDirection() * strength * AccumulatedPower;

        ChangeMode(Modes.OUTOFCONTROL);

        AccumulatedPower = 0;
    }

    void ChangeMode(Modes newMode)
    {
        PreviousMode = Mode;
        Mode = newMode;
    }

    void DragStop()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Rigidbody2D>().angularVelocity = 0;


        _monstersCollidedWith.Clear();

        switch (CheckConditionsForEnd())
        {
            case EndStageConditions.WIN:
                Mode = Modes.INACTIVE;
                GameManager.Instance.WinStage();
                break;

            case EndStageConditions.LOSE:
                Mode = Modes.INACTIVE;
                GameManager.Instance.LoseStage();
                break;

            default:
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                ChangeMode(Modes.AIMING);
                break;
        }

    }

    public void Combat(Monster monster, float DamageBonus)
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        _monstersCollidedWith.Add(monster);

        if (!monster.Attributes.Alterations.Contains(Alteration.NonPushable))
        {
            //Damage sequence
            monster.DisableAnimation();
            int _finalDamage = Mathf.RoundToInt(Attributes.Attack * DamageBonus);
            monster.Damage(_finalDamage, this.gameObject);
        }
        else
            GameManager.Instance.SoundManager.Play("bouncehit");
    }

    void RefreshHUD()
    {
        if (Time.timeScale == 1)
            GameManager.Instance.TimeRealplay += refreshHUDFreq;

        if (Mode == Modes.AIMING)
        {
            if (!GameManager.UsePowerBarWithTouchDuration || Input.GetMouseButton(0))
            {
                AccumulatePower();
            }
        }

        GameManager.Instance.HUD.RefreshHUD(Attributes);
    }

    public void AccumulatePower()
    {
        if (GameManager.SwipeToChargePower)
        {
            if (aimingFrom != null && aimingTo != null)
            {
                float _dist = (aimingTo.Value - aimingFrom.Value).magnitude;

                float _charge = Mathf.Clamp(_dist / (Screen.width / 2.5f), 0.1f, 1); //75
                AccumulatedPower = _charge;
            }

        }
        else
        {
            AccumulatedPower = Mathf.Clamp01(AccumulatedPower + AccumulationPowerRate);
        }
    }

    EndStageConditions CheckConditionsForEnd()
    {
        if (GameManager.Instance.currentlyLoadedStage.Goal == Stage.StageGoals.KillAllEnemies && GameManager.Instance.currentlyLoadedStage.NumberOfLivingMonsters() < 1)
        {
            return EndStageConditions.WIN;
        }
        else if (GameManager.Instance.currentlyLoadedStage.Goal == Stage.StageGoals.ReachToCertainPoint && reachedGoal)
        {
            return EndStageConditions.WIN;
        }
        else if (GameManager.Instance.currentlyLoadedStage.Goal == Stage.StageGoals.KillCertainEnemy && targetEnemyKilled)
        {
            return EndStageConditions.WIN;
        }
        else if (Attributes.Stamina < 1)
        {
            return EndStageConditions.LOSE;
        }

        return EndStageConditions.NONE;
    }

    void CheckPauseButton()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.M) || pressedPauseButton)
        {
            if (!GameManager.Instance.GamePaused)
            {
                PauseButton();
                pressedPauseButton = false;
            }
        }
    }



    void PauseButton()
    {
        GameManager.Instance.GamePaused = true;
        PreviousMode = Mode;
        Mode = Modes.INACTIVE;

        GameManager.Instance.GamePopup.Show(GameManager.Instance.GetPauseMenuText(), new PopupButton[] { 
            new PopupButton("Resume", () => 
            {
                //Resume
                GameManager.Instance.GamePaused = false;
                Mode = PreviousMode;
                Input.ResetInputAxes();
            }), 
            new PopupButton("Quit", () => 
            { 
                //Quit
                GameManager.Instance.GamePaused = false;
                GameManager.Instance.OpenStageSelect();
            }) });
    }

    public void SolidBump(Collider2D other)
    {
        if (other.GetComponent<Rigidbody2D>() != null)
        {
            //Mobile objects
            GameManager.Instance.SoundManager.Play("bump");
            GetComponent<Rigidbody2D>().velocity /= 4;
        }

        Rubber _rubber = other.GetComponent<Rubber>();

        if (_rubber != null)
        {
            //Rubber
            if (_rubber._inflatedStg == 0 && GetComponent<Rigidbody2D>().velocity.sqrMagnitude < 500 && GetComponent<Rigidbody2D>().velocity.sqrMagnitude > 4)
            {
                GetComponent<Rigidbody2D>().velocity *= 1.25f;
            }
            GameManager.Instance.SoundManager.Play("bouncehit");
        }
        else
        {
            //Default walls
            GameManager.Instance.SoundManager.Play("wall");
        }

    }

    public void Heal(int quantity)
    {
        GameManager.Instance.SoundManager.Play("foodeat");
        Attributes.Stamina = Mathf.Clamp(Attributes.Stamina + quantity, 0, Attributes.Stamina_max);
    }

    public void Damage(int quantity)
    {
        if (quantity > 1)
        {
            quantity -= Attributes.Defense;
            if (quantity < 1)
                quantity = 1;
        }

        GameManager.Instance.SoundManager.Play("bighit");
        Attributes.Stamina = Mathf.Clamp(Attributes.Stamina - quantity, 0, Attributes.Stamina_max);

        if (quantity > 0)
        {
            CharSprite.WinkInColor(GameManager.Instance.Effects.DamagedColor);
            GameManager.Instance.Effects.Minitext(quantity.ToString(), GameManager.Instance.Effects.DamagedColor, ((Vector2)Camera.main.WorldToScreenPoint(transform.position) + Vector2.up * 20), gameObject);
            GameCamera.Instance.Shake();

        }

        RefreshHUD();

        if (Attributes.Stamina < 1)
        {
            Die();
        }
        else
        {
            //Repel damage
            GetComponent<Rigidbody2D>().velocity = (-GetComponent<Rigidbody2D>().velocity.normalized) * 5;
            AccumulatedPower = 0;
        }
    }

    public void Die()
    {
        Mode = Modes.INACTIVE;

        GameManager.Instance.Effects.Smoke(transform.position);

        GetComponent<Renderer>().enabled = false;

        GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        Invoke("DieActivate", 1);
    }

    void DieActivate()
    {
        Mode = Modes.OUTOFCONTROL;
    }

    void AutoWinCheat()
    {
        if (GameManager.WinStagesWith1Key)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Monster[] _monsters = GameObject.FindObjectsOfType<Monster>();
                foreach (Monster _m in _monsters)
                {
                    _m.Damage(999);
                }
            }
        }

    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    //Declarations
    public enum EndStageConditions { NONE = 0, WIN, LOSE };
    public enum Modes { AIMING, OUTOFCONTROL, INACTIVE };
    enum InputState { NOTPRESSED = 0, FIRSTTOUCH, CONTINUETOUCH, ENDTOUCH, OVERUI };


    //Components
    public CharacterSprite CharSprite;
    public AimHUD AimHUD;
    private Hero HeroData;
    public Collider2D Collider;
    [HideInInspector]
    public PlayerAttributes Attributes = new PlayerAttributes();
    public Rigidbody2D Rigidbody;



    //Public variables
    [HideInInspector]
    public float AccumulatedPower = 0;
    [HideInInspector]
    public bool pressedPauseButton = false;
    [HideInInspector]
    public bool targetEnemyKilled = false;
    public static Vector2? aimingFrom = null, aimingTo = null;
    public bool Dead = false;
    public Modes Mode = Modes.OUTOFCONTROL;




    //Internal
    float lowspeedThreshold = 3;
    float msUnderLowspeedTreshold = 0.25f;
    float lowspdTr_time = 0;
    float smokeRunTime = 0;
    float combatMinForceThreshold = 10;
    List<Monster> monstersCollidedWith = new List<Monster>();

    [HideInInspector]
    Modes PreviousMode = Modes.OUTOFCONTROL;
    bool reachedGoal = false;

    float AccumulationPowerRate = 0.06f;
    float refreshHUDFreq = 0.1f;
    float damageWithSpeedFactor = 80;
    float TimerSinceLastChargeAudio = 0;
    float previouslyAccumulatedPower = 0;
    float accumulatingPowerSoundTimer = 0;

    InputState previousState = InputState.NOTPRESSED;



    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Collider = GetComponent<Collider2D>();
        ChangeMode(Modes.AIMING);
        InvokeRepeating("RefreshHUD", 0, refreshHUDFreq);
        accumulatingPowerSoundTimer = Time.time;
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
        if (Dead)
            return;

        Monster monster = other.collider.GetComponent<Monster>();
        if (monster != null)
        {
            if (other.relativeVelocity.sqrMagnitude > combatMinForceThreshold)
            {
                if (!monstersCollidedWith.Contains(monster))
                {
                    //Attack to monster
                    OnHitToMonster(monster, other.relativeVelocity);
                }
            }
            else if (monster.Charging)
            {
                //Receive damage from monster
                OnHitByMonster(monster, other.relativeVelocity);
            }

            if (!monstersCollidedWith.Contains(monster))
                monstersCollidedWith.Add(monster);
        }
        else
        {
            Interactive interactive = other.collider.GetComponent<Interactive>();
            if (interactive != null)
            {
                interactive.OnPlayerTouch();
            }
            else
            {
                OnHitWithSolid(other.collider);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (Dead)
            return;

        if (GameManager.Instance.currentlyLoadedStage.Goal == Stage.StageGoals.ReachToCertainPoint)
        {
            if (other == GameManager.Instance.currentlyLoadedStage.PlaceToReach)
            {
                reachedGoal = true;
                DragStop();
                return;
            }
        }

        BuildingVisibilityTrigger building = other.GetComponent<BuildingVisibilityTrigger>();
        if (building != null)
        {
            building.Building.Enter();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (Dead)
            return;

        BuildingVisibilityTrigger building = other.GetComponent<BuildingVisibilityTrigger>();
        if (building != null)
        {
            building.Building.Exit();
        }
    }

    public void Combat(Monster monster, float DamageBonus)
    {
        Rigidbody.velocity = Vector2.zero;

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

    #region Control

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

    void AccumulatingPowerSound(float zCurrentlyAccumulatedPower)
    {
        if (Time.time - accumulatingPowerSoundTimer > 0.16f)
        {
            if (zCurrentlyAccumulatedPower > previouslyAccumulatedPower)
            {
                GameManager.Instance.SoundManager.Play("AccumulatingPower");
            }
            accumulatingPowerSoundTimer = Time.time;
            previouslyAccumulatedPower = zCurrentlyAccumulatedPower;
        }
    }

    void PauseButton()
    {

        GameManager.Instance.SoundManager.Play("Pause");


        GameManager.Instance.GamePaused = true;
        PreviousMode = Mode;
        Mode = Modes.INACTIVE;

        GameManager.Instance.GamePopup.Show(GameManager.Instance.GetPauseMenuText(), new PopupButton[] { 
            new PopupButton("Resume", () => 
            {
                //Resume
                GameManager.Instance.SoundManager.Play("Confirm");

                GameManager.Instance.GamePaused = false;
                Mode = PreviousMode;
                Input.ResetInputAxes();
            }), 
            new PopupButton("Quit", () => 
            { 
                //Quit
                GameManager.Instance.SoundManager.Play("Confirm");

                GameManager.Instance.GamePaused = false;
                GameManager.Instance.OpenStageSelect();
            }) });
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
                AccumulatingPowerSound(AccumulatedPower);
            }

        }
        else
        {
            AccumulatedPower = Mathf.Clamp01(AccumulatedPower + AccumulationPowerRate);
        }
    }

    void AimingControl()
    {
        switch (GetCurrentInputState())
        {
            case InputState.OVERUI:
                break;

            case InputState.FIRSTTOUCH:
                if (Vector2.Distance(Input.mousePosition, Camera.main.WorldToScreenPoint(transform.position)) < 100)
                {
                    aimingFrom = Input.mousePosition;
                }
                else
                    aimingFrom = null;

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
        if (Rigidbody.velocity.sqrMagnitude < lowspeedThreshold)
        {
            if (Time.time - lowspdTr_time > 0.5)
            {
                DragStop();
            }
        }
        else
        {
            lowspdTr_time = Time.time;

            if (!Dead && Time.time - smokeRunTime > 0.18f)
            {
                GameManager.Instance.Effects.SmallSmoke(transform.position);
                smokeRunTime = Time.time;
            }
        }
    }

    void Charge(float angle, float strength)
    {
        Rigidbody.velocity = AimHUD.GetDirection() * strength * AccumulatedPower;

        ChangeMode(Modes.OUTOFCONTROL);

        if (Time.time - TimerSinceLastChargeAudio > 0.5f)
        {
            GameManager.Instance.SoundManager.Play("Charge");
            TimerSinceLastChargeAudio = Time.time;
        }


        GameManager.Instance.Effects.SmallSmoke(transform.position);

        smokeRunTime = Time.time;

        AccumulatedPower = 0;
    }

    void ChangeMode(Modes newMode)
    {
        PreviousMode = Mode;
        Mode = newMode;
    }

    void DragStop()
    {
        Rigidbody.velocity = Vector2.zero;
        Rigidbody.angularVelocity = 0;


        monstersCollidedWith.Clear();

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
                Rigidbody.velocity = Vector2.zero;
                ChangeMode(Modes.AIMING);
                break;
        }

    }

    #endregion

    #region Collisions

    void OnHitByMonster(Monster zMonster, Vector2 zRelativeVelocity)
    {
        zMonster.PauseForAMoment(1);

        Knockback(zMonster.transform.position - transform.position);

        Mode = Modes.OUTOFCONTROL;

        DamageByMonster(zMonster);

        AimHUD.SetDirection(zMonster.transform.position);
    }

    void OnHitToMonster(Monster zMonster, Vector2 zRelativeVelocity)
    {
        GameManager.Instance.Effects.Hit(transform.position + ((zMonster.transform.position - transform.position) / 2f));

        Combat(zMonster, DamageBonusOfSpeed(zRelativeVelocity.sqrMagnitude));
    }

    void DamageByMonster(Monster zMonster)
    {
        int _damageReceived = zMonster.Attributes.Attack;

        if (zMonster.Attributes.Alterations.Contains(Alteration.ClumsyAttack) && Random.Range(1, 10) > 7)
        {
            _damageReceived = 0;
        }

        Damage(_damageReceived);
    }

    public void OnHitWithSolid(Collider2D other)
    {
        if (other.GetComponent<Rigidbody2D>() != null)
        {
            //Mobile objects
            GameManager.Instance.SoundManager.Play("BumpWithOtherRigidbodies");
            Rigidbody.velocity /= 4;
        }

        Rubber _rubber = other.GetComponent<Rubber>();

        if (_rubber != null)
        {
            //Rubber
            if (_rubber._inflatedStg == 0 && Rigidbody.velocity.sqrMagnitude < 500 && Rigidbody.velocity.sqrMagnitude > 4)
            {
                Rigidbody.velocity *= 1.25f;
            }
            GameManager.Instance.SoundManager.Play("BumpWithRubber");
        }
        else
        {
            //Default walls
            GameManager.Instance.SoundManager.Play("BumpWithWall");
        }

    }

    #endregion

    #region Attributes

    public void LoadHero(Hero zHero)
    {
        HeroData = zHero;

        Attributes = HeroData.Attributes;
        Attributes.Restore();

        List<Sprite> sprites = new List<Sprite>();
        sprites.Add(zHero.Graphic);
        sprites.Add(zHero.Graphic);
        CharSprite.Sprites = sprites.ToArray();

        CharSprite.Renderer = GetComponent<SpriteRenderer>();
        CharSprite.Renderer.sprite = zHero.Graphic;


        //Upgrades (later)
    }

    float DamageBonusOfSpeed(float relativeSpeed)
    {
        return Mathf.Clamp(relativeSpeed / damageWithSpeedFactor, 0.6f, 3);
    }

    public void Heal(int quantity)
    {
        GameManager.Instance.SoundManager.Play("FoodEat");
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

        GameManager.Instance.SoundManager.Play("DamageToPlayer");
        Attributes.Stamina = Mathf.Clamp(Attributes.Stamina - quantity, 0, Attributes.Stamina_max);

        if (quantity > 0)
        {
            CharSprite.WinkInColor(GameManager.Instance.Effects.DamagedColor);
            GameManager.Instance.Effects.FloatingDmgText(quantity.ToString(), GameManager.Instance.Effects.DamagedColor, transform.position, gameObject);
            GameCamera.Instance.Shake();

        }

        RefreshHUD();

        if (Attributes.Stamina < 1)
        {
            Die();
        }
        else
        {
            //Automatic knockback
            Knockback((-Rigidbody.velocity.normalized) * 5);
            AccumulatedPower = 0;
        }
    }

    public void Knockback(Vector2 zKnockback)
    {
        ChangeMode(Modes.OUTOFCONTROL);

        Rigidbody.velocity = zKnockback;
    }

    public void Die()
    {
        Mode = Modes.INACTIVE;

        GameManager.Instance.Effects.Smoke(transform.position);

        CharSprite.Renderer.enabled = false;

        Rigidbody.velocity = Vector2.zero;

        Dead = true;

        Collider.enabled = false;

        Vector3 position = GameCamera.Instance.transform.position;
        GameCamera.Instance.transform.parent = GameManager.Instance.currentlyLoadedStage.transform;
        GameCamera.Instance.transform.rotation = Quaternion.identity;
        GameCamera.Instance.transform.position = position;

        Invoke("DieActivate", 1);
    }

    void DieActivate()
    {
        Mode = Modes.OUTOFCONTROL;
    }

    #endregion

    void AutoWinCheat()
    {
        if (GameManager.Instance.WinStagesWith1Key)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && Mode == Modes.AIMING)
            {
                GameManager.Instance.WinStage();
                //Monster[] _monsters = GameObject.FindObjectsOfType<Monster>();
                //foreach (Monster _m in _monsters)
                //{
                //    _m.Damage(999);
                //}
            }
        }

    }
}

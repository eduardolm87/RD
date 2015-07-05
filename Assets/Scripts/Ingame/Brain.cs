using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Brain : MonoBehaviour
{
    protected Monster monster;

    void Awake()
    {
        monster = GetComponent<Monster>();
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {

    }


    protected void WanderRandom()
    {
        int directionChosen = Random.Range(0, 5);

        float speed = monster.Attributes.Impulse;

        switch (directionChosen)
        {
            case 0: return;
            default:
                MoveInDirection((CharacterControl.Directions)directionChosen, speed);
                break;
        }
    }

    protected void FaceTowardsPlayer()
    {
        Vector2 heroPosition = GameManager.Instance.currentPlayer.transform.position;

        if (heroPosition.x > transform.position.x)
        {
            monster.CharSprite.SpriteFacing = CharacterControl.Directions.Right;
        }
        else if (heroPosition.x < transform.position.x)
        {
            monster.CharSprite.SpriteFacing = CharacterControl.Directions.Left;
        }
    }

    protected void ChargeTowardsPlayer(float zSpeed)
    {
        monster.Charging = true;
        MoveTowardsPlayer(zSpeed);
    }

    protected void MoveTowardsPlayer(float zSpeed)
    {
        Vector2 direction = (GameManager.Instance.currentPlayer.transform.position - transform.position).normalized;
        MoveInDirection(direction, zSpeed);
    }

    protected void MoveInDirection(CharacterControl.Directions zDirection, float zSpeed)
    {
        Vector2 Direction = Vector2.right;
        switch (zDirection)
        {
            case CharacterControl.Directions.Right: Direction = Vector2.right; break;
            case CharacterControl.Directions.Left: Direction = Vector2.left; break;
            case CharacterControl.Directions.Up: Direction = Vector2.up; break;
            case CharacterControl.Directions.Down: Direction = Vector2.down; break;
        }

        MoveInDirection(Direction, zSpeed);
    }

    protected void MoveInDirection(Vector2 zDirection, float zSpeed)
    {

        monster.Rigidbody.velocity = zDirection * zSpeed;
    }

    protected bool PlayerInVisionRange()
    {
        if (GameManager.Instance.currentPlayer != null)
        {
            return (GameManager.Instance.currentPlayer.transform.position - transform.position).sqrMagnitude < monster.Attributes.range;
        }
        else
            return false;
    }

    public float DistanceToPlayer
    {
        get { return ((GameManager.Instance.currentPlayer.transform.position - transform.position).magnitude); }
    }

    public float SqrDistanceToPlayer
    {
        get { return ((GameManager.Instance.currentPlayer.transform.position - transform.position).sqrMagnitude); }
    }

    protected void Shoot(Bullet zProjectilePrefab, GameObject zTarget = null)
    {
        GameObject _proj = Instantiate(zProjectilePrefab.gameObject, transform.position, Quaternion.identity) as GameObject;
        Bullet _bullet = _proj.GetComponent<Bullet>();
        _bullet.transform.parent = GameManager.Instance.GameWindow.transform;
        _bullet.Owner = gameObject;

        //Target
        Transform targetTransform;
        if (zTarget == null && GameManager.Instance.currentPlayer != null)
            targetTransform = GameManager.Instance.currentPlayer.transform;
        else
            targetTransform = zTarget.transform;

        //Shoot
        Vector2 direction = (targetTransform.position - transform.position).normalized;

        SpriteLookAt(_bullet.transform, direction);
        _bullet.GetComponent<Rigidbody2D>().velocity = direction * _bullet.Speed;
    }


    public static void SpriteLookAt(Transform zTransform, Vector3 zWorldPosition)
    {
        Vector2 Direction = (zWorldPosition - zTransform.position).normalized;

        SpriteLookAt(zTransform, Direction);
    }

    public static void SpriteLookAt(Transform zTransform, Vector2 zDirection)
    {
        float targetAngle = Mathf.Atan2(zDirection.y, zDirection.x) * Mathf.Rad2Deg;
        zTransform.transform.Rotate(Vector3.forward, targetAngle);
    }
}

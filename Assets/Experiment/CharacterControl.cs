using UnityEngine;
using System.Collections;

public class CharacterControl : MonoBehaviour
{

    Animator _Animator;

    void Start()
    {
        _Animator = GetComponent<Animator>();

    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 _direction = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
            Orientate(_direction);
        }




    }


    public enum Directions { None = 0, Down = 1, Right = 2, Up = 3, Left = 4 };
    Directions _orientation = Directions.Down;


    public void Orientate(Vector2 direction)
    {
        float _angle = PosNegAngle(Vector2.right, direction, Vector3.forward);

        Directions _angleDir = DirectionBasedOnAngle(_angle);

        Move(_angleDir);
    }

    void Move(Directions dir)
    {
        switch (dir)
        {
            case Directions.Down:
                transform.localScale = new Vector3(1, 1, 1);
                _Animator.SetInteger("Run", (int)dir);
                _orientation = dir;
                break;
            case Directions.Up:
                transform.localScale = new Vector3(1, 1, 1);
                _Animator.SetInteger("Run", (int)dir);
                _orientation = dir;
                break;
            case Directions.Left:
                transform.localScale = new Vector3(-1, 1, 1);
                _Animator.SetInteger("Run", 2);
                _orientation = dir;
                break;
            case Directions.Right:
                transform.localScale = new Vector3(1, 1, 1);
                _Animator.SetInteger("Run", (int)dir);
                _orientation = dir;
                break;

            case Directions.None:
                _Animator.SetInteger("Run", 0);
                break;
        }
    }

    public static float PosNegAngle(Vector3 a1, Vector3 a2, Vector3 n)
    {
        float angle = Vector3.Angle(a1, a2);
        float sign = Mathf.Sign(Vector3.Dot(n, Vector3.Cross(a1, a2)));
        return angle * sign;
    }

    Directions DirectionBasedOnAngle(float angle)
    {
        if (angle >= 45 && angle < 135)
        {
            return Directions.Up;
        }
        else if (angle >= 135 || angle < -135)
        {
            return Directions.Left;
        }
        else if (angle >= -135 && angle < -45)
        {
            return Directions.Down;
        }
        else
            return Directions.Right;
    }


}

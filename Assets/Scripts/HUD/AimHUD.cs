using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AimHUD : MonoBehaviour
{
    float rotatingSpd = 8;
    public SpriteRenderer arrow;

    //void Update()
    //{
    //    transform.Rotate(Vector3.forward, rotatingSpd);
    //}

    public void Enable(float speed = 8)
    {
        if (!GameManager.ShowRotatingArrow)
            return;

        if (enabled)
            return;

        rotatingSpd = speed;
        enabled = true;
        arrow.enabled = true;
    }

    public void Disable()
    {
        if (!enabled)
            return;

        arrow.enabled = false;
        enabled = false;
    }

    public Vector2 GetDirection()
    {
        if (Player.aimingFrom != null && Player.aimingTo != null)
        {
            return (Player.aimingTo.Value - Player.aimingFrom.Value).normalized;
        }
        else
        {
            Debug.LogError("Error getting aimingTo or aimingFrom positions");
            return Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        }

        //return ((Vector2)(arrow.transform.position - transform.position)).normalized;
    }

    public void SetDirection(Vector2 point)
    {
        Vector3 _desiredRotation = point - (Vector2)transform.position;
        transform.rotation = Quaternion.Euler(_desiredRotation);
    }


}

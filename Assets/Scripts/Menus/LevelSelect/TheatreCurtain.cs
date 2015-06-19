using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class TheatreCurtain : MonoBehaviour
{
    public GameObject Right, Left, Top;

    [HideInInspector]
    public bool Busy = false;

    [HideInInspector]
    public bool Closed = true;

    public float MovementTime { get { return movementTime; } }
    float movementTime = 0.5f;

    public void InstantOpen()
    {
        Right.transform.localScale = new Vector3(0, 1, 1);
        Left.transform.localScale = new Vector3(0, 1, 1);

        Closed = false;
    }

    public void InstantClose()
    {
        Right.transform.localScale = new Vector3(1, 1, 1);
        Left.transform.localScale = new Vector3(1, 1, 1);

        Closed = true;
    }

    public void Open()
    {
        iTween.ScaleTo(Right, iTween.Hash("x", 0, "time", MovementTime, "easetype", iTween.EaseType.easeOutCubic));
        iTween.ScaleTo(Left, iTween.Hash("x", 0, "time", MovementTime, "easetype", iTween.EaseType.easeOutCubic, "oncomplete", "EndTransition", "oncompletetarget", gameObject));
    }

    public void Close()
    {
        iTween.ScaleTo(Right, iTween.Hash("x", 1, "time", MovementTime, "easetype", iTween.EaseType.easeInCubic));
        iTween.ScaleTo(Left, iTween.Hash("x", 1, "time", MovementTime, "easetype", iTween.EaseType.easeInCubic, "oncomplete", "EndTransition", "oncompletetarget", gameObject));
    }

    public void EndTransition()
    {
        Closed = !Closed;
    }

}

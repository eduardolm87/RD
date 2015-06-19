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

    public float CurtainsTime { get { return movementTime / 4f; } }
    public float MaxTime { get { return movementTime; } }
    float movementTime = 1f;

    [SerializeField]
    private GameObject Character;

    Vector3 OriginalCharacterPos;
    bool setup = false;

    void Setup()
    {
        if (!setup)
        {
            OriginalCharacterPos = Character.GetComponent<RectTransform>().anchoredPosition;
            setup = true;
        }
    }

    public void InstantOpen()
    {
        Setup();

        Right.transform.localScale = new Vector3(0, 1, 1);
        Left.transform.localScale = new Vector3(0, 1, 1);

        SetPlayerInOrigin();

        Closed = false;
    }

    public void InstantClose()
    {
        Setup();

        Right.transform.localScale = new Vector3(1, 1, 1);
        Left.transform.localScale = new Vector3(1, 1, 1);

        Closed = true;
    }

    void SetPlayerInOrigin()
    {
        Character.GetComponent<RectTransform>().anchoredPosition = OriginalCharacterPos;
    }

    public void Open(int zDirectionOfComing = -1)
    {
        Setup();

        iTween.ScaleTo(Right, iTween.Hash("x", 0, "time", CurtainsTime, "easetype", iTween.EaseType.easeOutCubic));
        iTween.ScaleTo(Left, iTween.Hash("x", 0, "time", CurtainsTime, "easetype", iTween.EaseType.easeOutCubic, "oncomplete", "EndTransition", "oncompletetarget", gameObject));

        if (zDirectionOfComing != 0)
        {
            SetPlayerInOrigin();
            Character.transform.position += (Vector3.right * 300 * zDirectionOfComing);
            iTween.MoveTo(Character, iTween.Hash("x", OriginalCharacterPos.x, "isLocal", true, "time", MaxTime));
        }
        else
        {
            SetPlayerInOrigin();
        }
    }

    public void Close(int zDirectionOfGoing = -1)
    {
        Setup();

        iTween.ScaleTo(Right, iTween.Hash("x", 1, "time", CurtainsTime, "easetype", iTween.EaseType.easeInCubic));
        iTween.ScaleTo(Left, iTween.Hash("x", 1, "time", CurtainsTime, "easetype", iTween.EaseType.easeInCubic, "oncomplete", "EndTransition", "oncompletetarget", gameObject));

        SetPlayerInOrigin();
        if (zDirectionOfGoing != 0)
        {
            iTween.MoveTo(Character, iTween.Hash("x", zDirectionOfGoing * 600, "isLocal", true, "time", MaxTime));
        }
    }

    public void EndTransition()
    {
        Closed = !Closed;
    }

}

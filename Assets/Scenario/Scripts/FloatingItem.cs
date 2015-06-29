using UnityEngine;
using System.Collections;

public class FloatingItem : MonoBehaviour
{
    public GameObject item, shadow;

    void Start()
    {
        //star
        iTween.MoveFrom(item, iTween.Hash("y", 0.25f, "islocal", true, "easetype", iTween.EaseType.linear, "time", 0.5f, "looptype", iTween.LoopType.pingPong));

        //shadow
        iTween.ScaleFrom(shadow, iTween.Hash("x", 0.5f, "time", 0.5f, "easetype", iTween.EaseType.linear, "looptype", iTween.LoopType.pingPong));
    }

}

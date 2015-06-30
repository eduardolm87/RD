using UnityEngine;
using System.Collections;

public class DisappearWithTime : MonoBehaviour
{
    public float Time = 0.5f;

    void Start()
    {
        Invoke("End", Time);
    }

    void End()
    {
        Destroy(gameObject);
    }
}

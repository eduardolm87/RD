using UnityEngine;
using System.Collections;

public class RotatPole : MonoBehaviour
{

    public float Speed = 1;

    void Update()
    {
        transform.Rotate(Vector3.forward, Speed);
    }
}

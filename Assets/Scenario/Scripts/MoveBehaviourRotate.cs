using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveBehaviourRotate : MoveBehaviour
{
    public List<Transform> FreezeRotation = new List<Transform>();

    public override void Update()
    {
        transform.Rotate(Vector3.forward, Speed * Time.deltaTime);
        if (FreezeRotation.Count > 0)
        {
            FreezeRotation.ForEach(t => t.rotation = Quaternion.identity);
        }
    }
}

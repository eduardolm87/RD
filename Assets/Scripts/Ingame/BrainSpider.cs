using UnityEngine;
using System.Collections;

public class BrainSpider : Brain
{
    float Frequency = 0.5f;

    protected override void Start()
    {
        base.Start();

        InvokeRepeating("DiagonalWander", Frequency, Frequency);
    }


    void DiagonalWander()
    {
        WanderRandom();

        monster.Rigidbody.velocity = Quaternion.AngleAxis(45, Vector3.forward) * monster.Rigidbody.velocity;
    }
}
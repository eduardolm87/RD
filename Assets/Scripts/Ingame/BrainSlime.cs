using UnityEngine;
using System.Collections;

public class BrainSlime : Brain
{
    float Frequency = 0.75f;

    protected override void Start()
    {
        base.Start();

        InvokeRepeating("WanderRandom", Frequency, Frequency);
    }

}

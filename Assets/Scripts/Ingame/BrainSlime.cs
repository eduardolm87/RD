using UnityEngine;
using System.Collections;

public class BrainSlime : Brain
{
    float Frequency = 0.75f;

    protected override void Start()
    {
        base.Start();

        InvokeRepeating("Act", Frequency, Frequency);
    }

    void Act()
    {
        if (PlayerInVisionRange())
        {
            ChargeTowardsPlayer(monster.Attributes.Impulse);
        }
        else
        {
            WanderRandom();
        }
    }
}

using UnityEngine;
using System.Collections;

public class BrainSoldier : Brain
{

    float Frequency = 0.65f;
    float Cooldown = 1.5f;
    float cooldownTimer = 0;

    protected override void Start()
    {
        base.Start();

        cooldownTimer = Time.time;

        InvokeRepeating("Act", Frequency, Frequency);
    }

    void Act()
    {
        if (PlayerInVisionRange())
        {
            if (Time.time - cooldownTimer > Cooldown)
            {
                ChargeTowardsPlayer(monster.Attributes.Impulse);
                cooldownTimer = Time.time;
            }
        }
    }
}

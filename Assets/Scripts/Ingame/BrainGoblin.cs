using UnityEngine;
using System.Collections;

public class BrainGoblin : Brain
{
    [SerializeField]
    protected Bullet Projectile;

    float Frequency = 0.75f;
    float Cooldown = 3f;
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
                FaceTowardsPlayer();
                Shoot(Projectile);
                cooldownTimer = Time.time;
            }
        }
        else
        {
            WanderRandom();
        }
    }
}

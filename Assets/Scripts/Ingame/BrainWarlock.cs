using UnityEngine;
using System.Collections;

public class BrainWarlock : Brain
{

    [SerializeField]
    protected Bullet Projectile;

    float Frequency = 0.65f;
    float Cooldown = 2.5f;
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
    }
}

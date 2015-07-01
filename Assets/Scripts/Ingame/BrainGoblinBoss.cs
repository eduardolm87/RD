using UnityEngine;
using System.Collections;

public class BrainGoblinBoss : BrainTransformFollower
{
    [SerializeField]
    protected Bullet Projectile;

    public float Cooldown = 3f;

    float cooldownTimer = 0;


    protected override void Start()
    {
        base.Start();

        Frequency = 0.1f;

        cooldownTimer = Time.time;
    }

    protected override void Act()
    {
        UpgradeSpeedWithDamage();

        bool acted = false;

        if (PlayerInVisionRange())
        {
            if (SqrDistanceToPlayer < monster.Attributes.range / 3f)
            {
                //Player in melee distance (or cooldown)
                ChargeTowardsPlayer(monster.Attributes.Impulse);
                acted = true;
            }
            else if (Time.time - cooldownTimer > Cooldown)
            {
                //Player in range, long distance
                Shoot(Projectile);
                cooldownTimer = Time.time;
                acted = true;
            }
        }

        if (!acted)
        {
            //Player not in range
            Follow();
        }
    }

    void UpgradeSpeedWithDamage()
    {
        float changeFactor = (1 + (1 - (monster.Attributes.HP / monster.Attributes.HPmax)));
        monster.Attributes.Impulse = monster.Attributes.Impulse_max * changeFactor;
    }
}

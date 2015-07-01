using UnityEngine;
using System.Collections;

public class BrainTransformFollower : Brain
{
    public Transform FollowObject;

    protected float Frequency = 0.75f;

    protected override void Start()
    {
        base.Start();

        InvokeRepeating("Act", Frequency, Frequency);
    }

    protected virtual void Act()
    {
        Follow();
    }

    protected void Follow()
    {
        if (FollowObject == null)
            return;

        if (CloseToFollowedObject())
            return;

        Vector2 direction = (FollowObject.position - transform.position).normalized;

        MoveInDirection(direction, monster.Attributes.Impulse);
    }

    bool CloseToFollowedObject()
    {
        return (FollowObject.position - transform.position).sqrMagnitude < 2;
    }
}

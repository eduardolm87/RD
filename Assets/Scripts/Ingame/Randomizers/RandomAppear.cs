using UnityEngine;
using System.Collections;

public class RandomAppear : Randomizer
{
    [Range(0, 100)]
    public int ChanceOfAppearing = 100;

    public override void Trigger()
    {
        DestroyIfNotChance();
        Terminate();
    }

    void DestroyIfNotChance()
    {
        int dice = Random.Range(1, 101);
        if (ChanceOfAppearing == 0 || ChanceOfAppearing > dice)
        {
            Destroy(gameObject);
        }
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemRandomizer : Randomizer
{
    public List<GameObject> Items = new List<GameObject>();

    public override void Trigger()
    {
        GameObject objToSpawn = PickRandomItem();
        if (objToSpawn != null)
        {
            Spawn(objToSpawn, transform.position);
        }
    }

    GameObject PickRandomItem()
    {
        int choice = Random.Range(0, (int)(Items.Count * 1.25f));

        if (choice < Items.Count)
            return Items[choice];

        return null;
    }
}

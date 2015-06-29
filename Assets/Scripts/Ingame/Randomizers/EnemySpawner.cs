using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemySpawner : Randomizer
{
    public List<GameObject> GameObjects = new List<GameObject>();

    public override void Trigger()
    {
        SpawnRandomObject();

        Terminate();
    }

    GameObject SpawnRandomObject()
    {
        if (GameObjects.Count > 0)
        {
            int choice = UnityEngine.Random.Range(0, (int)(GameObjects.Count * 1.25f));

            if (choice < GameObjects.Count)
            {
                Spawn(GameObjects[choice], transform.position);
            }
        }

        return null;
    }


}

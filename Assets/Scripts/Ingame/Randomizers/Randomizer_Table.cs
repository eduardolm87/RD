using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Randomizer_Table : Randomizer
{
    public GameObject TablePrefab;
    public Transform PositionOfSpawn;
    public List<GameObject> Objects = new List<GameObject>();

    public override void Trigger()
    {
        GameObject spawnedObject = Spawn(TablePrefab, transform.position);

        GameObject spawnRandomItemPrefab = PickRandomItem();
        if (spawnRandomItemPrefab != null)
        {
            GameObject spawnedItem = Spawn(spawnRandomItemPrefab, PositionOfSpawn.position);
        }
        Terminate();
    }

    GameObject PickRandomItem()
    {
        int choice = UnityEngine.Random.Range(0, (int)(Objects.Count * 1.25f));

        if (choice < Objects.Count)
        {
            return Objects[choice];
        }

        return null;
    }

}

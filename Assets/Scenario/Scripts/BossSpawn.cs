using UnityEngine;
using System.Collections;

public class BossSpawn : SpawnObject
{
    public GameObject ObjectToEnableWhenDie;

    GameObject instantiatedObject = null;

    protected override void Start()
    {
        Spawn();
        InvokeRepeating("CheckDefeat", 1, 1);
    }

    protected override void Spawn()
    {
        instantiatedObject = Instantiate(ObjectToSpawn, transform.position, transform.rotation) as GameObject;
        instantiatedObject.transform.SetParent(transform.parent);
    }

    void CheckDefeat()
    {
        if (instantiatedObject == null)
        {
            if (ObjectToEnableWhenDie == null)
            {
                Debug.LogError("Error: No object to spawn when enemy is dead");
                return;
            }

            instantiatedObject.SetActive(true);
        }
    }
}

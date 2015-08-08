using UnityEngine;
using System.Collections;

public class ContinuousSpawner : SpawnObject
{
    GameObject instantiatedObject = null;

    public bool AppearingAnimation = true;

    protected override void Start()
    {
        InvokeRepeating("CheckSpawn", 0.5f, 5);
        GetComponent<SpriteRenderer>().enabled = false;
    }

    void CheckSpawn()
    {
        if (instantiatedObject == null)
        {
            Spawn();
        }
    }

    protected override void Spawn()
    {
        instantiatedObject = Instantiate(ObjectToSpawn, transform.position, transform.rotation) as GameObject;
        instantiatedObject.transform.SetParent(transform.parent);

        if (AppearingAnimation)
        {
            iTween.MoveFrom(instantiatedObject, iTween.Hash("y", 10, "isLocal", true, "time", 0.25f, "easetype", iTween.EaseType.easeOutBounce));
        }
    }

}

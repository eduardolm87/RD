using UnityEngine;
using System.Collections;

public abstract class Randomizer : MonoBehaviour
{
    public virtual void Start()
    {
        Trigger();
    }

    public virtual void Trigger()
    {

    }

    public virtual void Terminate()
    {
        Destroy(gameObject);
    }

    public virtual GameObject Spawn(GameObject zGameObject, Vector3 zPosition)
    {
        if (zGameObject == null)
            return null;

        GameObject justSpawn = Instantiate(zGameObject, zPosition, zGameObject.transform.rotation) as GameObject;
        justSpawn.transform.SetParent(transform.parent, true);
        justSpawn.name = zGameObject.name;

        return justSpawn;
    }
}

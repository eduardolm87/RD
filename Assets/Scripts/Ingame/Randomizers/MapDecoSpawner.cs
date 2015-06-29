using UnityEngine;
using System.Collections;

public class MapDecoSpawner : MonoBehaviour
{
    public Randomizer Spawner;

    void Start()
    {
        GameObject.Instantiate(Spawner.gameObject, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}

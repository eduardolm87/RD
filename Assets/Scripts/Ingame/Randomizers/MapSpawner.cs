using UnityEngine;
using System.Collections;

public class MapSpawner : MonoBehaviour
{
    public EnemySpawner Spawner;

    void Start()
    {
        GameObject.Instantiate(Spawner.gameObject, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}

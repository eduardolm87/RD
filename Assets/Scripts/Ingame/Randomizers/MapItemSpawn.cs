using UnityEngine;
using System.Collections;

public class MapItemSpawn : MonoBehaviour
{
    public ItemRandomizer ItemData;

    [Range(0, 10)]
    public int Value = 2;

    void Start()
    {
        GameObject.Instantiate(ItemData.gameObject, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}

using UnityEngine;
using System.Collections;

public class SpawnWhenNear : MonoBehaviour
{
    public float Distance = 5;
    public GameObject Spawn;


    void OnEnable()
    {
        InvokeRepeating("CheckSpawn", 1, 1);
    }


    void CheckSpawn()
    {
        if (gameObject.activeInHierarchy)
            if (Vector2.Distance(GameManager.Instance.currentPlayer.transform.position, transform.position) < Distance)
            {
                GameObject _spawnedObj = Instantiate(Spawn, transform.position, Quaternion.identity) as GameObject;
                _spawnedObj.transform.parent = GameManager.Instance.GameWindow.transform;
                GameManager.Instance.Effects.Smoke(transform.position);
                CancelInvoke();
            }
    }

}

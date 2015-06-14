using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Portal : MonoBehaviour
{
    public Portal ConnectedTo;
    [HideInInspector]
    public List<GameObject> TeleportedObjects;


    [HideInInspector]
    public float _time = 0;

    float _refreshTime = 1;

    float rotatingSpd = 10;
    void Update()
    {
        transform.Rotate(Vector3.forward, rotatingSpd);

        if (Time.time - _time > _refreshTime)
        {
            ClearTeleport();
            _time = Time.time;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Rigidbody2D>() != null)
        {
            Teleport(other);
        }
    }


    void Teleport(Collider2D other)
    {
        if (ConnectedTo == null)
            return;

        if (TeleportedObjects.Contains(other.gameObject))
            return;

        other.transform.position = ConnectedTo.transform.position;

        //Conserve inertia and amplify if too slow
        if (other.GetComponent<Rigidbody2D>().velocity.sqrMagnitude < 3)
            other.GetComponent<Rigidbody2D>().velocity = other.GetComponent<Rigidbody2D>().velocity.normalized * 3;


        TeleportedObjects.Add(other.gameObject);
        ConnectedTo.TeleportedObjects.Add(other.gameObject);

        _time = Time.time;
        ConnectedTo._time = Time.time;
    }

    void ClearTeleport()
    {
        TeleportedObjects.Clear();
    }


}

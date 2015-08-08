using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Portal : MonoBehaviour
{
    public Portal ConnectedTo;

    public bool Rotate = true;
    public bool KeepInertia = true;

    [HideInInspector]
    public List<GameObject> TeleportedObjects;


    [HideInInspector]
    public float _time = 0;

    float _refreshTime = 1;

    float rotatingSpd = 10;
    void Update()
    {
        if (Rotate)
        {
            transform.Rotate(Vector3.forward, rotatingSpd);
        }

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

        Rigidbody2D OtherRigidbody = other.GetComponent<Rigidbody2D>();


        if (!KeepInertia)
        {
            //Remove Inertia
            OtherRigidbody.velocity = Vector2.zero;
        }
        else
        {
            //Conserve inertia and amplify if too slow
            if (OtherRigidbody.velocity.sqrMagnitude < 3)
                OtherRigidbody.velocity = other.GetComponent<Rigidbody2D>().velocity.normalized * 3;
        }

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

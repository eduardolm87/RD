using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Water : MonoBehaviour
{
    public float drag = 0.1f;


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Rigidbody2D>() != null && other.GetComponent<Bullet>() == null)
        {
            if (!GameManager.Instance.currentlyLoadedStage._previousIntertia.ContainsKey(other.transform))
                GameManager.Instance.currentlyLoadedStage._previousIntertia.Add(other.transform, other.GetComponent<Rigidbody2D>().drag);

            other.GetComponent<Rigidbody2D>().drag = drag;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Rigidbody2D>() != null && other.GetComponent<Bullet>() == null)
        {
            if (GameManager.Instance.currentlyLoadedStage._previousIntertia.ContainsKey(other.transform))
            {
                other.GetComponent<Rigidbody2D>().drag = GameManager.Instance.currentlyLoadedStage._previousIntertia[other.transform];
            }
        }
    }
}

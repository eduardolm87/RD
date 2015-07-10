using UnityEngine;
using System.Collections;

public class Interactive : MonoBehaviour
{
    SpriteRenderer Renderer;
    Collider2D Collider;

    void Awake()
    {
        Renderer = GetComponent<SpriteRenderer>();
        Collider = GetComponent<Collider2D>();
    }

    public virtual void OnPlayerTouch()
    {

    }
}

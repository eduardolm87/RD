using UnityEngine;
using System.Collections;

public class Switch : MonoBehaviour
{
    public SpriteRenderer Renderer;
    public Sprite ActiveSprite, InactiveSprite;

    public string ID = "Switch";

    public bool State = false;

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.GetComponent<Rigidbody2D>() != null)
        {
            State = true;
            UpdateSprite();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Rigidbody2D>() != null)
        {
            State = false;
            UpdateSprite();
        }
    }

    void UpdateSprite()
    {
        Renderer.sprite = State ? ActiveSprite : InactiveSprite;
    }

}

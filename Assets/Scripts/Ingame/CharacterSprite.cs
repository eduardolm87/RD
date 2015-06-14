using UnityEngine;
using System.Collections;

public class CharacterSprite : MonoBehaviour
{
    SpriteRenderer Renderer;
    public Sprite[] Sprites;

    int currentlyAppliedSprite = 0;

    CharacterControl.Directions spriteFace = CharacterControl.Directions.Right;
    public CharacterControl.Directions SpriteFacing
    {
        get { return spriteFace; }
        set
        {
            spriteFace = value;
            SetSpriteOrientation();
        }
    }

    Rigidbody2D rigidbody;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        if (rigidbody != null)
        {
            rigidbody.angularDrag = 10;
            rigidbody.fixedAngle = false;
        }
        Renderer = GetComponent<SpriteRenderer>();
        InvokeRepeating("ChangeSprite", 0.5f, 0.5f);
    }

    void Update()
    {
        transform.rotation = Quaternion.identity;

        if (rigidbody != null)
        {
            if (rigidbody.velocity.x > 0.25f)
            {
                SpriteFacing = CharacterControl.Directions.Right;
            }
            else if (rigidbody.velocity.x < -0.25f)
            {
                SpriteFacing = CharacterControl.Directions.Left;
            }
        }
    }

    void SetSpriteOrientation()
    {
        switch (SpriteFacing)
        {
            case CharacterControl.Directions.Right:
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                break;

            case CharacterControl.Directions.Left:
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                break;
        }
    }

    void ChangeSprite()
    {
        currentlyAppliedSprite++;
        if (currentlyAppliedSprite >= Sprites.Length)
            currentlyAppliedSprite = 0;

        Renderer.sprite = Sprites[currentlyAppliedSprite];
    }

    bool _winking = false;
    public void WinkInColor(Color winkColor)
    {
        if (_winking)
            return;

        _winking = true;
        Renderer.color = winkColor;
        Invoke("WinkInColor_Off", 0.33f);
    }

    void WinkInColor_Off()
    {
        Renderer.color = Color.white;
        _winking = false;
    }

}

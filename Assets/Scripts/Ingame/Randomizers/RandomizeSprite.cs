using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomizeSprite : MonoBehaviour
{
    public List<Sprite> Sprites = new List<Sprite>();

    void Start()
    {
        SetRandomSprite();
        Destroy(this);
    }

    void SetRandomSprite()
    {
        GetComponent<SpriteRenderer>().sprite = Sprites[UnityEngine.Random.Range(0, Sprites.Count)];
    }
}

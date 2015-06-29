using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Randomizer_Deco : Randomizer
{
    public GameObject BaseObject;
    public List<Sprite> Sprites = new List<Sprite>();



    public override void Trigger()
    {
        GameObject spawnedObject = Spawn(BaseObject, transform.position);

        spawnedObject.GetComponent<SpriteRenderer>().sprite = PickRandomSprite();

        Terminate();
    }

    Sprite PickRandomSprite()
    {
        if (Sprites.Count > 0)
            return Sprites[UnityEngine.Random.Range(0, Sprites.Count)];

        return null;
    }
}

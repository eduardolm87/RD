using UnityEngine;
using System.Collections;

public class Food : Powerup
{
    public Sprite[] Sprites;

    public int StaminaCure = 3;


    void Start()
    {
        int _random = UnityEngine.Random.Range(0, Sprites.Length);

        GetComponent<SpriteRenderer>().sprite = Sprites[_random];
    }




    public override void Pickup()
    {
        GameManager.Instance.currentPlayer.Heal(StaminaCure);

        Disappear();
    }
}

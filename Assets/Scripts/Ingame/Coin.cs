using UnityEngine;
using System.Collections;

public class Coin : Powerup
{
    public int Quantity = 1;

    public override void Pickup()
    {
        GameManager.Instance.Run.ChangeMoney(Quantity);

        GameManager.Instance.SoundManager.Play("Coin");

        Disappear();
    }
}

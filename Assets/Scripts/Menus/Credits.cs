using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Credits : MonoBehaviour
{
    public List<Sprite> TinyFighters = new List<Sprite>();

    public Image TinyFighter1, TinyFighter2;

    void OnEnable()
    {
        SetupTinyfighters();
    }

    void SetupTinyfighters()
    {
        if (TinyFighters.Count < 2)
            return;

        int fighter1, fighter2;
        fighter1 = Random.Range(0, TinyFighters.Count);

        do
        {
            fighter2 = Random.Range(0, TinyFighters.Count);
        } while (fighter2 == fighter1);

        TinyFighter1.sprite = TinyFighters[fighter1];
        TinyFighter2.sprite = TinyFighters[fighter2];
    }

}

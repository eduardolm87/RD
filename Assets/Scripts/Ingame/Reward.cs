using UnityEngine;
using System.Collections;

[System.Serializable]
public class Reward
{
    public enum Chances
    {
        Never = 0,
        Rare = 1,
        Uncommon = 2,
        Common = 3,
        Often = 4,
        Always = 5
    }

    public GameObject Item;
    public Chances Chance = Chances.Always;
}

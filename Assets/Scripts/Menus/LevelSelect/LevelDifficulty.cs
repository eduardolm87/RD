using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelDifficulty : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> Tokens = new List<GameObject>();

    public void SetDifficulty(int q)
    {
        if (q < 0 || q >= Tokens.Count)
        {
            Debug.LogError("Error: Difficulty " + q + " is not valid.");
            return;
        }

        for (int i = 0; i < Tokens.Count; i++)
        {
            Tokens[i].SetActive(i < q);
        }
    }
}

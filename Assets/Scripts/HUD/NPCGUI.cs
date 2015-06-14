using UnityEngine;
using System.Collections;


public class NPCGUI : MonoBehaviour
{
    public GameObject HpPelletPrefab;


    public void ShowHP(int hp)
    {
        if (hp < 0 || hp > 20)
            return;

        int _children = transform.childCount;

        while (_children > hp)
        {
            Destroy(transform.GetChild(_children - 1).gameObject);
            _children--;
        }

        while (_children < hp)
        {
            GameObject _pellet = GameObject.Instantiate(HpPelletPrefab, transform.position, Quaternion.identity) as GameObject;
            _pellet.transform.parent = transform;
            _pellet.transform.localScale = HpPelletPrefab.transform.localScale;
            _children++;
        }
    }

}

using UnityEngine;
using System.Collections;


public class Building : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer Roof;

    public Color NormalColor = Color.white;
    public Color EnterColor = Color.clear;

    void Start()
    {
        Roof.color = NormalColor;
    }


    public void Enter()
    {
        Roof.color = EnterColor;
    }

    public void Exit()
    {
        Roof.color = NormalColor;
    }


}

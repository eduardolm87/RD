using System;
using System.Collections.Generic;

public enum Alteration
{
    NonPushable = 0,
    DamageResistant,
    NonstopAnimation,
    DamageWhenMoving,
    WaitMoreUntilAttackAgain,
    ClumsyAttack,
    Boss
};


[Serializable]
public class Attributes
{
    public List<Alteration> Alterations = new List<Alteration>();

    [UnityEngine.HideInInspector]
    public int HP = 3;
    public int HPmax = 3;

    public virtual void Restore()
    {
        HP = HPmax;
    }
}

[Serializable]
public class PlayerAttributes : Attributes
{
    public int Money = 0;

    public int Attack_max = 1;
    public float Impulse_max = 20;//16;
    public float Precission_max = 6;
    public int Stamina_max = 10;
    public int Defense_max = 0;


    [UnityEngine.HideInInspector]
    public int Attack;
    [UnityEngine.HideInInspector]
    public float Impulse;
    [UnityEngine.HideInInspector]
    public float Precission;
    [UnityEngine.HideInInspector]
    public int Stamina;
    [UnityEngine.HideInInspector]
    public int Defense;

    public PlayerAttributes()
    {
        Restore();
    }

    public override void Restore()
    {
        HP = HPmax;
        Attack = Attack_max;
        Impulse = Impulse_max;
        Precission = Precission_max;
        Stamina = Stamina_max;
    }
}

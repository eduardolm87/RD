using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using UnityEngine.UI;

public class Stage : MonoBehaviour
{
    public enum StageGoals
    {
        KillAllEnemies,
        GetItem,
        ReachToCertainPoint,
        KillCertainEnemy
    }

    public enum Frequencies
    {
        NEVER = 0,
        ALWAYS = 1,
        ALMOST_NEVER = 2,
        RARE = 4,
        COMMON = 8,
        VERY_COMMON = 12
    }


    public string Name;
    public Transform StartPoint;

    public Sprite Icon;
    [Range(1, 5)]
    public int Difficulty = 2;

    public int StageOrderFrom = 0;
    public int StageOrderTo = 10;
    public string VerboseString = "";
    public Frequencies Frequency = Frequencies.COMMON;

    public StageGoals Goal = StageGoals.KillAllEnemies;
    public GameObject ItemToGet = null;
    public Collider2D PlaceToReach = null;
    public Monster EnemyToKill = null;

    public AudioClip Music;


    [HideInInspector]
    public Dictionary<Transform, float> _previousIntertia = new Dictionary<Transform, float>();


    public int NumberOfLivingMonsters()
    {
        Monster[] _monsters = gameObject.GetComponentsInChildren<Monster>();
        if (_monsters.Length < 1) return 0;

        return _monsters.Count(x => x.Attributes.HP > 0);
    }

    public bool GetSwitchState(string ID)
    {
        Switch _switch = GetComponentsInChildren<Switch>().FirstOrDefault(x => x.ID == ID);
        return _switch ?? false;
    }

    public string VerboseGoal()
    {
        if (!string.IsNullOrEmpty(VerboseString))
            return VerboseString;

        switch (Goal)
        {
            case StageGoals.KillAllEnemies:
                return "Kill all enemies" + AppendNumberOfLivingMonsters();

            case StageGoals.GetItem:
                return "Get " + ItemToGet.name;

            case StageGoals.KillCertainEnemy:
                return "Kill the boss";

            case StageGoals.ReachToCertainPoint:
                return "Find the exit";
        }

        return "";
    }

    string AppendNumberOfLivingMonsters()
    {
        if (GameManager.Instance.currentPlayer != null)
            return " (" + NumberOfLivingMonsters() + " left)";
        else
            return "";
    }
}

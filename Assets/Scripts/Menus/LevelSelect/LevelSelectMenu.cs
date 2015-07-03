﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;


public class LevelSelectMenu : MonoBehaviour
{
    [SerializeField]
    public PCStats PCStats;

    [SerializeField]
    public Mapcontrol Map;

    public MapStageInfo StageInfo;

    private Stage CurrentStage = null;

    public void Open(Stage zStageToLoad = null)
    {
        gameObject.SetActive(true);

        if (zStageToLoad == null)
        {
            CurrentStage = GameManager.Instance.Run.GetLastUnlockedStage();
        }

        LoadCurrentStage();
    }

    void LoadCurrentStage()
    {
        PCStats.Load();
        Map.Open(CurrentStage);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void StartGame()
    {
        GameManager.Instance.StartCoroutine(GameManager.Instance.LoadStage(CurrentStage));
    }

    public void HomeButton()
    {
        GameManager.Instance.BackFromSelectToTitle();
    }

    public void SelectStage(Stage zStage)
    {
        CurrentStage = zStage;
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;


public class LevelSelectMenu : MonoBehaviour
{
    [SerializeField]
    public LevelTheatre Theatre;

    [SerializeField]
    public PCStats PCStats;


    private Stage CurrentStage = null;

    [HideInInspector]
    public int CurrentStageIndex = 0;

    bool AcceptInput { get { return !Theatre.Curtains.Busy; } }

    //void OnEnable()
    //{
    //    if (TitleScreen.TitleScreenShown)
    //        Open();
    //}

    public void Open(Stage zStageToLoad = null)
    {
        gameObject.SetActive(true);

        if (zStageToLoad == null)
        {
            CurrentStage = GameManager.Instance.Run.GetLastUnlockedStage();
        }

        CurrentStageIndex = GameManager.Instance.Run.GetStageIndex(CurrentStage);

        LoadCurrentStage();
    }

    void LoadCurrentStage()
    {
        PCStats.Load();

        StartCoroutine(Theatre.Load(CurrentStage, CurrentStageIndex));
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void StartGame()
    {
        if (!AcceptInput)
            return;

        GameManager.Instance.StartCoroutine(GameManager.Instance.LoadStage(CurrentStage));
    }

    public void ArrowFwd()
    {
        if (!AcceptInput)
            return;

        CurrentStageIndex++;

        if (CurrentStageIndex >= GameManager.Instance.Run.UnlockedStages.Count)
        {
            Debug.LogError("Error: Tried to load stage with index " + CurrentStageIndex);
            CurrentStageIndex--;
            return;
        }

        CurrentStage = GameManager.Instance.Run.UnlockedStages[CurrentStageIndex];
        LoadCurrentStage();
    }

    public void ArrowBck()
    {
        if (!AcceptInput)
            return;

        CurrentStageIndex--;

        if (CurrentStageIndex < 0)
        {
            Debug.LogError("Error: Tried to load stage with index " + CurrentStageIndex);
            CurrentStageIndex++;
            return;
        }

        CurrentStage = GameManager.Instance.Run.UnlockedStages[CurrentStageIndex];
        LoadCurrentStage();
    }

    public void HomeButton()
    {
        if (!AcceptInput)
            return;

        GameManager.Instance.BackFromSelectToTitle();
    }


}

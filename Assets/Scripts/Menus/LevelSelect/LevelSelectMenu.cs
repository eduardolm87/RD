using UnityEngine;
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

    private Stage CurrentStage = null;

    [HideInInspector]
    public int CurrentStageIndex = 0;


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
        Map.Open();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void StartGame()
    {
        GameManager.Instance.StartCoroutine(GameManager.Instance.LoadStage(CurrentStage));
    }

    public void ArrowFwd()
    {
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
        GameManager.Instance.BackFromSelectToTitle();
    }


}

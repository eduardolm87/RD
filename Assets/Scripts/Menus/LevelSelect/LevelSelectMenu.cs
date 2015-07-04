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

    public MapStageInfo StageInfo;

    private Stage CurrentStage = null;

    public bool InputAccepted
    {
        get
        {
            if (Map.PlayerNavigator.State != Mapnavigator.States.IDLE)
                return false;

            return true;
        }
    }

    public void Open(Stage zStageToLoad = null)
    {
        if (zStageToLoad == null)
        {
            CurrentStage = GameManager.Instance.Run.GetLastUnlockedStage();
        }
        else
        {
            CurrentStage = zStageToLoad;
        }

        GameManager.Instance.SoundManager.PlayMusic("Overworld01");

        gameObject.SetActive(true);

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
        if (!InputAccepted)
            return;

        GameManager.Instance.StartCoroutine(GameManager.Instance.LoadStage(CurrentStage));
    }

    public void HomeButton()
    {
        if (!InputAccepted)
            return;

        GameManager.Instance.BackFromSelectToTitle();
    }

    public void SelectStage(Stage zStage)
    {
        CurrentStage = zStage;
        GameManager.Instance.lastLoadedStagePrefab = zStage;
        GameManager.Instance.Progress.SaveLastVisitedStage();
    }
}

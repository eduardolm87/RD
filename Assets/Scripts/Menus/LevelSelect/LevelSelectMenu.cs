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

    public Text Money;

    public bool InputAccepted
    {
        get
        {
            if (Map.PlayerNavigator.State != Mapnavigator.States.IDLE)
                return false;

            if (Map.ShowingCinematic)
                return false;

            return true;
        }
    }

    public void Open(Stage zStageToLoad = null)
    {
        if (zStageToLoad == null)
        {
            CurrentStage = GameManager.Instance.Run.GetLastUnlockedStage();
            if (CurrentStage == null)
            {
                Debug.LogError("Error retrieving Current Stage");
            }
        }
        else
        {
            CurrentStage = zStageToLoad;
        }


        GameManager.Instance.SoundManager.PlayMusic("Overworld01");

        gameObject.SetActive(true);

        RefreshInfo();

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

        GameManager.Instance.SoundManager.Play("Confirm");

        GameManager.Instance.StartCoroutine(GameManager.Instance.LoadStage(CurrentStage));
    }

    public void HomeButton()
    {
        if (!InputAccepted)
            return;

        GameManager.Instance.SoundManager.Play("Cancel");

        GameManager.Instance.BackFromSelectToTitle();
    }

    public void SelectStage(Stage zStage)
    {
        CurrentStage = zStage;
        GameManager.Instance.lastLoadedStagePrefab = zStage;
        GameManager.Instance.Progress.SaveLastVisitedStage();
    }

    void RefreshInfo()
    {
        Money.text = GameManager.Instance.Run.Money.ToString();
    }
}

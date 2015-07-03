using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Mapcontrol : MonoBehaviour
{
    public Mapnavigator PlayerNavigator;

    public MapArrows NavigationInterface;




    private List<MapStep> MapSteps;
    bool Initialized = false;

    void Initialization()
    {
        MapSteps = GetComponentsInChildren<MapStep>().ToList();

        Initialized = true;
    }

    public void Open(Stage zStageToSet)
    {
        gameObject.SetActive(true);
        if (!Initialized)
        {
            Initialization();
        }

        PlayerNavigator.SetOnStep(GetMapstepFromStage(zStageToSet));
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void ShowStageInfo(MapStep zStep)
    {
        GameManager.Instance.LevelSelectMenu.StageInfo.LoadInfo(zStep.Stage);
    }

    public MapStep GetMapstepFromStage(Stage zStage)
    {
        return MapSteps.FirstOrDefault(s => s.Stage == zStage);
    }

}

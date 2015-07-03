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

        RefreshPaths();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void SelectStage(MapStep zStep)
    {
        GameManager.Instance.LevelSelectMenu.StageInfo.LoadInfo(zStep.Stage);
        GameManager.Instance.LevelSelectMenu.SelectStage(zStep.Stage);

        RefreshPaths();
        GameManager.Instance.LevelSelectMenu.Map.NavigationInterface.Show(PlayerNavigator.CurrentStep.GetAvailableDirections());
    }

    public MapStep GetMapstepFromStage(Stage zStage)
    {
        return MapSteps.FirstOrDefault(s => s.Stage == zStage);
    }

    public void RefreshPaths()
    {
        //Unlock maps
        List<MapStep> UnlockedLocations = MapSteps.Where(m => GameManager.Instance.Run.UnlockedStages.Contains(m.Stage)).ToList();
        List<MapStep> LockedLocations = MapSteps.Where(m => m.Stage != null && m.State != MapStep.Access.TRANSIT && !GameManager.Instance.Run.UnlockedStages.Contains(m.Stage)).ToList();
        UnlockedLocations.ForEach(map => map.State = MapStep.Access.VISITABLE);
        LockedLocations.ForEach(map => map.State = MapStep.Access.LOCKED);

        List<MapStep.Path> PathsUnlocked = new List<MapStep.Path>();
        foreach (MapStep mapStep in UnlockedLocations)
        {
            mapStep.Paths.ForEach(path =>
            {
                if (!PathsUnlocked.Contains(path))
                {

                    bool shouldUnlock = false;
                    switch (path.NextStep.State)
                    {
                        case MapStep.Access.VISITABLE:
                            shouldUnlock = true;
                            break;

                        case MapStep.Access.LOCKED:
                        case MapStep.Access.UNKNOWN:
                            shouldUnlock = false;
                            break;

                        case MapStep.Access.TRANSIT:
                            shouldUnlock = path.ThisPathArrivesToAVisitableLocation(mapStep);
                            break;
                    }

                    if (shouldUnlock)
                    {
                        path.Unlocked = true;
                        PathsUnlocked.Add(path);
                    }
                    else
                    {
                        path.Unlocked = false;

                    }
                }
            });
        }

        //Lock maps
        foreach (MapStep mapStep in LockedLocations)
        {
            mapStep.State = MapStep.Access.LOCKED;
            mapStep.Paths.ForEach(path =>
            {
                if (!PathsUnlocked.Contains(path))
                    path.Unlocked = false;
            });
        }
    }

}

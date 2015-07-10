using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Mapcontrol : MonoBehaviour
{
    public Mapnavigator PlayerNavigator;

    public Camera MapCamera;

    public MapArrows NavigationInterface;

    private List<MapStep> MapSteps;

    [HideInInspector]
    public Stage NewestStage = null;

    bool ShowNewCinematic = false;

    [SerializeField]
    private GameObject NewIndication;

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

        PlayerNavigator.Renderer.sprite = GameManager.Instance.Run.CurrentHero.Graphic;

        PlayerNavigator.SetOnStep(GetMapstepFromStage(zStageToSet));

        RefreshPaths();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void SelectStage(MapStep zStep)
    {
        GameManager.Instance.LevelSelectMenu.SelectStage(zStep.Stage);

        ShowNewCinematic = GameManager.Instance.Run.UnlockedStages.Count > 1 && NewestStage != GameManager.Instance.Run.GetLastUnlockedStage();

        NewestStage = GameManager.Instance.Run.GetLastUnlockedStage();

        RefreshPaths();

        StartCoroutine(ShowLatestMapStep(ShowNewCinematic));
    }

    public MapStep GetMapstepFromStage(Stage zStage)
    {
        MapStep map = MapSteps.FirstOrDefault(s => s.Stage == zStage);

        if (map == null)
        {
            Debug.LogError("Error in map: Stage " + zStage.name + " has not a location assigned.");
        }

        return map;
    }

    public void RefreshPaths()
    {
        //Unlock maps
        List<MapStep> UnlockedLocations = MapSteps.Where(m => GameManager.Instance.Run.UnlockedStages.Contains(m.Stage)).ToList();
        List<MapStep> LockedLocations = MapSteps.Where(m => m.Stage != null && m.State != MapStep.Access.TRANSIT && !GameManager.Instance.Run.UnlockedStages.Contains(m.Stage)).ToList();
        UnlockedLocations.ForEach(map =>
        {
            map.State = MapStep.Access.VISITABLE;
            if (!ShowNewCinematic || NewestStage != map.Stage)
                map.Show(0);
        });

        LockedLocations.ForEach(map =>
        {
            map.State = MapStep.Access.LOCKED;
            map.Hide();
        });

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

        //Transition mapsteps
        MapSteps.ForEach(step =>
        {
            if (step.State == MapStep.Access.TRANSIT)
            {

                if (step.Paths.Any(p => PathsUnlocked.Any(q => q.Graphic == p.Graphic)))
                {
                    step.Show(0);
                }
                else
                {
                    step.Hide();
                }
            }
        });
    }

    IEnumerator ShowLatestMapStep(bool showCinematic)
    {
        if (showCinematic)
        {
            if (NewestStage == null)
            {
                Debug.LogError("Error getting the newest stage.");
                yield break;
            }
            MapStep mapStep = MapSteps.FirstOrDefault(m => m.Stage == NewestStage);
            if (mapStep == null)
            {
                Debug.LogError("Error getting the newest stage in the map.");
            }

            GameManager.Instance.LevelSelectMenu.StageInfo.LoadInfo(NewestStage);

            mapStep.Hide();

            Vector3 MapStepPosition = new Vector3(mapStep.transform.position.x, mapStep.transform.position.y, MapCamera.transform.position.z);
            yield return StartCoroutine(MoveCameraToPoint(MapStepPosition, 0.5f));

            //Particles & stuff
            MapStepPosition.z = 0.25f;

            NewIndication.transform.position = MapStepPosition + Vector3.up;
            yield return null;

            NewIndication.SetActive(true);
            iTween.ScaleFrom(NewIndication, iTween.Hash("scale", Vector3.zero, "time", 0.5f));
            iTween.MoveTo(NewIndication, iTween.Hash("position", MapStepPosition + (Vector3.up * 2), "time", 0.5f));

            GameManager.Instance.Effects.Smoke(MapStepPosition);
            yield return new WaitForSeconds(0.1f);

            GameManager.Instance.SoundManager.Play("UnlockNewStage");

            mapStep.Show(0.66f);

            yield return new WaitForSeconds(1.5f);
            //


            MapStepPosition = new Vector3(PlayerNavigator.transform.position.x, PlayerNavigator.transform.position.y, MapCamera.transform.position.z);
            yield return StartCoroutine(MoveCameraToPoint(MapStepPosition, 0.5f));
        }

        yield return new WaitForSeconds(0.1f);

        NewIndication.SetActive(false);
        GameManager.Instance.LevelSelectMenu.StageInfo.LoadInfo(PlayerNavigator.CurrentStep.Stage);
        GameManager.Instance.LevelSelectMenu.Map.NavigationInterface.Show(PlayerNavigator.CurrentStep.GetAvailableDirections());
    }

    IEnumerator MoveCameraToPoint(Vector3 zPoint, float zTime)
    {
        iTween.MoveTo(MapCamera.gameObject, iTween.Hash("position", zPoint, "time", zTime, "easetype", iTween.EaseType.easeOutCubic));
        yield return new WaitForSeconds(zTime);
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Mapnavigator : MonoBehaviour
{
    public enum States { UNAVAILABLE, IDLE, MOVING };

    public States state = States.UNAVAILABLE;
    public States State
    {
        get { return state; }
        set
        {
            state = value;
            if (IsPlayer && CurrentStep != null && GameManager.Instance.LevelSelectMenu.Map.NavigationInterface != null)
            {
                if (state == States.IDLE)
                {
                    GameManager.Instance.LevelSelectMenu.Map.NavigationInterface.Show(CurrentStep.GetAvailableDirections());
                    if (CurrentStep.State == MapStep.Access.VISITABLE)
                    {
                        GameManager.Instance.LevelSelectMenu.StageInfo.ShowControls();
                    }
                }
                else
                {
                    GameManager.Instance.LevelSelectMenu.Map.NavigationInterface.Hide();
                    GameManager.Instance.LevelSelectMenu.StageInfo.HideControls();
                }
            }
        }
    }

    public bool IsPlayer { get { return GameManager.Instance.LevelSelectMenu.Map.PlayerNavigator == this; } }


    public SpriteRenderer Renderer;

    private Vector3 originalScale;

    [HideInInspector]
    public MapStep CurrentStep = null;

    void Awake()
    {
        originalScale = transform.localScale;
    }

    public void SetOnStep(MapStep zStep)
    {
        if (zStep == null)
        {
            Debug.LogError("Error: Location not found");
            return;
        }

        float z = transform.position.z;
        transform.position = new Vector3(zStep.transform.position.x, zStep.transform.position.y + Renderer.bounds.size.y / 2, z);

        CurrentStep = zStep;
        State = States.IDLE;

        if (IsPlayer && CurrentStep.State == MapStep.Access.VISITABLE)
        {
            GameManager.Instance.LevelSelectMenu.Map.SelectStage(CurrentStep);
        }
    }

    public void TryGo(MapStep.Directions zDirection)
    {
        if (State != States.IDLE)
        {
            return;
        }

        StartCoroutine(TryGoCoroutine(zDirection));
    }

    IEnumerator TryGoCoroutine(MapStep.Directions zDirection)
    {
        MapStep nextStep = null, previousStep = null;
        MapStep.Directions CurrentDirection = zDirection;

        State = States.MOVING;

        int steps = 0;



        do
        {
            steps++;
            SoundDependingOnStep(steps);

            nextStep = GetNextMapStep(CurrentStep, previousStep, CurrentDirection);

            if (nextStep == null)
            {
                break;
            }

            if (CurrentStep.transform.position.x < nextStep.transform.position.x - 1)
                Reorientate(MapStep.Directions.LEFT);
            else if (CurrentStep.transform.position.x > nextStep.transform.position.x + 1)
                Reorientate(MapStep.Directions.RIGHT);

            yield return StartCoroutine(MoveToStep(nextStep));

            previousStep = CurrentStep;
            CurrentStep = nextStep;
            CurrentDirection = MapStep.Directions.NONE;

        } while (CurrentStep.State == MapStep.Access.TRANSIT);

        State = States.IDLE;

        if (IsPlayer && CurrentStep.State == MapStep.Access.VISITABLE)
        {
            GameManager.Instance.LevelSelectMenu.Map.SelectStage(CurrentStep);
        }
    }

    void SoundDependingOnStep(int zSteps)
    {
        if (zSteps < 2)
        {
            GameManager.Instance.SoundManager.Play("OverworldStartMoving");
        }
        else
        {
            GameManager.Instance.SoundManager.Play("MoveOverworld");
        }
    }

    MapStep GetNextMapStep(MapStep zOrigin, MapStep zPrevious, MapStep.Directions zCurrentDirection)
    {
        if (zOrigin.Paths.Count < 1)
        {
            return null;
        }

        List<MapStep.Path> Paths = zOrigin.Paths.Where(p =>
        {
            if (p.NextStep == zPrevious)
            {
                return false;
            }

            if (zCurrentDirection != MapStep.Directions.NONE)
            {
                if (p.Direction != zCurrentDirection)
                {
                    return false;
                }
            }

            if (p.NextStep.State != MapStep.Access.TRANSIT && p.NextStep.State != MapStep.Access.VISITABLE)
                return false;

            return true;
        }).ToList();

        if (Paths.Count == 1)
        {
            return Paths[0].NextStep;
        }

        return null;
    }

    IEnumerator MoveToStep(MapStep zTarget)
    {
        float time = 1;


        Vector3 positionOfNode = new Vector3(zTarget.transform.position.x, zTarget.transform.position.y + Renderer.bounds.size.y / 2, transform.position.z);

        iTween.MoveTo(gameObject, iTween.Hash("position", positionOfNode, "time", time, "easetype", iTween.EaseType.easeInOutCubic));
        yield return new WaitForSeconds(time);
    }

    public void Reorientate(MapStep.Directions zDirection)
    {
        switch (zDirection)
        {
            case MapStep.Directions.RIGHT:
                Renderer.transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
                break;

            case MapStep.Directions.LEFT:
                Renderer.transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
                break;
        }
    }

}

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

    [HideInInspector]
    public MapStep CurrentStep = null;

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

        do
        {
            nextStep = GetNextMapStep(CurrentStep, previousStep, CurrentDirection);

            if (nextStep == null)
            {
                break;
            }

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

    //Debug: mejor leer de arrows
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            TryGo(MapStep.Directions.DOWN);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            TryGo(MapStep.Directions.UP);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            TryGo(MapStep.Directions.LEFT);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            TryGo(MapStep.Directions.RIGHT);
        }
    }
}

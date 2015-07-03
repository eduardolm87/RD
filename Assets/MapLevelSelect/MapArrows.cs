using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class MapArrows : MonoBehaviour
{
    public GameObject Down, Left, Up, Right;

    public void Show(List<MapStep.Directions> zDirections)
    {
        gameObject.SetActive(true);
        SetDirections(zDirections);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetDirections(List<MapStep.Directions> zDirections)
    {
        Down.SetActive(zDirections.Contains(MapStep.Directions.DOWN));
        Up.SetActive(zDirections.Contains(MapStep.Directions.UP));
        Left.SetActive(zDirections.Contains(MapStep.Directions.LEFT));
        Right.SetActive(zDirections.Contains(MapStep.Directions.RIGHT));
    }


    public void ArrowButton_Left()
    {
        if (!GameManager.Instance.LevelSelectMenu.InputAccepted)
            return;

        GameManager.Instance.LevelSelectMenu.Map.PlayerNavigator.TryGo(MapStep.Directions.LEFT);
    }

    public void ArrowButton_Right()
    {
        if (!GameManager.Instance.LevelSelectMenu.InputAccepted)
            return;

        GameManager.Instance.LevelSelectMenu.Map.PlayerNavigator.TryGo(MapStep.Directions.RIGHT);
    }

    public void ArrowButton_Up()
    {
        if (!GameManager.Instance.LevelSelectMenu.InputAccepted)
            return;

        GameManager.Instance.LevelSelectMenu.Map.PlayerNavigator.TryGo(MapStep.Directions.UP);
    }

    public void ArrowButton_Down()
    {
        if (!GameManager.Instance.LevelSelectMenu.InputAccepted)
            return;

        GameManager.Instance.LevelSelectMenu.Map.PlayerNavigator.TryGo(MapStep.Directions.DOWN);
    }
}

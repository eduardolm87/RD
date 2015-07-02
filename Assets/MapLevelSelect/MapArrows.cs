using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MapArrows : MonoBehaviour
{
    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }


    public void ArrowButton_Left()
    {
        GameManager.Instance.LevelSelectMenu.Map.PlayerNavigator.TryGo(MapStep.Directions.LEFT);
    }

    public void ArrowButton_Right()
    {
        GameManager.Instance.LevelSelectMenu.Map.PlayerNavigator.TryGo(MapStep.Directions.RIGHT);
    }

    public void ArrowButton_Up()
    {
        GameManager.Instance.LevelSelectMenu.Map.PlayerNavigator.TryGo(MapStep.Directions.UP);
    }

    public void ArrowButton_Down()
    {
        GameManager.Instance.LevelSelectMenu.Map.PlayerNavigator.TryGo(MapStep.Directions.DOWN);
    }
}

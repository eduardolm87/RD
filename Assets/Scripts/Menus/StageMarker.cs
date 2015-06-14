using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StageMarker : MonoBehaviour
{
    public Text StageName;

    public Text BestTime;

    public Stage Stage;




    public void OnMouseDown()
    {
        GameManager.Instance.StartCoroutine(GameManager.Instance.LoadStage(Stage));
    }

    public void RefreshWithStageData(SaveManager.StageInfo info)
    {
        Stage = info.Stage;

        StageName.text = info.Stage.Name;

        if (info.BestTimeInSeconds > 0)
        {
            BestTime.text = "Best time: " + GameManager.SecondsToTime(info.BestTimeInSeconds);
        }
        else
        {
            BestTime.text = "";
        }
    }
}

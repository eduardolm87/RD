using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class MapStageInfo : MonoBehaviour
{
    [SerializeField]
    private GameObject BottomBar;

    [SerializeField]
    private Text StageName;

    [SerializeField]
    private LevelDifficulty StageDifficulty;

    [SerializeField]
    private Button GoButton;

    public void LoadInfo(Stage zStage)
    {
        if (zStage == null)
        {
            Debug.LogError("Error loading Stage!");
            return;
        }

        StageName.text = zStage.Name;
        StageDifficulty.SetDifficulty(zStage.Difficulty);
    }

    public void ShowControls()
    {
        GoButton.gameObject.SetActive(true);
        StageDifficulty.gameObject.SetActive(true);

        GoButton.transform.localScale = Vector3.one;
        iTween.ScaleFrom(GoButton.gameObject, iTween.Hash("scale", Vector3.zero, "delay", 0.1f, "time", 0.25f, "easetype", iTween.EaseType.easeOutBounce));
        BottomBar.SetActive(true);

        BottomBar.transform.localScale = Vector3.one;
        iTween.ScaleFrom(BottomBar, iTween.Hash("y", 0, "time", 0.1f, "easetype", iTween.EaseType.easeOutBounce));

        StageName.enabled = true;
    }

    public void HideControls()
    {
        GoButton.gameObject.SetActive(false);
        StageDifficulty.gameObject.SetActive(false);

        BottomBar.SetActive(false);

        StageName.enabled = false;
    }
}

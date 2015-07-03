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
    private GameObject HeaderBar;

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

        //GoButton.transform.localScale = Vector3.one;
        //iTween.ScaleFrom(GoButton.gameObject, iTween.Hash("scale", Vector3.zero, "delay", 0.1f, "time", 0.25f, "easetype", iTween.EaseType.easeOutBounce));

        BottomBar.SetActive(true);
        BottomBar.transform.localScale = Vector3.one;
        iTween.ScaleFrom(BottomBar, iTween.Hash("y", 0, "time", 0.25f, "easetype", iTween.EaseType.easeOutBounce));

        HeaderBar.SetActive(true);
        HeaderBar.transform.localScale = Vector3.one;
        iTween.ScaleFrom(HeaderBar, iTween.Hash("y", 0, "time", 0.25f, "easetype", iTween.EaseType.easeOutBounce));

        StageName.enabled = true;
    }

    bool busy = false;
    public void HideControls()
    {
        if (busy)
            return;

        iTween.ScaleTo(BottomBar, iTween.Hash("y", 0, "time", 0.25f, "easetype", iTween.EaseType.easeOutBounce));
        iTween.ScaleTo(HeaderBar, iTween.Hash("y", 0, "time", 0.25f, "easetype", iTween.EaseType.easeOutBounce));

        busy = true;
        Invoke("EndOfHideControls", 0.25f);
    }

    void EndOfHideControls()
    {
        GoButton.gameObject.SetActive(false);
        StageDifficulty.gameObject.SetActive(false);

        BottomBar.SetActive(false);

        HeaderBar.SetActive(false);

        StageName.enabled = false;

        busy = false;
    }
}

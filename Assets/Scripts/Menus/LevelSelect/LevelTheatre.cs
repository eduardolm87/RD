using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class LevelTheatre : MonoBehaviour
{
    [SerializeField]
    private Image StageIcon;

    [SerializeField]
    private Text StageName;

    [SerializeField]
    private LevelDifficulty StageDifficulty;

    [SerializeField]
    private GameObject ArrowForward, ArrowBack;



    public void Load(Stage zStage, int zIndex)
    {
        StageName.text = zStage.Name;
        StageIcon.sprite = zStage.Icon;
        StageDifficulty.SetDifficulty(zStage.Difficulty);
        SetArrows(zIndex);
    }

    void SetArrows(int zIndex)
    {
        int numberOfStages = GameManager.Instance.Run.UnlockedStages.Count;

        ArrowForward.SetActive((zIndex < numberOfStages - 1));

        ArrowBack.SetActive(zIndex > 0);
    }



}

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

    [SerializeField]
    private Text StageGoal;

    [SerializeField]
    private Text NewIcon;

    int previousStageIndex = -1;


    public TheatreCurtain Curtains;

    public IEnumerator Load(Stage zStage, int zIndex)
    {
        if (Curtains.Busy)
        {
            yield break;
        }

        Curtains.Busy = true;

        if (!Curtains.Closed)
        {
            Curtains.Close(GetDirectionOfClose(previousStageIndex, zIndex));
            yield return new WaitForSeconds(Curtains.MaxTime);
        }

        StageName.text = "Level " + zIndex.ToString() + " - " + zStage.Name;
        StageIcon.sprite = zStage.Icon;
        StageGoal.text = zStage.VerboseGoal();
        StageDifficulty.SetDifficulty(zStage.Difficulty);
        SetArrows(zIndex);

        NewIcon.gameObject.SetActive(zIndex == GameManager.Instance.Run.UnlockedStages.Count - 1);

        yield return null;

        if (Curtains.Closed)
        {
            Curtains.Open(GetDirectionOfOpen(previousStageIndex, zIndex));
            yield return new WaitForSeconds(Curtains.MaxTime);
        }

        previousStageIndex = zIndex;
        Curtains.Busy = false;
    }

    int GetDirectionOfOpen(int zOldIndex, int zNewIndex)
    {
        if (zOldIndex < zNewIndex)
        {
            return -1;
        }
        else if (zOldIndex > zNewIndex)
        {
            return 1;
        }

        return 0;
    }

    int GetDirectionOfClose(int zOldIndex, int zNewIndex)
    {
        if (zOldIndex < zNewIndex)
        {
            return 1;
        }
        else if (zOldIndex > zNewIndex)
        {
            return -1;
        }

        return 0;
    }


    void SetArrows(int zIndex)
    {
        int numberOfStages = GameManager.Instance.Run.UnlockedStages.Count;

        ArrowForward.SetActive((zIndex < numberOfStages - 1));

        ArrowBack.SetActive(zIndex > 0);
    }



}

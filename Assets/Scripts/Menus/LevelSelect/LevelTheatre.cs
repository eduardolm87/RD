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
            Curtains.Close();
            yield return new WaitForSeconds(Curtains.MovementTime);
        }

        StageName.text = "Level " + zIndex.ToString() + " - " + zStage.Name;
        StageIcon.sprite = zStage.Icon;
        StageGoal.text = zStage.VerboseGoal();
        StageDifficulty.SetDifficulty(zStage.Difficulty);
        SetArrows(zIndex);

        NewIcon.gameObject.SetActive(zIndex == GameManager.Instance.Run.UnlockedStages.Count - 1);


        yield return null;

        yield return new WaitForSeconds(0.25f);

        if (Curtains.Closed)
        {
            Curtains.Open();
            yield return new WaitForSeconds(Curtains.MovementTime);
        }

        Curtains.Busy = false;
    }


    void SetArrows(int zIndex)
    {
        int numberOfStages = GameManager.Instance.Run.UnlockedStages.Count;

        ArrowForward.SetActive((zIndex < numberOfStages - 1));

        ArrowBack.SetActive(zIndex > 0);
    }



}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    public static bool Busy = false;

    public static bool TitleScreenShown = false;

    public AudioClip MusicClip;

    public Animator Intro;

    public List<iTweenEvent> Tweens = new List<iTweenEvent>();

    bool AnimationEnded = false;

    float TimeStart = 0;

    void Start()
    {
        MemorizeTweens();
        TitleScreenShown = true;
    }

    void OnEnable()
    {
        Busy = false;
        TimeStart = Time.time;
    }


    void Play()
    {
        Busy = false;
        GameManager.Instance.OpenStageSelect();
    }

    void Options()
    {
        Busy = false;
        GameManager.Instance.ChangeWindow(GameManager.Instance.OptionsWindow);
    }

    void Credits()
    {
        Busy = false;
        GameManager.Instance.ChangeWindow(GameManager.Instance.CreditsWindow);
    }

    public void PlayButton()
    {
        Play();


        if (Busy)
            return;

        Busy = true;

        if (!AnimationEnded && (Time.time - TimeStart < 2))
        {
            Intro.StopPlayback();
            Intro.enabled = false;
            AnimationEnded = true;
            StopTweens();
            Busy = false;

            //Destroy(Intro);
        }
        else
        {
            Play();
        }
    }

    public void OptionsButton()
    {
        if (Busy)
            return;

        Options();
    }

    public void CreditsButton()
    {
        if (Busy)
            return;

        Credits();
    }


    List<Vector3> TweenPositions = new List<Vector3>();



    void MemorizeTweens()
    {
        TweenPositions.Clear();
        Tweens.ForEach(t =>
        {
            TweenPositions.Add(t.transform.position);
        });
    }

    void StopTweens()
    {
        for (int i = 0; i < Tweens.Count; i++)
        {
            if (TweenPositions.Count <= i)
            {
                Debug.LogError("Error stopping tweens");
                return;
            }

            Tweens[i].Stop();
            Tweens[i].transform.position = TweenPositions[i];
        }
    }

}

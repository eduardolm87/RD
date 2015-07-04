using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{

    public static bool TitleScreenShown = false;

    public AudioClip MusicClip;


    void Play()
    {
        GameManager.Instance.SoundManager.Play("Confirm");
        GameManager.Instance.OpenStageSelect();
    }

    void Options()
    {
        GameManager.Instance.SoundManager.Play("Confirm");
        GameManager.Instance.ChangeWindow(GameManager.Instance.OptionsWindow);
    }

    void Credits()
    {
        GameManager.Instance.SoundManager.Play("Confirm");
        GameManager.Instance.ChangeWindow(GameManager.Instance.CreditsWindow);
    }

    public void PlayButton()
    {
        Play();
    }

    public void OptionsButton()
    {
        Options();
    }

    public void CreditsButton()
    {
        Credits();
    }
}

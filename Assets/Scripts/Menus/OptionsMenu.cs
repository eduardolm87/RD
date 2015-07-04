using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Toggle ToggleMusic;
    public Toggle ToggleSFX;


    void OnEnable()
    {
        LoadOptions();
    }

    public void LoadOptions()
    {
        ToggleMusic.isOn = !GameManager.Instance.MuteMusic;
        ToggleSFX.isOn = !GameManager.Instance.MuteSFX;
    }


    public void Button_DeleteAllData()
    {
        GameManager.Instance.SoundManager.Play("Confirm");

        GameManager.Instance.GamePopup.Show("Really delete all saved data?", new PopupButton[] { 
            new PopupButton("Delete", () => 
            {
                GameManager.Instance.SoundManager.Play("Confirm");

                GameManager.Instance.Progress.ResetAll();

                while (GameManager.Instance.StageList.childCount > 0)
                {
                    DestroyImmediate(GameManager.Instance.StageList.GetChild(0).gameObject);
                }

                GameManager.Instance.Progress.SaveProgress();

                GameManager.Instance.Start();
            }), 
            new PopupButton("Cancel", () => 
            {
                //do nothing
                GameManager.Instance.SoundManager.Play("Cancel");
            }) });
    }

    public void Toggle_Music(bool set)
    {
        GameManager.Instance.SoundManager.Play("Confirm");

        GameManager.Instance.MuteMusic = !set;

        if (!set)
            GameManager.Instance.SoundManager.StopMusic();
        else
            GameManager.Instance.SoundManager.PlayMusic(GameManager.Instance.TitleScreenWindow.GetComponent<TitleScreen>().MusicClip.name);

        GameManager.Instance.Progress.SaveSettings();
    }

    public void Toggle_SFX(bool set)
    {
        GameManager.Instance.MuteSFX = !set;

        GameManager.Instance.SoundManager.Play("Confirm");

        GameManager.Instance.Progress.SaveSettings();
    }
}

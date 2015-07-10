using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Curtain : MonoBehaviour
{
    public Text Text;
    public Image Image;
    public float Duration = 0.5f;

    public void Show()
    {
        Activate();
        Text.text = "";
        Image.CrossFadeAlpha(1, Duration, false);
    }

    public void Hide()
    {
        Text.CrossFadeAlpha(0, Duration, false);
        Image.CrossFadeAlpha(0, Duration, false);
        Invoke("Deactivate", Duration);
    }

    void Activate()
    {
        Image.enabled = true;
        Text.enabled = true;
        gameObject.SetActive(true);
    }

    void Deactivate()
    {
        Image.enabled = false;
        Text.enabled = false;
        gameObject.SetActive(false);
    }
}

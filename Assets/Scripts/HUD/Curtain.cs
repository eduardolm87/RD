using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Curtain : MonoBehaviour
{
    public static Curtain Instance = null;

    public Text Text;
    public Image Image;
    public float Duration = 0.5f;

    void Awake()
    {
        Instance = this;
        Image.CrossFadeAlpha(0, 0, true);
        Deactivate();
    }

    public void Show()
    {
        Text.text = "";
        Activate();
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

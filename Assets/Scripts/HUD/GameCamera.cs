using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour
{
    Vector3 originalLocalPosition = new Vector3(0, 0, -10);
    public static GameCamera Instance = null;

    void Awake()
    {
        Instance = this;
        originalLocalPosition = transform.localPosition;
    }

    public void Shake(float duration = 0.1f)
    {
        iTween.ShakePosition(gameObject, Vector3.one * 0.1f, duration);
        Invoke("Restore", duration + 0.01f);
    }


    public void Restore()
    {
        if (GameManager.Instance.currentPlayer.Dead)
            return;

        transform.localPosition = originalLocalPosition;
        CancelInvoke();
    }
}

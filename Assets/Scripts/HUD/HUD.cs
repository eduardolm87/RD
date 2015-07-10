using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Image StaminaBar;
    public Image AccumulatedPowerBar;

    public Text Name;
    public Text Speed;
    public Text Attack;
    //public Text Defense;

    public Image Portrait;

    void Update()
    {
        UpdatePowerBar();
    }

    public void RefreshHUD(PlayerAttributes attributes)
    {
        //Level.text = GameManager.Instance.currentlyLoadedStage.Name;

        StaminaBar.fillAmount = attributes.Stamina * 1f / attributes.Stamina_max;

        Speed.text = Mathf.FloorToInt(GameManager.Instance.currentPlayer.Attributes.Impulse).ToString();
        Attack.text = GameManager.Instance.currentPlayer.Attributes.Attack.ToString();
        //Defense.text = GameManager.Instance.currentPlayer.Attributes.Defense.ToString();

        Name.text = GameManager.Instance.Run.CurrentHero.Name.ToUpper();

        Portrait.sprite = GameManager.Instance.Run.CurrentHero.Graphic;
    }

    void UpdatePowerBar()
    {
        if (GameManager.Instance.currentPlayer.AccumulatedPower > 0 && GameManager.Instance.currentPlayer.Mode == Player.Modes.AIMING)
        {
            if (Player.aimingFrom == null || Player.aimingTo == null)
                return;

            AccumulatedPowerBar.enabled = true;
            AccumulatedPowerBar.fillAmount = GameManager.Instance.currentPlayer.AccumulatedPower;
            AccumulatedPowerBar.transform.position = Camera.main.WorldToScreenPoint(GameManager.Instance.currentPlayer.transform.position);

            //Rotate sprite towards finger
            Vector3 _to = Player.aimingTo.Value; //Input.mousePosition;
            Vector3 _from = Player.aimingFrom.Value;//Camera.main.WorldToScreenPoint(GameManager.Instance.currentPlayer.transform.position);

            Vector3 moveDirection = _to - _from;
            moveDirection.z = 0;
            moveDirection.Normalize();

            float targetAngle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg - 90;
            AccumulatedPowerBar.transform.rotation = Quaternion.Euler(0, 0, targetAngle);
        }
        else if (AccumulatedPowerBar.enabled)
        {
            AccumulatedPowerBar.enabled = false;
        }
    }

    public void PauseButton()
    {
        if (!GameManager.Instance.GamePaused)
            GameManager.Instance.currentPlayer.pressedPauseButton = true;
    }

}

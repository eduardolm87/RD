using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;


public class PCStats : MonoBehaviour
{
    [SerializeField]
    private Image CharacterIcon;

    [SerializeField]
    private Text Name, HP, ATK, AGI;

    [SerializeField]
    private PCUpgrades PCUpgrades;

    public void Load()
    {
        CharacterIcon.sprite = GameManager.Instance.Run.CurrentHero.Graphic;

        Name.text = GameManager.Instance.Run.CurrentHero.Name.ToUpper();
        HP.text = GameManager.Instance.Run.CurrentHero.Attributes.HPmax.ToString();
        ATK.text = GameManager.Instance.Run.CurrentHero.Attributes.Attack_max.ToString();
        AGI.text = GameManager.Instance.Run.CurrentHero.Attributes.Impulse_max.ToString();
    }
}

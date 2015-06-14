using UnityEngine;
using System.Collections;

public class TalkingNPC : MonoBehaviour
{

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject == GameManager.Instance.currentPlayer.gameObject)
        {
            GameManager.Instance.GamePopup.Show("Thanks for playing! Be prepared for new updates coming soon.", new PopupButton[] { new PopupButton("Ok", () => 
        { 
            //do nothing;
        }) 
            });
        }
    }

}

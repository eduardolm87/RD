using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.Linq;

public class PopupButton
{
    public string Text = "Ok";
    public Action Action = null;

    public PopupButton(string nText, Action nAction)
    {
        Text = nText;
        Action = nAction;
    }
}

public class GamePopup : MonoBehaviour
{
    public Button PopupButtonPrefab;
    public Text Message;
    public List<Button> MessageButtons = new List<Button>();
    List<PopupButton> ButtonsData = new List<PopupButton>();

    //Usage:
    /*
     * Show("Message",new PopupButton[] { 
     * new PopupButton("Button #1 text", () => { Action #1 }), 
     * new PopupButton("Button #2 text", Action #2) });
     */
    public void Show(string MessageText, PopupButton[] Buttons)
    {
        gameObject.SetActive(true);

        Time.timeScale = 0;

        Message.text = MessageText;

        ButtonsData = Buttons.ToList();

        for (int i = 0; i < MessageButtons.Count; i++)
        {
            if (i < Buttons.Length)
            {
                MessageButtons[i].gameObject.SetActive(true);

                Text _bText = MessageButtons[i].GetComponentInChildren<Text>();
                _bText.text = Buttons[i].Text;
            }
            else
            {
                MessageButtons[i].gameObject.SetActive(false);
            }
        }

    }

    public void Close()
    {
        Time.timeScale = 1;
        ButtonsData.Clear();
        gameObject.SetActive(false);
    }

    public void OnClick(Button which)
    {
        int _i = MessageButtons.IndexOf(which);
        if (_i > -1)
        {
            if (ButtonsData[_i].Action != null)
                ButtonsData[_i].Action();
        }

        Close();
    }

}

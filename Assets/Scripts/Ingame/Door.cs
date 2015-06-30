using UnityEngine;
using System.Collections;
using System;

public class Door : MonoBehaviour
{
    [Serializable]
    public class Condition
    {
        public Switch Switch;
        public bool Reverse = false;
    }

    public SpriteRenderer Renderer;
    public Sprite OpenSprite, CloseSprite;
    public bool Opened = false;
    public Condition[] Conditions;


    void Start()
    {
        InvokeRepeating("CheckStatus", 0, 0.1f);
    }

    void CheckStatus()
    {
        foreach (Condition _cond in Conditions)
        {
            if (_cond.Switch != null)
            {
                if (_cond.Switch.State != Opened)
                {
                    Open(_cond.Switch.State);
                }
            }
        }
    }

    public void ForceState(bool _open)
    {
        Open(_open);
        Conditions = null;
    }

    public void Open(bool _open)
    {
        //Closing check
        if (!_open)
        {
            RaycastHit2D _obstacle = Physics2D.CircleCast(transform.position, Renderer.bounds.size.x / 2f, Vector2.zero);
            if (_obstacle.collider != null)
            {
                return;
            }
        }

        //Do the operation
        Opened = _open;
        Renderer.sprite = Opened ? OpenSprite : CloseSprite;
        GetComponent<Collider2D>().enabled = !Opened;
        GameManager.Instance.SoundManager.Play("DoorOpenClose");
    }

}

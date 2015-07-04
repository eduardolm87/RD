using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class FloatingDmgText : MonoBehaviour
{
    public Text Text;

    public void EndOfTransition()
    {
        Destroy(gameObject);
    }
}

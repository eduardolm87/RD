using UnityEngine;
using System.Collections;

public class UtilityScript : MonoBehaviour
{
    public Color ColorToApply;
    public bool ApplyColor = false;

    public Material NewSpriteRenderer;
    public bool ApplyRenderer = false;

    void OnValidate()
    {
        //ApplyColorToChildren();    
        ApplySpriteRendererToChildren();
    }


    void ApplyColorToChildren()
    {
        if (ApplyColor)
        {
            SpriteRenderer[] _renderers = GetComponentsInChildren<SpriteRenderer>();

            foreach (var _r in _renderers)
            {
                _r.color = ColorToApply;
            }

            Debug.Log("Color aplicado.");

            ApplyColor = false;
        }
    }

    void ApplySpriteRendererToChildren()
    {
        if (!ApplyRenderer)
            return;

        if (NewSpriteRenderer == null)
        {
            Debug.LogError("FAIL: No new renderer");
            ApplyRenderer = false;
            return;
        }

        SpriteRenderer[] _renderers = GetComponentsInChildren<SpriteRenderer>();

        Debug.Log("Applying new Renderer to " + _renderers.Length + " objects.");

        foreach (SpriteRenderer _rend in _renderers)
        {
            _rend.material = NewSpriteRenderer;
        }

        Debug.Log("Finished");

        ApplyRenderer = false;
    }
}

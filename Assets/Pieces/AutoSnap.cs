using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode]
public class AutoSnap : MonoBehaviour
{
    public bool doFix = false;
    public bool doSnap = false;

    public AutoAdjustTile Walls = new AutoAdjustTile();


    private float snapValue = 1;
    float depth = 0;




    void Update()
    {
        if (doFix)
        {
            doFix = false;
            FixAll();
        }

        if (doSnap)
        {
            doSnap = false;
            SnapAll();
        }
    }

    void SnapAll()
    {
        transform.localPosition = Vector3.zero;

        foreach (Transform transformChild in transform)
        {
            Snap(transformChild);
        }

    }

    void FixAll()
    {
        transform.localPosition = Vector3.zero;

        foreach (Transform transformChild in transform)
        {
            Fix(transformChild.gameObject);
        }
    }


    private void Fix(GameObject zTile)
    {
        Snap(zTile.transform);

        SetRightGraphic(zTile.GetComponent<SpriteRenderer>());
    }

    void SetRightGraphic(SpriteRenderer zRenderer)
    {
        if (zRenderer == null)
            return;

        if (Walls.Contains(zRenderer.sprite))
        {

        }

    }

    private void Snap(Transform zTransform)
    {
        Vector3 t = zTransform.transform.position;

        t.x = Round(t.x);
        t.y = Round(t.y);
        t.z = depth;

        zTransform.position = t;
    }

    private float Round(float input)
    {
        return snapValue * Mathf.Round((input / snapValue));
    }
}

[System.Serializable]
public class AutoAdjustTile
{
    public string Name = "";
    public Sprite TopLeft;
    public Sprite TopMiddle;
    public Sprite TopRight;
    public Sprite CenterLeft;
    public Sprite CenterMiddle;
    public Sprite CenterRight;
    public Sprite BottomLeft;
    public Sprite BottomMiddle;
    public Sprite BottomRight;

    public bool Contains(Sprite zSprite)
    {
        if (zSprite == TopLeft) return true;
        if (zSprite == TopMiddle) return true;
        if (zSprite == TopRight) return true;
        if (zSprite == CenterLeft) return true;
        if (zSprite == CenterMiddle) return true;
        if (zSprite == CenterRight) return true;
        if (zSprite == BottomLeft) return true;
        if (zSprite == BottomMiddle) return true;
        if (zSprite == BottomRight) return true;

        return false;
    }

    public enum Position { None, TopLeft, TopMiddle, TopRight, CenterLeft, CenterMiddle, CenterRight, BottomLeft, BottomMiddle, BottomRight };

    public Position GetPositionGraphic(Sprite zSprite)
    {
        if (zSprite == TopLeft) return Position.TopLeft;
        if (zSprite == TopMiddle) return Position.TopMiddle;
        if (zSprite == TopRight) return Position.TopRight;
        if (zSprite == CenterLeft) return Position.CenterLeft;
        if (zSprite == CenterMiddle) return Position.CenterMiddle;
        if (zSprite == CenterRight) return Position.CenterRight;
        if (zSprite == BottomLeft) return Position.BottomLeft;
        if (zSprite == BottomMiddle) return Position.BottomMiddle;
        if (zSprite == BottomRight) return Position.BottomRight;

        return Position.None;
    }

    public Sprite GetPart(Position zPart)
    {
        switch (zPart)
        {
            case Position.TopLeft: return TopLeft;
            case Position.TopMiddle: return TopMiddle;
            case Position.TopRight: return TopRight;
            case Position.CenterLeft: return CenterLeft;
            case Position.CenterMiddle: return CenterMiddle;
            case Position.CenterRight: return CenterRight;
            case Position.BottomLeft: return BottomLeft;
            case Position.BottomMiddle: return BottomMiddle;
            case Position.BottomRight: return BottomRight;
        }

        return null;
    }

}
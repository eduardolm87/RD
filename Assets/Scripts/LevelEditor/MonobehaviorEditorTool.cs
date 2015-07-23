using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class ToolTile
{
    public MonobehaviorEditorTool.TilePlaces Place = new MonobehaviorEditorTool.TilePlaces();
    public Sprite Graphic = null;

    public ToolTile()
    {

    }

    public ToolTile(MonobehaviorEditorTool.TilePlaces zPlace)
    {
        Place = zPlace;
    }

    public ToolTile(MonobehaviorEditorTool.TilePlaces zPlace, Sprite zSprite)
    {
        Place = zPlace;
        Graphic = zSprite;
    }
}

public class MonobehaviorEditorTool : MonoBehaviour
{
    public enum TilePlaces { TopLeft, TopCenter, TopRight, MiddleLeft, MiddleCenter, MiddleRight, BottomLeft, BottomCenter, BottomRight, UNKNOWN };

    public Sprite ReferenceSprite;

    public List<ToolTile> Tiles = new List<ToolTile>(new ToolTile[] { 
        new ToolTile(TilePlaces.TopLeft), new ToolTile(TilePlaces.TopCenter), new ToolTile(TilePlaces.TopRight),
        new ToolTile(TilePlaces.MiddleLeft), new ToolTile(TilePlaces.MiddleCenter), new ToolTile(TilePlaces.MiddleRight),
        new ToolTile(TilePlaces.BottomLeft), new ToolTile(TilePlaces.BottomCenter), new ToolTile(TilePlaces.BottomRight)
    });


    List<SpriteRenderer> AllTiles = new List<SpriteRenderer>();
    int changed = 0;

    public void Apply(List<SpriteRenderer> zTiles)
    {
        //zTiles.RemoveAll(t => Tiles.Any(l => l.Graphic == t.sprite));

        AllTiles = zTiles;

        changed = 0;

        foreach (SpriteRenderer tile in zTiles)
        {
            TilePlaces position = GetTileRelativePosition(tile);
            ApplyTilePositionGraphic(tile, position);
        }

        Debug.Log("Changed " + changed + " tiles out of " + zTiles.Count + " processed.");
    }

    TilePlaces GetTileRelativePosition(SpriteRenderer zTile)
    {
        float x = zTile.transform.position.x;
        float y = zTile.transform.position.y;


        bool AlgunoALaIzquierda = AllTiles.Any(t => t.transform.position.x == zTile.transform.position.x - 1 && t.transform.position.y == zTile.transform.position.y);
        bool AlgunoALaDerecha = AllTiles.Any(t => t.transform.position.x == zTile.transform.position.x + 1 && t.transform.position.y == zTile.transform.position.y);
        bool AlgunoArriba = AllTiles.Any(t => t.transform.position.y == zTile.transform.position.y + 1 && t.transform.position.x == zTile.transform.position.x);
        bool AlgunoAbajo = AllTiles.Any(t => t.transform.position.y == zTile.transform.position.y - 1 && t.transform.position.x == zTile.transform.position.x);



        if (!AlgunoALaIzquierda && AlgunoALaDerecha && !AlgunoArriba && AlgunoAbajo)
        {
            return TilePlaces.TopLeft;
        }
        if (AlgunoALaIzquierda && AlgunoALaDerecha && !AlgunoArriba && AlgunoAbajo)
        {
            return TilePlaces.TopCenter;
        }
        if (AlgunoALaIzquierda && !AlgunoALaDerecha && !AlgunoArriba && AlgunoAbajo)
        {
            return TilePlaces.TopRight;
        }
        if (!AlgunoALaIzquierda && AlgunoALaDerecha && AlgunoArriba && AlgunoAbajo)
        {
            return TilePlaces.MiddleLeft;
        }
        if (AlgunoALaIzquierda && AlgunoALaDerecha && AlgunoArriba && AlgunoAbajo)
        {
            return TilePlaces.MiddleCenter;
        }
        if (AlgunoALaIzquierda && !AlgunoALaDerecha && AlgunoArriba && AlgunoAbajo)
        {
            return TilePlaces.MiddleRight;
        }
        if (!AlgunoALaIzquierda && AlgunoALaDerecha && AlgunoArriba && !AlgunoAbajo)
        {
            return TilePlaces.BottomLeft;
        }
        if (AlgunoALaIzquierda && AlgunoALaDerecha && AlgunoArriba && !AlgunoAbajo)
        {
            return TilePlaces.BottomCenter;
        }
        if (AlgunoALaIzquierda && !AlgunoALaDerecha && AlgunoArriba && !AlgunoAbajo)
        {
            return TilePlaces.BottomRight;
        }

        return TilePlaces.UNKNOWN;
    }

    void ApplyTilePositionGraphic(SpriteRenderer zTile, TilePlaces zPosition)
    {
        if (zPosition == TilePlaces.UNKNOWN)
        {
            Debug.LogError("Tile " + zTile.name + " has not been fixed.");
            return;
        }

        ToolTile tileToApply = Tiles.FirstOrDefault(t => t.Place == zPosition);
        if (tileToApply != null)
        {
            zTile.sprite = tileToApply.Graphic;
            changed++;
        }
    }
}

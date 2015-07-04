using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class EffectsManager : MonoBehaviour
{
    #region Colors

    public Color DamagedColor = Color.red;

    #endregion

    #region Smoke
    public GameObject ObjSmoke;
    Queue<GameObject> _smokes = new Queue<GameObject>();
    public void Smoke(Vector3 spawnPosition)
    {
        _smokes.Enqueue(GameObject.Instantiate(ObjSmoke, spawnPosition, Quaternion.identity) as GameObject);
        Invoke("Smoke_Off", 0.3f);
    }
    void Smoke_Off()
    {
        Destroy(_smokes.Dequeue());
    }


    public GameObject ObjSmallSmoke;
    Queue<GameObject> _smallsmokes = new Queue<GameObject>();
    public void SmallSmoke(Vector2 spawnPosition)
    {
        GameObject Particle = GameObject.Instantiate(ObjSmallSmoke, spawnPosition, Quaternion.identity) as GameObject;
        _smallsmokes.Enqueue(Particle);

        iTween.ScaleTo(Particle, iTween.Hash("scale", new Vector3(0.75f, 0.75f, 1), "time", 0.3f));

        Invoke("SmallSmoke_Off", 0.3f);
    }
    void SmallSmoke_Off()
    {
        Destroy(_smallsmokes.Dequeue());
    }
    #endregion

    #region Minitext
    public Canvas GameCanvas;
    public GameObject MinitextPrefab;

    public void Minitext(string msg, Color msgColor, Vector2 position, GameObject attachTo = null)
    {
        GameObject _txtObj = Instantiate(MinitextPrefab, position, Quaternion.identity) as GameObject;
        _txtObj.transform.parent = GameCanvas.transform;

        Minitext _mtext = _txtObj.GetComponent<Minitext>();
        _mtext.Text.text = msg;
        _mtext.Text.color = msgColor;
        _mtext.FollowObject = attachTo;

        _txtObj.GetComponent<Rigidbody2D>().velocity = Vector2.up * 50;
    }

    #endregion

    #region Hit
    public GameObject ObjHit;
    Queue<GameObject> _hit = new Queue<GameObject>();
    public void Hit(Vector3 spawnPosition)
    {
        spawnPosition.z = 0.5f;
        GameObject newParticle = GameObject.Instantiate(ObjHit, spawnPosition, Quaternion.identity) as GameObject;
        _hit.Enqueue(newParticle);

        newParticle.transform.Rotate(Vector3.forward, Random.Range(0, 360));
        newParticle.transform.localScale = Vector3.one * Random.Range(0.8f, 1f);

        iTween.ScaleFrom(newParticle, iTween.Hash("scale", newParticle.transform.localScale * 0.5f, "time", 0.15f));
        Invoke("Hit_Off", 0.3f);


    }
    void Hit_Off()
    {
        Destroy(_hit.Dequeue());
    }
    #endregion
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class Chest : Interactive
{
    [SerializeField]
    Animator Animator;

    public List<GameObject> Contents = new List<GameObject>();

    private List<Collider2D> Colliders = new List<Collider2D>();

    public override void OnPlayerTouch()
    {
        Debug.Log("Player touch");

        Animator.enabled = true;
        Invoke("AnimationEnd", 0.5f);
    }

    void AnimationEnd()
    {
        for (int i = 0; i < Contents.Count; i++)
        {
            SpawnContent(Contents[i], i);
        }

        Invoke("Disappear", 0.5f);
    }

    void Disappear()
    {
        Colliders.ForEach(c => c.enabled = true);

        GameManager.Instance.Effects.Smoke(transform.position);
        Destroy(gameObject);
    }

    void SpawnContent(GameObject zGameObject, int zIndex)
    {
        GameObject instantiatedObject = Instantiate(zGameObject, RandomPositionAround(), Quaternion.identity) as GameObject;

        Collider2D oCollider = instantiatedObject.GetComponent<Collider2D>();
        if (oCollider != null)
        {
            Colliders.Add(oCollider);
            oCollider.enabled = false;
        }

        iTween.MoveFrom(instantiatedObject, iTween.Hash("position", transform.position, "time", 0.4f, "easetype", iTween.EaseType.easeInOutBounce));
    }

    Vector3 RandomPositionAround()
    {
        Vector3 pos = transform.position;

        pos.x += Random.Range(-0.25f, 0.25f);
        pos.y -= 0.2f;

        return pos;
    }
}

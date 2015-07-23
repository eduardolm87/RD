using UnityEngine;
using System.Collections;

public class SpawnObject : MonoBehaviour
{
    SpriteRenderer Renderer;

    [SerializeField]
    protected GameObject ObjectToSpawn;

    public bool Refresh = false;



    private Monster componentMonster = null;
    private float range = -1;

    void OnValidate()
    {
        if (Renderer == null)
        {
            Renderer = GetComponent<SpriteRenderer>();
        }

        if (ObjectToSpawn != null)
        {
            SpriteRenderer rendererOfObject = ObjectToSpawn.GetComponent<SpriteRenderer>();
            if (rendererOfObject != null && (Renderer.sprite != rendererOfObject.sprite || Refresh))
            {
                Refresh = false;

                Renderer.sprite = rendererOfObject.sprite;
                name = "(Spawn)" + ObjectToSpawn.name;

                //Get useful components
                componentMonster = ObjectToSpawn.GetComponent<Monster>();
                if (componentMonster != null)
                {
                    range = Mathf.Sqrt(componentMonster.Attributes.range);
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if (componentMonster != null)
        {
            DrawMonster();
        }
    }

    void DrawMonster()
    {
        if (range > 0)
        {
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }









    protected virtual void Start()
    {
        Spawn();
    }

    protected virtual void Spawn()
    {
        GameObject Instantiated = Instantiate(ObjectToSpawn, transform.position, transform.rotation) as GameObject;
        Instantiated.transform.SetParent(transform.parent);
        Destroy(gameObject);
    }
}

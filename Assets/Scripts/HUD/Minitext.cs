using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Minitext : MonoBehaviour
{
    public Text Text;

    public GameObject FollowObject = null;

    void Start()
    {
        Invoke("Delete", 0.5f);
    }

    //void Update()
    //{
    //    if (FollowObject != null)
    //    {
    //        Vector2 _followPoint = (Vector2)Camera.main.WorldToScreenPoint(FollowObject.transform.position);
    //        if (Vector3.Distance(_followPoint, transform.position) > 50)
    //            transform.position = _followPoint;
    //    }
    //}

    void Delete()
    {
        Destroy(gameObject);
    }
}

using UnityEngine;
using System.Collections;

public class Rubber : MonoBehaviour
{

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.GetComponent<Rigidbody2D>() != null)
        {
            Invoke("Inflate", 0.05f);

        }
    }

    public int _inflatedStg = 0;

    void Deflate()
    {
        switch (_inflatedStg)
        {
            case 1:
                //deflate
                transform.localScale = new Vector3(0.98f, 0.98f, 1);
                _inflatedStg = 2;
                Invoke("Deflate", 0.1f);
                break;

            case 2:
                //normal and rest
                transform.localScale = Vector3.one;
                _inflatedStg = 3;
                Invoke("Deflate", 0.8f);
                break;

            case 3:
                //restore
                _inflatedStg = 0;
                break;
        }
    }

    void Inflate()
    {
        if (_inflatedStg != 0)
            return;

        _inflatedStg = 1;
        transform.localScale = new Vector3(1.05f, 1.05f, 1);
        Invoke("Deflate", 0.1f);
    }


}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class NPCGUI : MonoBehaviour
{
    [HideInInspector]
    public Monster Entity = null;

    [SerializeField]
    private Image HPBar;


    public static NPCGUI CreateNPCGUI(Monster zEntity)
    {
        NPCGUI newNPCGUI = (Instantiate(GameManager.Instance.Collections.PrefabReferences.NPCGUI.gameObject, zEntity.transform.position, Quaternion.identity) as GameObject).GetComponent<NPCGUI>();

        newNPCGUI.Entity = zEntity;
        newNPCGUI.transform.SetParent(zEntity.transform);
        newNPCGUI.transform.localPosition += Vector3.up * (newNPCGUI.Entity.Collider.bounds.size.y / 2f);

        return newNPCGUI;
    }

    void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (Entity == null)
            return;

        HPBar.fillAmount = Mathf.Clamp01(Entity.Attributes.HP * 1f / Entity.Attributes.HPmax);
    }
}

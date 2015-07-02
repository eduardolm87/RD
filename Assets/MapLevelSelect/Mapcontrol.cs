using UnityEngine;
using System.Collections;

public class Mapcontrol : MonoBehaviour
{
    public Mapnavigator PlayerNavigator;

    public MapStep InitialMapStep;

    public MapArrows NavigationInterface;

    public void Open()
    {
        gameObject.SetActive(true);
        PlayerNavigator.SetOnStep(InitialMapStep);
    }


}

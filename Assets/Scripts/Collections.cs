using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Collections : MonoBehaviour
{
    [System.Serializable]
    public class PrefabContainer
    {
        public NPCGUI NPCGUI;
    }

    public List<Stage> StagesInGame = new List<Stage>();
    public List<Hero> HeroesInGame = new List<Hero>();
    public PrefabContainer PrefabReferences = new PrefabContainer();
}

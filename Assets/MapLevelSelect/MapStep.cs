using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MapStep : MonoBehaviour
{
    [System.Serializable]
    public class Path
    {
        public Directions Direction = Directions.RIGHT;
        public MapStep NextStep = null;
    }

    public enum Access { UNKNOWN, TRANSIT, VISITABLE, LOCKED };
    public enum Directions { NONE, DOWN, RIGHT, UP, LEFT };

    public Access State = Access.TRANSIT;
    public List<Path> Paths = new List<Path>();

    public Stage Stage;

    public List<Directions> GetAvailableDirections()
    {
        return Paths.ConvertAll(p => p.Direction).Distinct().ToList();
    }
}

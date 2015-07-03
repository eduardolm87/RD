using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MapStep : MonoBehaviour
{
    public enum UnlockMethods { AlreadyUnlocked, Normal, Secret, NeverUnlock };

    [System.Serializable]
    public class Path
    {
        public Directions Direction = Directions.RIGHT;
        public MapStep NextStep = null;
        public GameObject Graphic;

        private bool unlocked = false;
        public bool Unlocked
        {
            get { return unlocked; }
            set
            {
                unlocked = value;
                Graphic.SetActive(unlocked);
            }
        }


        public bool ThisPathArrivesToAVisitableLocation(MapStep From)
        {
            if (NextStep == null)
                return false;

            switch (NextStep.State)
            {
                case Access.VISITABLE:
                    return true;
                case Access.LOCKED:
                case Access.UNKNOWN:
                    return false;
            }

            List<Path> pathsHijos = NextStep.Paths.Where(p => p.NextStep != From).ToList();

            if (pathsHijos.Count < 1)
                return false;

            return pathsHijos.Any(p => p.ThisPathArrivesToAVisitableLocation(NextStep));
        }
    }

    public enum Access { UNKNOWN, TRANSIT, VISITABLE, LOCKED };
    public enum Directions { NONE, DOWN, RIGHT, UP, LEFT };

    public Access State = Access.TRANSIT;
    public List<Path> Paths = new List<Path>();

    public Stage Stage;

    public List<Directions> GetAvailableDirections()
    {
        return Paths.Where(k => k.Unlocked).ToList().ConvertAll(p => p.Direction).Distinct().ToList();
    }
}

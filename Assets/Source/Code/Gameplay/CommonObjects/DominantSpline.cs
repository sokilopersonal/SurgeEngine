using SurgeEngine.Source.Code.Gameplay.CommonObjects.ChangeModes;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects
{
    public class DominantSpline : MonoBehaviour
    {
        [SerializeField] private DominantSide overrideDominantSide = DominantSide.Left;
        public DominantSide OverrideDominantSide => overrideDominantSide;
    }
}
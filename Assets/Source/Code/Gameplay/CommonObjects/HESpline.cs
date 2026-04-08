using Alchemy.Inspector;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects
{
    [RequireComponent(typeof(SplineContainer))]
    public class HESpline : MonoBehaviour
    {
        [SerializeField] private SplineTag splineTag;
        [SerializeField, ReadOnly] private string id;
        public SplineTag SplineTag => splineTag;
        public string Id => id;
        public SplineContainer Container => _container;
        
        private SplineContainer _container;
        
        private void Awake()
        {
            _container = GetComponent<SplineContainer>();
        }
        
        public void SetID(string newId) => id = newId;
        public void SetSplineTag(SplineTag newSplineTag) => splineTag = newSplineTag;
    }
    
    public enum DominantSide
    {
        Left,
        Right
    }
}
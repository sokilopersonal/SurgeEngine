using UnityEngine;

namespace SurgeEngine.Code.CommonObjects.System
{
    [ExecuteInEditMode]
    public class PointMarkerView : MonoBehaviour
    {
        [SerializeField] private Transform left, right, line;
        [SerializeField] private BoxCollider _boxCollider;

        [SerializeField] private Animator leftAnimator, rightAnimator;
        
        private PointMarker _pointMarker;
        
        private static readonly int Forward = Animator.StringToHash("Forward");

        private void Awake()
        {
            FindPointMarker();
        }

        private void Start()
        {
            FindPointMarker();
        }

        private void FindPointMarker()
        {
            if (!_pointMarker)
                _pointMarker = GetComponentInParent<PointMarker>();
        }

        private void OnEnable()
        {
            _pointMarker.OnContact += ContactAnimation;
        }
        
        private void OnDisable()
        {
            _pointMarker.OnContact -= ContactAnimation;
        }

        private void Update()
        {
            UpdateView();
        }

        public void UpdateView()
        {
            if (!left || !right || !line || !_boxCollider) return;
            
            left.localPosition = new Vector3(-_pointMarker.Length, 0, 0);
            right.localPosition = new Vector3(_pointMarker.Length, 0, 0);
            
            line.localPosition = Vector3.zero;
            line.localScale = new Vector3(_pointMarker.Length, 1f, 1f);

            Vector3 size = _boxCollider.size;
            size.x = _pointMarker.Length * 2f;
            _boxCollider.size = size;
        }

        private void ContactAnimation(ContactBase obj)
        {
            leftAnimator.CrossFadeInFixedTime(Forward, 0.1f, 0);
            rightAnimator.CrossFadeInFixedTime(Forward, 0.1f, 0);
        }
    }
}
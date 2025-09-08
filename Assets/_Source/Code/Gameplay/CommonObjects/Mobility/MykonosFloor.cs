using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.Mobility
{
    public class MykonosFloor : StageObject
    {
        [SerializeField] private float amplitude = 5f;
        [SerializeField] private float cycle = 0.5f;
        [SerializeField] private float phase;
        [SerializeField] private float gravity = 35f;
        [SerializeField] private int moveType = 1;
        
        private Vector3 _startPos;

        private void Awake()
        {
            _startPos = transform.position;
        }

        private void Update()
        {
            if (moveType == 2)
            {
                float offset = Mathf.Sin((Time.time * cycle + phase) * Mathf.PI * 2f) * amplitude;
                transform.position = _startPos + transform.right * offset;
            }
        }
    }
}
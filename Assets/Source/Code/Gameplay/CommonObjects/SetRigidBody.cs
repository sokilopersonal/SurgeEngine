using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects
{
    public class SetRigidBody : StageObject
    {
        [SerializeField] private bool defaultOn = true;
        
        private Collider _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            
            _collider.enabled = defaultOn;
        }
        
        public void Enable(bool value)
        {
            _collider.enabled = value;
        }
    }
}
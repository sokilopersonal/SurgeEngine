using System;
using SurgeEngine._Source.Code.Core.Character.System;
using UnityEngine;
using UnityEngine.Events;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects
{
    public class EventCollision : StageObject
    {
        [SerializeField] private int defaultStatus;
        [SerializeField] private int durability;
        [SerializeField] private UnityEvent eventOnContact;

        private Collider _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _collider.isTrigger = true;
            
            _collider.enabled = defaultStatus == 0;
        }

        public override void Contact(Collider msg, CharacterBase context)
        {
            base.Contact(msg, context);
            
            eventOnContact.Invoke();

            if (durability == 0)
            {
                Destroy(gameObject);
            }
            else
            {
                _collider.enabled = false;
            }
        }
    }
}
using SurgeEngine.Source.Code.Core.Character.System;
using UnityEngine;
using UnityEngine.Events;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects
{
    public class EventCollision : StageObject
    {
        [SerializeField] private int defaultStatus;
        [SerializeField] private bool oneTime = true;
        [SerializeField] private int durability;
        [SerializeField] private UnityEvent eventOnContact;
        [SerializeField] private UnityEvent<CharacterBase> eventOnCharacterContact;

        private Collider _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _collider.isTrigger = true;
            
            _collider.enabled = defaultStatus == 0;
        }

        public override void OnEnter(Collider msg, CharacterBase context)
        {
            base.OnEnter(msg, context);
            
            eventOnContact.Invoke();
            eventOnCharacterContact.Invoke(context);

            if (durability == 0)
            {
                Destroy(gameObject);
            }
            else if (oneTime)
            {
                _collider.enabled = false;
            }
        }
    }
}
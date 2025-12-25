using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using SurgeEngine.Source.Code.Core.Character.Sound;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.Collectables;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Environment
{
    public class ParaloopVolume : StageObject
    {
        [SerializeField] private EventReference paraloopSound;
        
        private BoxCollider _boxCollider;
        private List<Ring> _ringsList;
        
        private Coroutine _paraloopCoroutine;

        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider>();
            _ringsList = new List<Ring>();
        }

        public override void OnEnter(Collider msg, CharacterBase context)
        {
            base.OnEnter(msg, context);
            
            context.Effects.ParaloopEffect.Toggle(true);
            RuntimeManager.PlayOneShotAttached(paraloopSound, context.gameObject);
            
            var rings = Physics.OverlapBox(_boxCollider.bounds.center, _boxCollider.bounds.extents, transform.rotation);
            if (rings.Length == 0) return;
            _ringsList.Clear();
            foreach (var ring in rings)
            {
                var ringComponent = ring.GetComponent<Ring>();
                if (ringComponent != null && !ringComponent.IsSuperRing)
                {
                    _ringsList.Add(ringComponent);
                }
            }
        }

        public override void OnExit(Collider msg, CharacterBase context)
        {
            base.OnExit(msg, context);
            
            if (_paraloopCoroutine != null) StopCoroutine(_paraloopCoroutine);
            _paraloopCoroutine = StartCoroutine(CollectRings(context));
        }

        private IEnumerator CollectRings(CharacterBase character)
        {
            yield return new WaitForSeconds(1f);

            character.Effects.ParaloopEffect.Toggle(false);
            
            foreach (var ring in _ringsList)
            {
                ring.Collect(1);
            }
        }
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.VFX;

namespace SurgeEngine.Code.Gameplay.Effects
{
    public class BoostAura : Effect
    {
        [SerializeField] private VisualEffect auraEffect;

        private Coroutine _coroutine;

        private void Awake()
        {
            GetComponent<CustomPassVolume>().targetCamera = Camera.main;
        }

        public override void Toggle(bool value)
        {
            if (value) 
            {
                if (_coroutine != null)
                {
                    StopCoroutine(_coroutine);
                }

                auraEffect.Play();
            } 
            else 
            {
                _coroutine = StartCoroutine(StopAuraWithDelay());
            }
        }

        private IEnumerator StopAuraWithDelay()
        {
            yield return new WaitForSeconds(stopDelay);
            auraEffect.Stop();
        }
    }
}
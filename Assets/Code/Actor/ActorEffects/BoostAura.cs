using System.Collections;
using UnityEngine;

namespace SurgeEngine.Code.ActorEffects
{
    public class BoostAura : MonoBehaviour
    {
        [SerializeField] private ParticleSystem[] particleSystems;
        [SerializeField] private float stopDelay = 0.2f;

        private Coroutine _coroutine;

        private void OnEnable()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            foreach (var gme in particleSystems)
            {
                gme.Play();
            }
        }

        private void OnDisable()
        {
            if (_coroutine == null)
            {
                _coroutine = StartCoroutine(StopParticlesWithDelay());
            }
        }

        private IEnumerator StopParticlesWithDelay()
        {
            yield return new WaitForSeconds(stopDelay);
        
            foreach (var gme in particleSystems)
            {
                gme.Stop();
            }

            _coroutine = null;
        }
    }
}
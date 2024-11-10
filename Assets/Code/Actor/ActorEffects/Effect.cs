using System.Collections;
using UnityEngine;

namespace SurgeEngine.Code.ActorEffects
{
    public class Effect : MonoBehaviour
    {
        [SerializeField] private ParticleSystem particle;
        [SerializeField] private float stopDelay;
        
        private Coroutine _coroutine;
        
        /// <summary>
        /// Toggle effect. If stop delay is more than 0, coroutine will be started
        /// </summary>
        /// <param name="value"></param>
        public void Toggle(bool value)
        {
            if (value)
            {
                if (_coroutine != null)
                {
                    StopCoroutine(_coroutine);
                    _coroutine = null;
                }

                particle.Play(true);
            }
            else
            {
                if (stopDelay > 0)
                {
                    _coroutine ??= StartCoroutine(StopParticlesWithDelay());
                }
                else
                {
                    particle.Stop(true);
                }
            }
        }

        private IEnumerator StopParticlesWithDelay()
        {
            yield return new WaitForSeconds(stopDelay);
        
            particle.Stop(true);

            _coroutine = null;
        }
    }
}
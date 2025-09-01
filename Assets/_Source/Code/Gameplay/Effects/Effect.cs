using System.Collections;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.Effects
{
    public class Effect : MonoBehaviour
    {
        [SerializeField] protected ParticleSystem particle;
        public ParticleSystem Particle => particle;
        [SerializeField] protected float stopDelay;
        
        private Coroutine _coroutine;

        /// <summary>
        /// Toggle effect. If stop delay is more than 0, coroutine will be started
        /// </summary>
        /// <param name="value"></param>
        public virtual void Toggle(bool value)
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

        public virtual void Clear()
        {
            particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        private IEnumerator StopParticlesWithDelay()
        {
            yield return new WaitForSeconds(stopDelay);
        
            particle.Stop(true);

            _coroutine = null;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects
{
    public class Flame : StageObject
    {
        [SerializeField] private float appearTime = 0.25f;
        [SerializeField] private float onTime = 1f;
        [SerializeField] private float offTime = 1f;
        [SerializeField] private float length = 5;
        [SerializeField] private int phase;
        [SerializeField] private int type = 1; // 0 - cycle, 1 - constant, 2 - disabled
        [SerializeField] private List<Flame> multiSetParam = new List<Flame>();
        [SerializeField] private BoxCollider damageCollider;
        [SerializeField] private ParticleSystem flame;
        
        private Coroutine _cycleRoutine;

        private void Awake()
        {
            damageCollider.isTrigger = true;

            if (type != 2)
            {
                SetActive(true);
            }
            else
            {
                flame.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                flame.Clear();
                damageCollider.enabled = false;
            }
        }

        public void SetActive(bool active)
        {
            if (active)
            {
                if (_cycleRoutine != null)
                {
                    StopCoroutine(_cycleRoutine);
                    _cycleRoutine = null;
                }
                
                if (type == 0)
                {
                    _cycleRoutine = StartCoroutine(FlameCycle());
                }
                else if (type == 1)
                {
                    _cycleRoutine = StartCoroutine(ConstantFlame());
                }
                else if (type == 2)
                {
                    _cycleRoutine = StartCoroutine(ConstantFlame());
                }
            }
            else
            {
                if (_cycleRoutine != null)
                {
                    StopCoroutine(_cycleRoutine);
                    _cycleRoutine = null;
                }

                _cycleRoutine = StartCoroutine(ScaleFlame(1f, 0f, appearTime));
            }
            
            foreach (var flameSet in multiSetParam)
            {
                flameSet.SetActive(active);
            }
        }

        private IEnumerator ConstantFlame()
        {
            yield return ScaleFlame(0f, 1f, appearTime);
            
            damageCollider.enabled = true;
            damageCollider.size = new Vector3(damageCollider.size.x, damageCollider.size.y, length);
            damageCollider.center = new Vector3(damageCollider.center.x, damageCollider.center.y, length * 0.5f);
            
            while (true)
            {
                yield return null;
            }
        }

        private IEnumerator FlameCycle()
        {
            while (true)
            {
                yield return ScaleFlame(0f, 1f, appearTime);
                yield return new WaitForSeconds(onTime);
                yield return ScaleFlame(1f, 0f, appearTime);
                yield return new WaitForSeconds(offTime);
            }
        }

        private IEnumerator ScaleFlame(float from, float to, float duration)
        {
            float elapsed = 0f;
            flame.Play();

            if (to > from)
                damageCollider.enabled = true;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Lerp(from, to, elapsed / duration);

                float currentLength = length * t;

                var main = flame.main;
                main.startSpeed = currentLength * 2;

                damageCollider.size = new Vector3(damageCollider.size.x, damageCollider.size.y, currentLength);
                damageCollider.center = new Vector3(damageCollider.center.x, damageCollider.center.y, currentLength * 0.5f);

                yield return null;
            }

            if (to <= 0.01f)
            {
                flame.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                damageCollider.enabled = false;
            }
        }
    }
}
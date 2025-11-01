using System.Collections;
using SurgeEngine._Source.Code.Core.Character.States.Characters.Sonic.SubStates;
using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Core.StateMachine.Base;
using UnityEngine;

namespace SurgeEngine._Source.Code.Core.Character.CameraSystem.Modifiers
{
    public class BoostBaseCameraModifier : BaseCameraModifier, ICameraFloatModifier
    {
        [SerializeField] protected AnimationCurve blendCurve;
        [SerializeField] protected AnimationCurve unBlendCurve;
        [SerializeField] protected float blendTime = 4.5f;
        [SerializeField] protected float unBlendTime = 1.2f;

        private float BoostBlendFactor { get; set; }
        private Coroutine BoostBlendCoroutine { get; set; }
        public float Value { get; set; }

        public override void Set(CharacterBase character)
        {
            base.Set(character);

            // Character.StateMachine.GetSubState<FBoost>().OnActiveChanged += OnBoostActivate;
            
            if (character.StateMachine.GetState(out FBoost boost))
                boost.OnActiveChanged += OnBoostActivate;
            
            Value = 1;
        }

        private void OnBoostActivate(FSubState arg1, bool arg2)
        {
            if (BoostBlendCoroutine != null)
                StopCoroutine(BoostBlendCoroutine);
                
            BoostBlendCoroutine = StartCoroutine(Blend(arg1, arg2));
        }

        private IEnumerator Blend(FSubState sub, bool active)
        {
            if (sub is FBoost)
            {
                float t = 0;
                float time = active ? blendTime : unBlendTime;
                float lastBlendFactor = BoostBlendFactor;
                float lastValue = Value;
                while (t < 1f)
                {
                    t += Time.deltaTime / time;

                    if (active)
                    {
                        BoostBlendFactor = t;
                        Value = blendCurve.Evaluate(t);
                    }
                    else
                    {
                        BoostBlendFactor = Mathf.Lerp(lastBlendFactor, 0f, unBlendCurve.Evaluate(t));
                        Value = Mathf.Lerp(lastValue, 1f, unBlendCurve.Evaluate(t));
                    }
                    
                    yield return null;
                }
            }
        }

    }
}
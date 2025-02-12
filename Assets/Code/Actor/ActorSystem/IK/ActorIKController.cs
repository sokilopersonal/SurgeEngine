using System;
using SurgeEngine.Code.StateMachine.Components;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace SurgeEngine.Code.ActorSystem.IK
{
    public class ActorIKController : MonoBehaviour
    {
        [SerializeField] private Rig[] rigs;
        [SerializeField] private float weightSpeed = 14;
        private StateAnimator _stateAnimator;

        private void Awake()
        {
            _stateAnimator = GetComponent<StateAnimator>();

            foreach (var rig in rigs)
            {
                rig.weight = 1;
            }
        }

        private void Update()
        {
            float value = _stateAnimator.IsIKAllowed() ? 1 : 0;
            foreach (var rig in rigs)
            {
                rig.weight = Mathf.Lerp(rig.weight, value, Time.deltaTime * weightSpeed);
            }
        }
    }
}
using SurgeEngine.Code.StateMachine.Components;
using UnityEngine;

namespace SurgeEngine.Code.Actor.System.IK
{
    public class ActorIKController : MonoBehaviour
    {
        [SerializeField] private AnimationRiggingFootPlanter[] planters;
        [SerializeField] private float weightSpeed = 14;
        private StateAnimator _stateAnimator;

        private float _ikWeight;

        private void Awake()
        {
            _stateAnimator = GetComponent<StateAnimator>();

            _ikWeight = 1;
        }

        private void Update()
        {
            float value = _stateAnimator.IsIKAllowed() ? 1 : 0;
            _ikWeight = Mathf.Lerp(_ikWeight, value, Time.deltaTime * weightSpeed);
            foreach (var rig in planters)
            {
                rig.SolveIK(_ikWeight);
            }
        }
    }
}
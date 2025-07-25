using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.System
{
    public class IdleBehaviour : StateMachineBehaviour
    {
        [SerializeField] private string[] idleAnimations;
        [SerializeField] private float idleTime = 2f;

        private float _time;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            _time = 0;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            if (_time < idleTime)
            {
                _time += Time.deltaTime;
                
                if (_time >= idleTime)
                {
                    int index = Random.Range(0, idleAnimations.Length);
                    animator.CrossFadeInFixedTime(idleAnimations[index], 0.1f);
                    _time = 0;
                }
            }
        }

        public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex) { }

        public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex) { }
    }
}
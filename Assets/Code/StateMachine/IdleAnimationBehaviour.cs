using UnityEngine;

namespace SurgeEngine.Code.StateMachine
{
    public class IdleAnimationBehaviour : StateMachineBehaviour
    {
        [SerializeField] private float timeToIdle = 3f;
        [SerializeField] private int idleNumber;

        private bool _isIdle;
        private float _idleTimer;
        private int _idleIndex;
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            ResetIdle(animator);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            if (!_isIdle)
            {
                _idleTimer += Time.deltaTime;

                if (_idleTimer >= timeToIdle)
                {
                    _isIdle = true;
                    _idleIndex = Random.Range(0, idleNumber + 1);

                    animator.SetInteger("IdleIndex", _idleIndex);
                    animator.SetTrigger("IdleAction");
                }
            }
        }

        public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
        }

        public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
        }

        private void ResetIdle(Animator animator)
        {
            _isIdle = false;
            _idleTimer = 0;
            _idleIndex = 0;
            
            animator.SetInteger("IdleIndex", 0);
        }
    }
}
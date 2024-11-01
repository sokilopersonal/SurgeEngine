using UnityEngine;

namespace SurgeEngine.Code.StateMachine
{
    [DefaultExecutionOrder(-9888)]
    public class EnemyBase : MonoBehaviour
    {
        public FStateMachine stateMachine;

        protected virtual void Awake()
        {
            stateMachine = new FStateMachine();
        }
    }
}
using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.ActorStates.SonicSpecific;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Enemy.States;
using SurgeEngine.Code.Tools;
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
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.Enemy
{
    public class EggFighter : MonoBehaviour
    {
        private FStateMachine _stateMachine;
        
        private void Awake()
        {
            _stateMachine = new FStateMachine();
        }
    }
}
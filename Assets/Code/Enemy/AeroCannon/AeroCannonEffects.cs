using SurgeEngine.Code.Enemy.AeroCannon.States;
using SurgeEngine.Code.StateMachine;
using SurgeEngine.Code.StateMachine.Components;
using UnityEngine;

namespace SurgeEngine.Code.Enemy.AeroCannon.States
{
    public class AeroCannonEffects : MonoBehaviour
    {
        [SerializeField] AeroCannon enemy;
        [SerializeField] ParticleSystem chargeParticle;
        [SerializeField] ParticleSystem fireParticle;

        private void Start()
        {
            enemy.stateMachine.OnStateAssign += OnStateAssign;
        }

        void OnStateAssign(FState obj)
        {
            if (obj is ACStatePrepare)
                chargeParticle.Play(true);
            else
                chargeParticle.Stop(true);

            if (obj is ACStateShoot)
                fireParticle.Play();
        }
    }
}
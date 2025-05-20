using SurgeEngine.Code.Core.StateMachine.Base;
using SurgeEngine.Code.Gameplay.Enemy.AeroCannon.States;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.Enemy.AeroCannon
{
    public class AeroCannonEffects : MonoBehaviour
    {
        [SerializeField] private AeroCannon enemy;
        [SerializeField] private ParticleSystem chargeParticle;
        [SerializeField] private ParticleSystem fireParticle;

        private void Start()
        {
            enemy.StateMachine.OnStateAssign += OnStateAssign;
        }

        private void OnStateAssign(FState obj)
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
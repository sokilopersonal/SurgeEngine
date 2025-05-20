using FMOD.Studio;
using FMODUnity;
using SurgeEngine.Code.Core.StateMachine.Base;
using SurgeEngine.Code.Gameplay.Enemy.AeroCannon.States;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.Enemy.AeroCannon
{
    public class AeroCannonSound : MonoBehaviour
    {
        [SerializeField] private AeroCannon enemy;
        [SerializeField] private EventReference chargeSound;
        [SerializeField] private EventReference fireSound;

        private EventInstance charge;
        private EventInstance fire;

        private void Start()
        {
            charge = RuntimeManager.CreateInstance(chargeSound);
            charge.set3DAttributes(transform.To3DAttributes());

            fire = RuntimeManager.CreateInstance(fireSound);
            fire.set3DAttributes(transform.To3DAttributes());

            enemy.StateMachine.OnStateAssign += OnStateAssign;
        }

        private void OnDestroy()
        {
            charge.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }

        private void OnStateAssign(FState obj)
        {
            if (obj is ACStatePrepare)
                charge.start();
            else
                charge.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

            if (obj is ACStateShoot)
                fire.start();
        }
    }
}
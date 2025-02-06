using FMOD.Studio;
using FMODUnity;
using SurgeEngine.Code.Enemy.AeroCannon.States;
using SurgeEngine.Code.StateMachine;
using SurgeEngine.Code.StateMachine.Components;
using UnityEngine;

namespace SurgeEngine.Code.Enemy.AeroCannon.States
{
    public class AeroCannonSound : MonoBehaviour
    {
        [SerializeField] AeroCannon enemy;
        [SerializeField] EventReference chargeSound;
        [SerializeField] EventReference fireSound;

        EventInstance charge;
        EventInstance fire;

        private void Start()
        {
            charge = RuntimeManager.CreateInstance(chargeSound);
            charge.set3DAttributes(transform.To3DAttributes());

            fire = RuntimeManager.CreateInstance(fireSound);
            fire.set3DAttributes(transform.To3DAttributes());

            enemy.stateMachine.OnStateAssign += OnStateAssign;
        }

        private void OnDestroy()
        {
            charge.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }

        void OnStateAssign(FState obj)
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
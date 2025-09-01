using FMOD.Studio;
using FMODUnity;
using SurgeEngine._Source.Code.Core.StateMachine.Base;
using SurgeEngine._Source.Code.Gameplay.Enemy.AeroCannon.States;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.Enemy.AeroCannon
{
    public class AeroCannonSound : MonoBehaviour
    {
        [SerializeField] private AeroCannon enemy;
        [SerializeField] private EventReference chargeSound;
        [SerializeField] private EventReference fireSound;

        private EventInstance _chargeInstance;
        private EventInstance _fireInstance;

        private void Start()
        {
            _chargeInstance = RuntimeManager.CreateInstance(chargeSound);
            _chargeInstance.set3DAttributes(transform.To3DAttributes());

            _fireInstance = RuntimeManager.CreateInstance(fireSound);
            _fireInstance.set3DAttributes(transform.To3DAttributes());

            enemy.StateMachine.OnStateAssign += OnStateAssign;
        }

        private void OnDisable()
        {
            _chargeInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            _fireInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }

        private void OnStateAssign(FState obj)
        {
            if (obj is ACStatePrepare)
                _chargeInstance.start();
            else
                _chargeInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

            if (obj is ACStateShoot)
                _fireInstance.start();
        }
    }
}
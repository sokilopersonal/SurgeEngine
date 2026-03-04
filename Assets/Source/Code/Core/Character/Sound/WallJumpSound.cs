using System;
using FMOD.Studio;
using FMODUnity;
using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.StateMachine.Base;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace SurgeEngine.Source.Code.Core.Character.Sound
{
    public class WallJumpSound : CharacterSound
    {
        [SerializeField] private EventReference wallStickSound;
        [SerializeField] private EventReference wallStickLoopSound; // TODO: Implement wall loop sound idk which one Unleashed uses (if it does)
        [SerializeField] private EventReference wallJumpSound;
        
        private EventInstance _wallStickLoopInstance;

        private void Awake()
        {
            // _wallStickLoopInstance = RuntimeManager.CreateInstance(wallStickLoopSound);
            // _wallStickLoopInstance.set3DAttributes(transform.To3DAttributes());
        }

        protected override void SoundState(FState obj)
        {
            base.SoundState(obj);

            if (obj is FStateWall)
            {
                RuntimeManager.PlayOneShotAttached(wallStickSound, Character.gameObject);
                //_wallStickLoopInstance.start();
            }
            else
            {
                //_wallStickLoopInstance.stop(STOP_MODE.ALLOWFADEOUT);
            }
            
            if (obj is FStateWallJump)
            {
                RuntimeManager.PlayOneShotAttached(wallJumpSound, Character.gameObject);
            }
        }
    }
}
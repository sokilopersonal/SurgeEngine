using System.Collections.Generic;
using System.Linq;
using SurgeEngine.Code.ActorSoundEffects;
using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorSounds : MonoBehaviour, IActorComponent
    {
        public Actor actor { get; set; }

        private List<ActorSound> _sounds = new List<ActorSound>(); 
        
        private const float BOOST_VOICE_DELAY = 1.5f;

        private void Awake()
        {
            _sounds = GetComponents<ActorSound>().ToList();

            foreach (var sound in _sounds)
            {
                sound.Initialize();
            }
        }
        
        // private void OnBoostActivate(FSubState arg1, bool arg2)
        // {
        //     if (arg1 is FBoost && arg2)
        //     {
        //         if (_lastBoostVoiceTime + BOOST_VOICE_DELAY < Time.time)
        //         {
        //             PlaySound($"Boost_Start{Random.Range(1, 4)}", false);
        //             _lastBoostVoiceTime = Time.time;
        //         }
        //         
        //         PlaySound("BoostLoop", true);
        //         PlaySound("BoostJet", false);
        //         PlaySound("BoostForce", false);
        //         PlaySound("BoostImpulse", false);
        //         
        //         distortion.ToggleDistortion();
        //     }
        //     else if (arg1 is FBoost && !arg2)
        //     {
        //         StopSound("BoostLoop", true);
        //         StopSound("BoostJet", false);
        //         
        //         distortion.ToggleDistortion();
        //     }
        // }
        //
        // private void OnStateAssign(FState obj)
        // {
        //     if (obj is FStateJump)
        //     {
        //         PlaySound($"Jump_Start{Random.Range(1, 4)}", false);
        //         
        //         PlaySound("Spin", false);
        //     }
        // }
        //
        // private void OnRingCollected(Ring obj)
        // {
        //     PlaySound("Ring", false);
        //     PlaySound($"Ring_Sparkle{Random.Range(1, 4)}", false);
        // }
    }
}
using System;
using UnityEngine;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.ActorSoundEffects;
using DG.Tweening;

namespace SurgeEngine.Code.ActorEffects
{
    public class SwingEffect : Effect
    {
        public TrailRenderer trail;
        public override void Toggle(bool value)
        {
            trail.emitting = value;
        }
    }
}
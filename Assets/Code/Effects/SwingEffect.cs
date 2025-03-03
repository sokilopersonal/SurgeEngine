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
        public bool trail2D = false;
        public override void Toggle(bool value)
        {
            trail.transform.localEulerAngles = new Vector3(trail2D ? 90 : 0, 0, 0);
            trail.emitting = value;
        }
    }
}
using FMODUnity;
using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic.SubStates;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Infrastructure.Custom;
using SurgeEngine.Source.Code.Infrastructure.Custom.Drawers;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility
{
    [Serializable]
    public class BalloonStyle
    {
        public Color balloonColor;
        public List<Material> templateMaterials = new List<Material>();
        public List<ParticleSystem> colorParticles = new List<ParticleSystem>();
    }
    public class Balloon : StageObject
    {
        [SerializeField] private BalloonStyle style;
        [SerializeField] private float minSpeed = 1.0f;
        [SerializeField] private float maxSpeed = 10.0f;
        [SerializeField] private ParticleSystem mainParticle;
        [SerializeField] private SkinnedMeshRenderer mesh;
        [SerializeField] private HomingTarget target;
        [SerializeField] private EventReference sound;

        private bool _triggered = false;

        private void OnValidate()
        {
            if (mesh == null || style.templateMaterials.Count == 0)
                return;

            Material[] mats = mesh.sharedMaterials;
            
            for (int i = 0; i < mats.Length; i++)
            {
                Material mat = new Material(style.templateMaterials[i]);
                mat.SetColor("_AlbedoColor", style.balloonColor);
                mat.SetColor("_ReflectionColor", style.balloonColor);
                mats[i] = mat;
            }
            
            foreach(ParticleSystem particle in style.colorParticles)
            {
                ParticleSystem.MainModule mainMod = particle.main;
                mainMod.startColor = style.balloonColor;
            }

            mesh.sharedMaterials = mats;
        }

        public override void OnEnter(Collider msg, CharacterBase context)
        {
            base.OnEnter(msg, context);

            if (_triggered)
                return;

            mesh.enabled = false;
            target.gameObject.SetActive(false);

            RuntimeManager.PlayOneShot(sound, transform.position);

            mainParticle.Play();

            context.StateMachine.SetState<FStateBalloon>(true);

            float speed = Mathf.Clamp(context.Kinematics.Speed, minSpeed, maxSpeed);

            context.Rigidbody.linearVelocity = (context.transform.forward * speed) + (context.transform.up * speed * 0.5f);

            _triggered = true;
        }
    }
}
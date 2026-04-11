using System.Xml.Linq;
using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic.SubStates;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Infrastructure.Custom;
using SurgeEngine.Source.Code.Infrastructure.Custom.Drawers;
using SurgeEngine.Source.Code.Tools;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility
{
    /// <summary>
    /// Trigger for applying an impulse to the player
    /// </summary>
    public class JumpCollision : StageObject, IHE1Importable
    {
        [Header("Properties")] 
        [SerializeField] private float speedMin = 20f;
        [SerializeField, Range(0, 90)] private float pitch = 10f;
        [SerializeField] private bool groundOnly = true;
        [SerializeField, Min(0)] private float impulseOnNormal = 15f;
        [SerializeField, Min(0)] private float impulseOnBoost = 15f;
        [SerializeField] private float outOfControl = 0.5f;
        [SerializeField] private float terrainIgnoreTime = 0.25f;
        
        private const float CoyoteTime = 0.2f;
        
        private CharacterBase _character;
        private float _lastGroundedTime = float.NegativeInfinity;

        private void Update()
        {
            if (_character != null && !_character.Kinematics.InAir)
                _lastGroundedTime = Time.time;
        }

        public override void OnEnter(Collider msg, CharacterBase context)
        {
            base.OnEnter(msg, context);
            
            _character = context;
            _lastGroundedTime = Time.time;

            Launch(context);
        }

        private void Launch(CharacterBase context)
        {
            float dot = Vector3.Dot(context.transform.forward, transform.forward);
            float impulse = impulseOnNormal;
            if (context.StateMachine.GetState(out FBoost boost))
            {
                if (boost.Active)
                    impulse = impulseOnBoost;
            }

            if (dot <= 0) return;
            if (context.Kinematics.Speed < speedMin) return;

            bool wasRecentlyGrounded = Time.time - _lastGroundedTime <= CoyoteTime;
            if ((groundOnly && !wasRecentlyGrounded) || !groundOnly) return;

            context.Model.DisableCollision(terrainIgnoreTime);
            context.Kinematics.SetDetachTime(terrainIgnoreTime);

            context.StateMachine.SetState<FStateAir>();

            Rigidbody body = context.Kinematics.Rigidbody;
            Vector3 force = Utility.GetImpulseWithPitch(transform.forward, -transform.right, pitch, impulse);
            body.linearVelocity = force;

            if (outOfControl > 0) context.Flags.AddFlag(new Flag(FlagType.OutOfControl, outOfControl));
        }

        private void OnDrawGizmosSelected()
        {
            TrajectoryDrawer.DrawTrajectory(transform.position + transform.up, Utility.GetImpulseWithPitch(transform.forward, -transform.right, pitch, impulseOnNormal), Color.green, impulseOnNormal);
            TrajectoryDrawer.DrawTrajectory(transform.position + transform.up, Utility.GetImpulseWithPitch(transform.forward, -transform.right, pitch, impulseOnBoost), Color.cyan, impulseOnBoost);
        }

        public void ImportSetData(string objectName, XElement elem)
        {
            var boxCollider = GetComponent<BoxCollider>();
            float w = HE1Helper.GetFloat(elem, "Collision_Width");
            float h = HE1Helper.GetFloat(elem, "Collision_Height");
            boxCollider.size = new Vector3(w, h, boxCollider.size.z);
            
            speedMin = HE1Helper.GetFloat(elem, "SpeedMin") / 2;
            outOfControl = HE1Helper.GetFloat(elem, "OutOfControl");
            impulseOnNormal = HE1Helper.GetFloat(elem, "ImpulseSpeedOnNormal");
            impulseOnBoost = HE1Helper.GetFloat(elem, "ImpulseSpeedOnBoost");
            pitch = HE1Helper.GetFloat(elem, "Pitch");
            terrainIgnoreTime = HE1Helper.GetFloat(elem, "TerrainIgnoreTime", 0.25f);
        }
    }
}

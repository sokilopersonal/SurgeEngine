using System.Xml.Linq;
using FMODUnity;
using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic.SubStates;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Infrastructure.Custom.Drawers;
using SurgeEngine.Source.Code.Tools;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility
{
    public class Spring : StageObject, IHE1Importable
    {
        [SerializeField] protected float speed = 30f;
        [SerializeField] protected float keepVelocityDistance = 5;
        [SerializeField] protected float outOfControl = 0.5f;
        [SerializeField] protected bool cancelBoost;
        [SerializeField] protected bool isTo3D;
        [SerializeField] protected bool isWallWalk;
        [SerializeField] private bool hasBase;
        [SerializeField] private EventReference sound;
        [SerializeField] private GameObject baseModel;
        public float Speed => speed;
        public virtual float KeepVelocityDistance => keepVelocityDistance;
        public bool IsWallWalk => isWallWalk;
        public virtual Vector3 Direction => transform.up;

        public virtual bool ShouldSnap => true;

        public override void OnEnter(Collider msg, CharacterBase context)
        {
            base.OnEnter(msg, context);

            Launch(context);
        }

        protected virtual void Launch(CharacterBase context)
        {
            var springState = context.StateMachine.GetState<FStateSpring>();
            if (springState.SpringObject == this) return;

            if (cancelBoost && context.StateMachine.GetState(out FBoost boost)) 
                boost.Active = false;
            
            springState.SetKeepVelocityDistance(keepVelocityDistance);
            springState.SetSpringObject(this);
            context.StateMachine.SetState<FStateSpring>();
            
            if (ShouldSnap) Snap(context);
            
            if (outOfControl > 0) context.Flags.AddFlag(new Flag(FlagType.OutOfControl, outOfControl));
            
            RuntimeManager.PlayOneShot(sound, transform.position);

            if (context.Kinematics.Mode.ModeSide != null)
            {
                if (isTo3D)
                {
                    context.Kinematics.Mode.Set2DMode(null);
                    context.Kinematics.Mode.SetForwardMode(null);
                    context.Kinematics.Mode.SetDashPath(null);
                }
            }
        }

        private void Snap(CharacterBase context)
        {
            context.Kinematics.MoveToPosition(context.Kinematics.Rigidbody, transform.position);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            EditorApplication.delayCall += () =>
            {
                if (baseModel)
                    baseModel.SetActive(hasBase);
            };
        }
#endif

        private void OnDrawGizmosSelected()
        {
            TrajectoryDrawer.DrawTrajectory(transform.position, Direction, Color.green, speed, keepVelocityDistance);
        }

        public virtual void ImportSetData(string objectName, XElement elem)
        {
            speed = HE1Helper.GetFloat(elem, "FirstSpeed");
            keepVelocityDistance = HE1Helper.GetFloat(elem, "KeepVelocityDistance");
            outOfControl = HE1Helper.GetFloat(elem, "OutOfControl");
            isTo3D = HE1Helper.GetBool(elem, "m_IsTo3D");
            cancelBoost = HE1Helper.GetBool(elem, "m_IsStopBoost");
            hasBase = HE1Helper.GetBool(elem, "HasBase");
        }
    }
}
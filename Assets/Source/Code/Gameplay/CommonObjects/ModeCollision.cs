using System;
using SurgeEngine.Source.Code.Core.Character.System;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects
{
    public abstract class ModeCollision : StageObject
    {
        [SerializeField] protected bool isChangeCamera;
        [SerializeField] protected bool isEnabledFromBack = true;
        [SerializeField] protected bool isEnabledFromFront = true;

        private CharacterBase _character;

        public override void OnEnter(Collider msg, CharacterBase context)
        {
            base.OnEnter(msg, context);
            
            if (!CheckFacing(context.transform.forward))
                return;

            _character = context;
        }

        public override void OnExit(Collider msg, CharacterBase context)
        {
            base.OnExit(msg, context);

            _character = null;
        }

        private void OnTriggerStay(Collider other)
        {
            if (_character)
            {
                float dot = Vector3.Dot(transform.forward, _character.transform.forward);
                if (dot > 0)
                {
                    SetMode(_character);
                }
                else
                {
                    RemoveMode(_character);
                }
            }
        }

        protected abstract void SetMode(CharacterBase ctx);
        protected abstract void RemoveMode(CharacterBase ctx);

        private bool CheckFacing(Vector3 dir)
        {
            if (isEnabledFromBack && isEnabledFromFront)
                return true;
            
            float dot = Vector3.Dot(transform.forward, dir);
            
            return isEnabledFromBack && dot > 0 || isEnabledFromFront && dot < 0;
        }
    }

    public class ChangeModeData
    {
        public bool IsCameraChange { get; private set; }
        
        protected ChangeModeData(bool isCameraChange)
        {
            IsCameraChange = isCameraChange;
        }
    }

    [Flags]
    public enum SplineTag
    {
        Default = 0,
        SideView = 1,
        Quickstep = 2,
        DashPath = 4,
        Grind = 8,
    }
}
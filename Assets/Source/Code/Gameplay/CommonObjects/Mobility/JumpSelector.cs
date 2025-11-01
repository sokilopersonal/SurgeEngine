using System;
using DG.Tweening;
using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Infrastructure.Custom;
using SurgeEngine.Source.Code.Infrastructure.Custom.Drawers;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility
{
    [SelectionBase]
    public class JumpSelector : StageObject
    {
        [SerializeField] private float downShotForce = 10f;
        [SerializeField] private float downShotOutOfControl = 0.5f;
        [SerializeField] private float forwardShotForce = 15f;
        [SerializeField] private float forwardShotOutOfControl = 0.25f;
        [SerializeField] private float upShotForce = 20f;
        [SerializeField] private float upShotOutOfControl = 0.75f;
        [SerializeField] private float upShotPitch;
        [SerializeField] private float inputTime = 0.85f;
        [SerializeField] private JumpSelectorButton button;
        public float DownShotForce => downShotForce;
        public float DownShotOutOfControl => downShotOutOfControl;
        public float ForwardShotForce => forwardShotForce;
        public float ForwardShotOutOfControl => forwardShotOutOfControl;
        public float UpShotForce => upShotForce;
        public float UpShotOutOfControl => upShotOutOfControl;
        public float UpShotPitch => upShotPitch;
        public float InputTime => inputTime;
        public JumpSelectorButton Button => button;
        
        public Action<JumpSelectorResultType> OnJumpSelectorResult;

        private Transform _model;
        private Sequence _sequence;

        private void Awake()
        {
            _model = transform.GetChild(0);
        }

        public override void Contact(Collider msg, CharacterBase context)
        {
            var state = context.StateMachine.GetState<FStateJumpSelector>();
            state.AttachJumpSelector(this);
            context.StateMachine.SetState<FStateJumpSelector>();
        }

        public void Rotate(JumpSelectorButton button)
        {
            _sequence?.Kill(true);
            _sequence = DOTween.Sequence();
            float angle = 0;
            switch (button)
            {
                case JumpSelectorButton.A:
                    angle = -90;
                    break;
                case JumpSelectorButton.B:
                    angle = 90;
                    break;
            }

            _sequence.Append(_model.DOLocalRotate(new Vector3(angle, 0, 0), 0.3f));
            _sequence.AppendInterval(0.4f);
            _sequence.Append(_model.DOLocalRotate(new Vector3(0, 0, 0), 0.5f));
        }

        private void OnDrawGizmosSelected()
        {
            // Forward
            TrajectoryDrawer.DrawTrajectory(transform.position, transform.forward, Color.blue, forwardShotForce);
            
            // Down
            TrajectoryDrawer.DrawTrajectory(transform.position, -transform.up, Color.red, downShotForce);
            
            // Up
            TrajectoryDrawer.DrawTrajectory(transform.position, Utility.GetImpulseWithPitch(transform.up, transform.right, upShotPitch, upShotForce), Color.green, upShotForce);
        }
    }

    public enum JumpSelectorResultType
    {
        Start,
        OK,
        Wrong,
        Fall
    }
    
    public enum JumpSelectorButton
    {
        A,
        X,
        B,
        U
    }
}
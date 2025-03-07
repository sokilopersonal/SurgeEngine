using System.Collections;
using SurgeEngine.Code.ActorEffects;
using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.ActorStates.SonicSpecific;
using SurgeEngine.Code.ActorStates.SonicSubStates;
using SurgeEngine.Code.Shaders;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorEffects : ActorComponent
    {
        [Header("Trail")]
        [SerializeField] private VolumeTrailRenderer trailRenderer;
        public SwingEffect swingTrail;

        [Header("Boost")]
        [SerializeField] private Effect boostAura;
        [SerializeField] private BoostDistortion boostDistortion;

        [Header("Spinball")] 
        public Effect spinball;

        [Header("Stomping")]
        public StompEffect stompEffect;

        [Header("Sliding")]
        public Effect slideEffect;
        
        [Header("Grind")]
        public Effect grindEffect;

        [Header("Other")]
        public Effect jumpDeluxEffect;
        public ParaloopEffect paraloopEffect;
        public Effect sweepKickEffect;

        private void Start()
        {
            boostAura.Toggle(false);
            spinball.Toggle(false);
            stompEffect.Toggle(false);
            slideEffect.Toggle(false);
            grindEffect.Toggle(false);
        }

        public void CreateLocus(float time = 0.6f)
        {
            trailRenderer.Emit(time);
        }

        public void DestroyLocus(bool instant = false)
        {
            trailRenderer.Clear(instant);
        }

        private void OnBoostActivate(FSubState obj, bool value)
        {
            if (obj is not FBoost) return;
            
            boostAura.Toggle(value);
            Vector3 viewportPosition = Actor.camera.GetCamera().WorldToViewportPoint(Actor.transform.position);
            viewportPosition.y *= 0.8f; // Correction because the player's pivot is in the head (wtf?)
            if (value) boostDistortion.Play(viewportPosition);
        }

        IEnumerator PlayJumpball()
        {
            yield return new WaitForSeconds(0.117f);
            
            if (Actor.stateMachine.CurrentState is not FStateJump)
                yield break;
            
            if (Actor.input.JumpHeld)
                spinball.Toggle(true);
        }

        private void OnStateAssign(FState obj)
        {
            FStateMachine machine = Actor.stateMachine;
            FState prev = Actor.stateMachine.PreviousState;
            
            if (obj is FStateJump && prev is not FStateUpreel)
            {
                if (prev is FStateJump)
                    spinball.Toggle(true);
                else
                    StartCoroutine(PlayJumpball());
            }
            else
            {
                StopCoroutine(PlayJumpball());
                spinball.Toggle(false);
            }
            grindEffect.Toggle(obj is FStateGrind or FStateGrindSquat);

            if ((obj is FStateGround or FStateStompLand) && prev is FStateStomp)
            {
                stompEffect.Land();
            }
            
            stompEffect.Toggle(obj is FStateStomp);
            slideEffect.Toggle(obj is FStateSlide);

            if (obj is FStateSweepKick)
            {
                sweepKickEffect.Clear();
                sweepKickEffect.Toggle(true);
            }
            else if (prev is FStateSweepKick && (obj is FStateGround or FStateIdle or FStateSit))
            {
                sweepKickEffect.Toggle(false);
            }
            else if (prev is FStateSweepKick)
            {
                sweepKickEffect.Clear();
            }

            if (obj is FStateHoming or FStateStomp or FStateSwingJump)
            {
                CreateLocus();
            }
        }

        public void CreateParaloop()
        {
            paraloopEffect.startPoint = Actor.kinematics.Rigidbody.position;
            paraloopEffect.sonicContext = Actor;
            paraloopEffect.Toggle(true);
        }

        private void OnEnable()
        {
            Actor.stateMachine.GetSubState<FBoost>().OnActiveChanged += OnBoostActivate;
            Actor.stateMachine.OnStateAssign += OnStateAssign;
        }

        private void OnDisable()
        {
            Actor.stateMachine.GetSubState<FBoost>().OnActiveChanged -= OnBoostActivate;
            Actor.stateMachine.OnStateAssign -= OnStateAssign;
        }
    }
}
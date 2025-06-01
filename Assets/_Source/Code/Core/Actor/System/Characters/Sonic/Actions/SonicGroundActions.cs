using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic;
using SurgeEngine.Code.Core.StateMachine;
using SurgeEngine.Code.Gameplay.Inputs;
using SurgeEngine.Code.Infrastructure.Config.SonicSpecific;
using SurgeEngine.Code.Infrastructure.Tools;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.System.Characters.Sonic.Actions
{
    public class SonicGroundActions : ActorActions
    {
        private SlideConfig _slideConfig;
        private QuickStepConfig _quickstepConfig;
        
        protected override void Connect(FStateMachine stateMachine)
        {
            stateMachine.GetState<FStateGround>().SetActions(this);
            
            Actor.TryGetConfig(out _slideConfig);
            Actor.TryGetConfig(out _quickstepConfig);
        }

        public override void Execute()
        {
            base.Execute();
            
            if (!Flags.HasFlag(FlagType.OutOfControl))
            {
                if (!SonicTools.IsBoost())
                {
                    if (Kinematics.Skidding && Kinematics.HorizontalSpeed > 15f)
                    {
                        StateMachine.SetState<FStateBrake>();
                    }
                }

                float minSpeed = _slideConfig.minSpeed;
                minSpeed += minSpeed * 1.5f;
                float dot = Kinematics.MoveDot;
                float abs = Mathf.Abs(dot);
            
                bool readyForDrift = Kinematics.Speed > 5f && abs < 0.4f && !Mathf.Approximately(dot, 0f);
                bool readyForSlide = Kinematics.Speed > minSpeed;

                if (_quickstepConfig)
                {
                    if (Input.LeftBumperPressed)
                    {
                        if (Kinematics.Speed >= _quickstepConfig.minSpeed)
                        {
                            StateMachine.GetState<FStateRunQuickstep>().SetDirection(QuickstepDirection.Left);
                            StateMachine.SetState<FStateRunQuickstep>();
                        }
                        else
                        {
                            StateMachine.GetState<FStateQuickstep>().SetDirection(QuickstepDirection.Left);
                            StateMachine.SetState<FStateQuickstep>();
                        }
                    }
                    else if (Input.RightBumperPressed)
                    {
                        if (Kinematics.Speed >= _quickstepConfig.minSpeed)
                        {
                            StateMachine.GetState<FStateRunQuickstep>().SetDirection(QuickstepDirection.Right);
                            StateMachine.SetState<FStateRunQuickstep>();
                        }
                        else
                        {
                            StateMachine.GetState<FStateQuickstep>().SetDirection(QuickstepDirection.Right);
                            StateMachine.SetState<FStateQuickstep>();
                        }
                    }
                }
                
                if (Input.BHeld)
                {
                    if (readyForSlide && !readyForDrift)
                    {
                        StateMachine.SetState<FStateSlide>();
                    }
                    else if(!readyForSlide && !readyForDrift)
                    {
                        StateMachine.SetState<FStateCrawl>();
                    }

                    if (readyForDrift)
                    {
                        StateMachine.SetState<FStateDrift>();
                    }
                }

                if (SonicInputLayout.DriftHeld)
                {
                    if (readyForDrift)
                    {
                        StateMachine.SetState<FStateDrift>();
                    }
                }
            }
        }
    }
}
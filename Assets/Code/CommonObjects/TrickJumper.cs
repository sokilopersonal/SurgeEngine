using System;
using System.Collections.Generic;
using FMODUnity;
using SurgeEngine.Code.ActorHUD;
using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.ActorStates.SonicSubStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.Parameters;
using SurgeEngine.Code.SurgeDebug;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SurgeEngine.Code.CommonObjects
{
    public class TrickJumper : ContactBase
    {
        [SerializeField] private float firstSpeed = 30f;
        [SerializeField] private float firstPitch = 45f;
        [SerializeField] private float firstOutOfControl = 1f;
        [SerializeField] private float secondSpeed = 45f;
        [SerializeField] private float secondPitch = 50f;
        [SerializeField] private float secondOutOfControl = 1f;
        [SerializeField] private float trickTime1 = 3.5f;
        [SerializeField] private float trickTime2;
        [SerializeField] private float trickTime3;
        [SerializeField, Range(0, 5)] private int trickCount1 = 3;
        [SerializeField, Range(0, 5)] private int trickCount2;
        [SerializeField, Range(0, 5)] private int trickCount3;

        [SerializeField] private EventReference qteHitSound;
        [SerializeField] private EventReference qteSuccessSound;
        [SerializeField] private EventReference qteSuccessVoiceSound;
        [SerializeField] private EventReference qteFailSound;
        [SerializeField] private EventReference qteFailVoiceSound;
        
        private const float TargetTimeScale = 0.045f;
        private const float TimeScaleDuration = 1.125f;
        
        [SerializeField] private Transform startPoint;
        
        public Action<QTEResult> OnQTEResultReceived;
        public event Action OnCorrectButton; 

        private List<QTESequence> _qteSequences;
        private int _buttonId;
        private int _sequenceId;
        private QuickTimeEventUI _currentQTEUI;
        private float timer;

        private void Awake()
        {
            _qteSequences = new List<QTESequence>();
        }

        private void OnEnable()
        {
            OnQTEResultReceived += OnResultReceived;
        }
        
        private void OnDisable()
        {
            OnQTEResultReceived -= OnResultReceived;
        }

        private void Update()
        {
            if (_qteSequences.Count > 0)
            {
                timer -= Time.unscaledDeltaTime;
                if (timer <= 0)
                {
                    OnQTEResultReceived?.Invoke(QTEResult.Fail);
                }
            }
        }
        
        public override async void OnTriggerContact(Collider msg)
        {
            base.OnTriggerContact(msg);

            var context = ActorContext.Context;

            if (context.stateMachine.CurrentState is FStateSpecialJump) return;
            if (firstSpeed > 0)
            {
                context.stateMachine.GetSubState<FBoost>().Active = false;
                Common.ResetVelocity(ResetVelocityType.Both);
                
                context.transform.position = startPoint.position;
                context.transform.forward = Vector3.Cross(-startPoint.right, Vector3.up);
                context.model.transform.forward = Vector3.Cross(-startPoint.right, Vector3.up);
                
                Vector3 impulse = Common.GetImpulseWithPitch(Vector3.Cross(-startPoint.right, Vector3.up), startPoint.right, firstPitch, firstSpeed);
                Common.ApplyImpulse(impulse);
                
                var specialJump = context.stateMachine.SetState<FStateSpecialJump>(0.2f, true, true);
                specialJump.SetSpecialData(new SpecialJumpData(SpecialJumpType.TrickJumper));
                specialJump.PlaySpecialAnimation(0);
                
                context.flags.AddFlag(new Flag(FlagType.OutOfControl, null, false));
                
                await Common.ChangeTimeScaleOverTime(TargetTimeScale, TimeScaleDuration);
                
                CreateQTEUI();
            }
        }

        private void CreateQTEUI()
        {
            _currentQTEUI = Instantiate(ActorHUDContext.Context.GetQTEUI());
            _currentQTEUI.SetTrickJumper(this);

            int[] trickCount = {
                trickCount1,
                trickCount2,
                trickCount3
            };
            float[] trickTime = {
                trickTime1,
                trickTime2,
                trickTime3,
            };
                
            for (int i = 0; i < 3; i++)
            {
                if (trickCount[i] <= 0 || trickTime[i] == 0.0f)
                {
                    break;
                }

                CreateSequence(trickCount[i], trickTime[i]);
            }
                
            _currentQTEUI.CreateButtonIcon(_qteSequences[0]);
                
            ActorContext.Context.input.OnButtonPressed += OnButtonPressed;
        }

        private void CreateSequence(int count, float time)
        {
            QTESequence sequence = new QTESequence
            {
                time = time
            };
            for (int i = 0; i < count; i++)
            {
                var buttonType = (ButtonType)Random.Range(0, (int)ButtonType.COUNT);
                QTEButton button = new QTEButton(buttonType);
                sequence.buttons.Add(button);
            }

            _qteSequences.Add(sequence);
            if (_qteSequences.Count == 1)
            {
                timer = sequence.time;
            }
        }

        private void OnButtonPressed(ButtonType button)
        {
            for (int i = 0; i < _qteSequences.Count; i++)
            {
                var sequence = _qteSequences[_sequenceId];
                ButtonType sequenceButton = sequence.buttons[_buttonId].type;
                if (sequenceButton == button)
                {
                    _buttonId++;
                    OnCorrectButton?.Invoke();
                    RuntimeManager.PlayOneShot(qteHitSound);

                    if (_buttonId >= sequence.buttons.Count) // all buttons pressed
                    {
                        float additionalScore = 1000f * _qteSequences[_sequenceId].time * (1.0f - timer * 0.01f);
                        int score = 1000;
                        Common.AddScore(score + (int)additionalScore);
                        
                        _sequenceId++;
                        RuntimeManager.PlayOneShot(qteSuccessSound);
                        
                        if (_sequenceId >= _qteSequences.Count)
                        {
                            OnQTEResultReceived.Invoke(QTEResult.Success);
                            RuntimeManager.PlayOneShot(qteSuccessVoiceSound);
                        }
                        else
                        {
                            _buttonId = 0;
                            timer = _qteSequences[_sequenceId].time;
                            Destroy(_currentQTEUI.gameObject);
                            _currentQTEUI = Instantiate(ActorHUDContext.Context.GetQTEUI());
                            _currentQTEUI.SetTrickJumper(this);
                            _currentQTEUI.CreateButtonIcon(_qteSequences[_sequenceId]);
                        }
                    }
                    
                    break;
                }

                // you fucked up
                if (sequenceButton != button)
                {
                    OnQTEResultReceived.Invoke(QTEResult.Fail);
                    break;
                }
            }
        }

        private void OnResultReceived(QTEResult result)
        {
            _qteSequences.Clear();
            _buttonId = 0;
            _sequenceId = 0;
            if (_currentQTEUI) Destroy(_currentQTEUI.gameObject);
            
            var context = ActorContext.Context;
            if (result.success)
            {
                context.flags.RemoveFlag(FlagType.OutOfControl);
                context.flags.AddFlag(new Flag(FlagType.OutOfControl, null, true, secondOutOfControl));

                Common.ResetVelocity(ResetVelocityType.Both);
                Vector3 arcPeak = Trajectory.GetArcPosition(startPoint.position,
                    Common.GetCross(transform, firstPitch, true), firstSpeed);
                context.rigidbody.position = arcPeak; // should snap Sonic to the arc point
                context.animation.TransitionToState($"Trick {Random.Range(1, 8)}", 0.2f);
                
                Vector3 impulse = Common.GetImpulseWithPitch(Vector3.Cross(-startPoint.right, Vector3.up), startPoint.right, secondPitch, secondSpeed);
                Common.ApplyImpulse(impulse);
            }
            else
            {
                context.flags.RemoveFlag(FlagType.OutOfControl);
                context.flags.AddFlag(new Flag(FlagType.OutOfControl, null, true, firstOutOfControl));
                
                context.animation.TransitionToState("Air Cycle");
                
                RuntimeManager.PlayOneShot(qteFailSound);
                RuntimeManager.PlayOneShot(qteFailVoiceSound);
            }

            context.stats.movementVector = context.rigidbody.linearVelocity;
            context.stats.planarVelocity = context.rigidbody.linearVelocity;
            
            context.stateMachine.SetState<FStateAir>();
            _ = Common.ChangeTimeScaleOverTime(1, 0.1f);

            context.input.OnButtonPressed -= OnButtonPressed;
        }

        public float GetTimer()
        {
            return timer;
        }

        public QTESequence GetCurrentSequence()
        {
            return _qteSequences[_sequenceId];
        }

        protected override void Draw()
        {
            base.Draw();

            if (startPoint == null) return;
            
            TrajectoryDrawer.DrawTrickTrajectory(startPoint.position, Common.GetCross(startPoint, firstPitch, true), Color.red, firstSpeed);
            Vector3 peakPosition = Trajectory.GetArcPosition(startPoint.position, Common.GetCross(startPoint, firstPitch, true), firstSpeed);
            TrajectoryDrawer.DrawTrickTrajectory(peakPosition, Common.GetCross(startPoint, secondPitch, true), Color.green, secondSpeed);
        }
    }

    public class QTESequence
    {
        public List<QTEButton> buttons = new List<QTEButton>();
        public float time;
    }
    
    public class QTEButton
    {
        public ButtonType type;
        
        public QTEButton(ButtonType type)
        {
            this.type = type;
        }
    }

    public enum ButtonType
    {
        A,
        B,
        X,
        Y,
        LB,
        RB,
        COUNT,
    }

    public struct QTEResult
    {
        public bool success;
        
        public static QTEResult Success => new QTEResult { success = true };
        public static QTEResult Fail => new QTEResult { success = false };
    }
}

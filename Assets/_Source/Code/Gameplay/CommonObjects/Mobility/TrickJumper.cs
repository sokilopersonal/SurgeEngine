using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using SurgeEngine.Code.Core.Actor.HUD;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic.SubStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Infrastructure.Custom;
using SurgeEngine.Code.Infrastructure.Custom.Drawers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Mobility
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
        public event Action<QTESequence> OnNewSequenceStarted;

        private List<QTESequence> _qteSequences;
        private int _buttonId;
        private int _sequenceId;
        private QuickTimeEventUI _currentQTEUI;
        private float timer;
        
        public List<QTESequence> QTESequences => _qteSequences;

        protected override void Awake()
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

        protected override void Update()
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
        
        public override void Contact(Collider msg, ActorBase context)
        {
            base.Contact(msg, context);
            
            StartCoroutine(TrickContact());

            ObjectEvents.OnTrickJumperTriggered?.Invoke(this);
        }

        private IEnumerator TrickContact()
        {
            ActorBase context = ActorContext.Context;

            if (context.stateMachine.CurrentState is FStateSpecialJump) yield break;
            if (firstSpeed > 0)
            {
                context.stateMachine.GetSubState<FBoost>().Active = false;
                context.kinematics.ResetVelocity();
                
                context.PutIn(startPoint.position);
                context.transform.forward = Vector3.Cross(-startPoint.right, Vector3.up);
                context.model.transform.forward = Vector3.Cross(-startPoint.right, Vector3.up);
                
                Vector3 impulse = Common.GetImpulseWithPitch(Vector3.Cross(-startPoint.right, Vector3.up), startPoint.right, firstPitch, firstSpeed);
                context.kinematics.Rigidbody.linearVelocity = impulse;
                
                context.stateMachine.GetState<FStateSpecialJump>().SetSpecialData(new SpecialJumpData(SpecialJumpType.TrickJumper));
                FStateSpecialJump specialJump = context.stateMachine.SetState<FStateSpecialJump>(0.2f, true, true);
                specialJump.PlaySpecialAnimation(0);
                
                context.flags.AddFlag(new Flag(FlagType.OutOfControl, null, false));
                
                yield return SetTime(TargetTimeScale, TimeScaleDuration);
                
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
                
                // Clear previous state
                _buttonId = 0;
                _sequenceId = 0;
                _qteSequences.Clear();
                
                for (int i = 0; i < 3; i++)
                {
                    if (trickCount[i] <= 0 || trickTime[i] == 0.0f)
                    {
                        break;
                    }

                    CreateSequence(trickCount[i], trickTime[i]);
                }
                
                ActorContext.Context.input.OnButtonPressed += OnButtonPressed;
            }
        }

        private IEnumerator SetTime(float target, float duration)
        {
            float startScale = Time.timeScale;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                Time.timeScale = Mathf.Lerp(startScale, target, elapsed / duration);
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
            
            Time.timeScale = target;
        }

        private void CreateSequence(int count, float time)
        {
            QTESequence sequence = new QTESequence
            {
                time = time
            };
            for (int i = 0; i < count; i++)
            {
                ButtonType buttonType = (ButtonType)Random.Range(0, (int)ButtonType.COUNT);
                QTEButton button = new QTEButton(buttonType);
                sequence.buttons.Add(button);
            }

            _qteSequences.Add(sequence);
            if (_qteSequences.Count == 1)
            {
                timer = sequence.time;
                
                // Notify TrickJumperUI about the first sequence
                OnNewSequenceStarted?.Invoke(sequence);
            }
        }

        private void OnButtonPressed(ButtonType button)
        {
            for (int i = 0; i < _qteSequences.Count; i++)
            {
                QTESequence sequence = _qteSequences[_sequenceId];
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
                            
                            // Notify TrickJumperUI about the new sequence
                            OnNewSequenceStarted?.Invoke(_qteSequences[_sequenceId]);
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
            StartCoroutine(OnResultReceivedRoutine(result));
        }

        private IEnumerator OnResultReceivedRoutine(QTEResult result)
        {
            _qteSequences.Clear();
            _buttonId = 0;
            _sequenceId = 0;
            
            ActorBase context = ActorContext.Context;
            Rigidbody body = context.kinematics.Rigidbody;
            if (result.success)
            {
                context.flags.RemoveFlag(FlagType.OutOfControl);
                context.flags.AddFlag(new Flag(FlagType.OutOfControl, null, true, secondOutOfControl));

                context.kinematics.ResetVelocity();
                Vector3 arcPeak = Trajectory.GetArcPosition(startPoint.position,
                    Common.GetCross(transform, firstPitch, true), firstSpeed);
                yield return MoveRigidbodyToArc(body, arcPeak); // should snap Sonic to the arc point
                context.animation.StateAnimator.TransitionToState($"Trick {Random.Range(1, 8)}", 0.2f).Then(() => context.animation.StateAnimator.TransitionToState(AnimatorParams.AirCycle, 0.2f));
                
                Vector3 impulse = Common.GetImpulseWithPitch(Vector3.Cross(-startPoint.right, Vector3.up), startPoint.right, secondPitch, secondSpeed);
                context.kinematics.Rigidbody.linearVelocity = impulse;
            }
            else
            {
                context.flags.RemoveFlag(FlagType.OutOfControl);
                context.flags.AddFlag(new Flag(FlagType.OutOfControl, null, true, firstOutOfControl));
                
                context.animation.StateAnimator.TransitionToState("Air Cycle");
                
                RuntimeManager.PlayOneShot(qteFailSound);
                RuntimeManager.PlayOneShot(qteFailVoiceSound);
            }

            context.stats.movementVector = body.linearVelocity;
            context.stats.planarVelocity = body.linearVelocity;
            
            context.stateMachine.SetState<FStateAir>();
            context.input.OnButtonPressed -= OnButtonPressed;
            
            yield return SetTime(1f, 0.1f);
        }

        private IEnumerator MoveRigidbodyToArc(Rigidbody body, Vector3 arcPeak)
        {
            body.isKinematic = true;
            Vector3 saved = body.position;
            float elapsed = 0f;
            while (elapsed < 1f)
            {
                body.position = Vector3.Lerp(saved, arcPeak, elapsed);
                elapsed += Time.unscaledDeltaTime / 0.05f;
                yield return null;
            }
            
            body.position = arcPeak;
            body.isKinematic = false;
        }

        public float GetTimer()
        {
            return timer;
        }

        public QTESequence GetCurrentSequence()
        {
            return _qteSequences[_sequenceId];
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            
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

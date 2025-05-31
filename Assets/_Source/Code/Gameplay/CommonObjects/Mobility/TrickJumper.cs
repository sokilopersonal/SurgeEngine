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
        private const float MaxFrameTime = 0.33f;
        
        [SerializeField] private float initialSpeed = 30f;
        [SerializeField] private float initialPitch = 45f;
        [SerializeField] private float initialOutOfControl = 1f;
        [SerializeField] private float finalSpeed = 45f;
        [SerializeField] private float finalPitch = 50f;
        [SerializeField] private float finalOutOfControl = 1f;
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
        private float _timer;
        private ActorBase _actor;

        protected override void Awake()
        {
            _qteSequences = new List<QTESequence>();
        }

        private void OnEnable()
        {
            OnQTEResultReceived += HandleQTEResult;
        }
        
        private void OnDisable()
        {
            OnQTEResultReceived -= HandleQTEResult;
        }

        protected override void Update()
        {
            if (_qteSequences.Count > 0)
            {
                float deltaTime = Time.unscaledDeltaTime;
                if (deltaTime < MaxFrameTime)
                {
                    _timer -= deltaTime;
                }
                
                if (_timer <= 0)
                {
                    OnQTEResultReceived?.Invoke(QTEResult.Fail);
                }
            }
        }
        
        public override void Contact(Collider msg, ActorBase context)
        {
            base.Contact(msg, context);
            _actor = context;
            StartCoroutine(PerformTrickContact());
            ObjectEvents.OnTrickJumperTriggered?.Invoke(this);
        }

        private IEnumerator PerformTrickContact()
        {
            if (_actor.StateMachine.CurrentState is FStateSpecialJump) yield break;
            if (initialSpeed > 0)
            {
                _actor.StateMachine.GetSubState<FBoost>().Active = false;
                _actor.Kinematics.ResetVelocity();
                _actor.PutIn(startPoint.position);
                _actor.transform.forward = Vector3.Cross(-startPoint.right, Vector3.up);
                _actor.Model.transform.forward = Vector3.Cross(-startPoint.right, Vector3.up);

                Vector3 impulse = Utility.GetImpulseWithPitch(
                    Vector3.Cross(-startPoint.right, Vector3.up),
                    startPoint.right,
                    initialPitch,
                    initialSpeed
                );
                _actor.Kinematics.Rigidbody.linearVelocity = impulse;
                _actor.StateMachine.GetState<FStateSpecialJump>().SetSpecialData(new SpecialJumpData(SpecialJumpType.TrickJumper));
                _actor.StateMachine.SetState<FStateSpecialJump>(0f, true, true);
                _actor.Flags.AddFlag(new Flag(FlagType.OutOfControl, null, false));

                yield return SetTime(TargetTimeScale, TimeScaleDuration);

                int[] trickCounts = { trickCount1, trickCount2, trickCount3 };
                float[] trickTimes = { trickTime1, trickTime2, trickTime3 };
                InitializeQTESequences(trickCounts, trickTimes);
            }
        }

        private void InitializeQTESequences(int[] trickCounts, float[] trickTimes)
        {
            _buttonId = 0;
            _sequenceId = 0;
            _qteSequences.Clear();

            for (int i = 0; i < 3; i++)
            {
                if (trickCounts[i] <= 0 || trickTimes[i] == 0f)
                {
                    break;
                }
                CreateSequence(trickCounts[i], trickTimes[i]);
            }

            _actor.Input.OnButtonPressed += HandleButtonPressed;
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
            QTESequence sequence = new QTESequence { time = time };
            for (int i = 0; i < count; i++)
            {
                ButtonType buttonType = (ButtonType)Random.Range(0, (int)ButtonType.COUNT);
                QTEButton button = new QTEButton(buttonType);
                sequence.buttons.Add(button);
            }
            _qteSequences.Add(sequence);
            if (_qteSequences.Count == 1)
            {
                _timer = sequence.time;
                OnNewSequenceStarted?.Invoke(sequence);
            }
        }

        private void HandleButtonPressed(ButtonType button)
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

                    if (_buttonId >= sequence.buttons.Count)
                    {
                        float additionalScore = 1000f * _qteSequences[_sequenceId].time * (1.0f - _timer * 0.01f);
                        int score = 1000;
                        Utility.AddScore(score + (int)additionalScore);
                        
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
                            _timer = _qteSequences[_sequenceId].time;
                            OnNewSequenceStarted?.Invoke(_qteSequences[_sequenceId]);
                        }
                    }
                    break;
                }
                OnQTEResultReceived.Invoke(QTEResult.Fail);
                break;
            }
        }

        private void HandleQTEResult(QTEResult result)
        {
            StartCoroutine(HandleQTEResultRoutine(result));
        }

        private IEnumerator HandleQTEResultRoutine(QTEResult result)
        {
            _qteSequences.Clear();
            _buttonId = 0;
            _sequenceId = 0;
            
            Rigidbody body = _actor.Kinematics.Rigidbody;
            if (result.success)
            {
                _actor.Flags.RemoveFlag(FlagType.OutOfControl);
                _actor.Flags.AddFlag(new Flag(FlagType.OutOfControl, null, true, finalOutOfControl));
                _actor.Kinematics.ResetVelocity();

                Vector3 arcPeak = Trajectory.GetArcPosition(
                    startPoint.position,
                    Utility.GetCross(transform, initialPitch, true),
                    initialSpeed
                );
                _actor.StateMachine.SetState<FStateTrick>().SetTimer(finalOutOfControl);
                yield return MoveRigidbodyToArc(body, arcPeak);

                Vector3 impulse = Utility.GetImpulseWithPitch(
                    Vector3.Cross(-startPoint.right, Vector3.up),
                    startPoint.right,
                    finalPitch,
                    finalSpeed
                );
                _actor.Kinematics.Rigidbody.linearVelocity = impulse;
            }
            else
            {
                _actor.Flags.RemoveFlag(FlagType.OutOfControl);
                _actor.Flags.AddFlag(new Flag(FlagType.OutOfControl, null, true, initialOutOfControl));
                _actor.StateMachine.SetState<FStateAir>();
                
                RuntimeManager.PlayOneShot(qteFailSound);
                RuntimeManager.PlayOneShot(qteFailVoiceSound);
            }

            _actor.Input.OnButtonPressed -= HandleButtonPressed;
            _actor = null;
            
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
            return _timer;
        }

        public QTESequence GetCurrentSequence()
        {
            return _qteSequences[_sequenceId];
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (startPoint == null) return;
            
            TrajectoryDrawer.DrawTrickTrajectory(
                startPoint.position,
                Utility.GetCross(startPoint, initialPitch, true),
                Color.red,
                initialSpeed
            );
            Vector3 peakPosition = Trajectory.GetArcPosition(
                startPoint.position,
                Utility.GetCross(startPoint, initialPitch, true),
                initialSpeed
            );
            TrajectoryDrawer.DrawTrickTrajectory(
                peakPosition,
                Utility.GetCross(startPoint, finalPitch, true),
                Color.green,
                finalSpeed
            );
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
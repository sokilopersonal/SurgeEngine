using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using SurgeEngine.Source.Code.Core.Character.HUD;
using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic.SubStates;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Infrastructure.Custom;
using SurgeEngine.Source.Code.Infrastructure.Custom.Drawers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility
{
    public class TrickJumper : StageObject
    {
        private const float MaxFrameTime = 0.33f;
        
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
        
        private const float TargetTimeScale = 0.01f;
        private const float TimeScaleDuration = 1.125f;
        private Vector3 StartPosition => transform.position + Vector3.up;

        public Action<QTEResult> OnQTEResultReceived;
        public event Action OnCorrectButton;
        public event Action<QTESequence> OnNewSequenceStarted;
        private List<QTESequence> _qteSequences;
        private int _buttonId;
        private int _sequenceId;
        private QuickTimeEventUI _currentQTEUI;
        private float _timer;
        private QTESequence _finishingSequence;
        private CharacterBase _character;

        private void Awake()
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

        private void Update()
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
        
        public override void OnEnter(Collider msg, CharacterBase context)
        {
            base.OnEnter(msg, context);
            _character = context;
            StartCoroutine(PerformTrickContact());
            ObjectEvents.OnTrickJumperTriggered?.Invoke(this);
        }

        private IEnumerator PerformTrickContact()
        {
            if (_character.StateMachine.CurrentState is FStateTrickJump) yield break;
            if (firstSpeed > 0)
            {
                if (_character.StateMachine.GetState(out FBoost boost))
                    boost.Active = false;
                
                _character.Kinematics.ResetVelocity();
                _character.Rigidbody.position = StartPosition;
                _character.transform.forward = Vector3.Cross(-transform.right, Vector3.up);
                _character.Model.Root.forward = Vector3.Cross(-transform.right, Vector3.up);

                Vector3 impulse = Utility.GetImpulseWithPitch(
                    -transform.forward,
                    transform.right,
                    firstPitch,
                    firstSpeed
                );
                
                _character.Kinematics.Rigidbody.linearVelocity = impulse;
                _character.StateMachine.SetState<FStateTrickJump>(true);
                _character.Flags.AddFlag(new Flag(FlagType.OutOfControl, false));

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

            _character.Input.OnButtonPressed += HandleButtonPressed;
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
            for (int i = 0; i < _qteSequences.Count;)
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
            _finishingSequence = _qteSequences[_sequenceId-1];
            _qteSequences.Clear();
            _buttonId = 0;
            _sequenceId = 0;
            
            Rigidbody body = _character.Kinematics.Rigidbody;
            if (result.success)
            {
                _character.Flags.RemoveFlag(FlagType.OutOfControl);
                _character.Flags.AddFlag(new Flag(FlagType.OutOfControl, true, secondOutOfControl));
                _character.Kinematics.ResetVelocity();

                Vector3 arcPeak = Trajectory.GetArcPosition(
                    StartPosition,
                    Utility.GetCross(transform, firstPitch, true),
                    firstSpeed
                );
                _character.StateMachine.SetState<FStateTrick>().SetTimer(secondOutOfControl);
                yield return MoveRigidbodyToArc(body, arcPeak);

                Vector3 impulse = Utility.GetImpulseWithPitch(
                    Vector3.Cross(-transform.right, Vector3.up),
                    transform.right,
                    secondPitch,
                    secondSpeed
                );
                _character.Kinematics.Rigidbody.linearVelocity = impulse;
            }
            else
            {
                _character.Flags.RemoveFlag(FlagType.OutOfControl);
                _character.Flags.AddFlag(new Flag(FlagType.OutOfControl, true, firstOutOfControl));
                _character.StateMachine.SetState<FStateAir>();
                
                RuntimeManager.PlayOneShot(qteFailSound);
                RuntimeManager.PlayOneShot(qteFailVoiceSound);
            }

            _character.Input.OnButtonPressed -= HandleButtonPressed;
            _character = null;
            
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

        public QTESequence GetFinishingSequence()
        {
            return _finishingSequence;
        }

        private void OnDrawGizmosSelected()
        {
            TrajectoryDrawer.DrawTrickTrajectory(
                StartPosition,
                Utility.GetCross(transform, firstPitch, true),
                Color.red,
                firstSpeed
            );
            Vector3 peakPosition = Trajectory.GetArcPosition(
                StartPosition,
                Utility.GetCross(transform, firstPitch, true),
                firstSpeed
            );
            TrajectoryDrawer.DrawTrickTrajectory(
                peakPosition,
                Utility.GetCross(transform, secondPitch, true),
                Color.green,
                secondSpeed
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
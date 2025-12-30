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
    public class ReactionPanel : StageObject
    {
        private const float MaxFrameTime = 0.33f;

        [SerializeField] private float firstSpeed = 30f;
        [SerializeField] private float firstPitch = 45f;
        [SerializeField] private float firstOutOfControl = 1f;
        [SerializeField] private float secondSpeed = 45f;
        [SerializeField] private float secondPitch = 50f;
        [SerializeField] private float secondOutOfControl = 1f;
        [SerializeField] private float trickTime = 3.5f;

        [SerializeField] private EventReference qteHitSound;
        [SerializeField] private EventReference qteSuccessSound;
        [SerializeField] private EventReference qteSuccessVoiceSound;
        [SerializeField] private EventReference qteFailSound;
        [SerializeField] private EventReference qteFailVoiceSound;
        private Vector3 StartPosition => transform.position + Vector3.up;

        public Action<QTEResult> OnQTEResultReceived;
        public event Action OnCorrectButton;
        public event Action<QTESequence> OnNewSequenceStarted;
        private QTESequence _qteSequence = null;
        private float _timer;
        private QTESequence _finishingSequence;
        private CharacterBase _character;

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
            if (_qteSequence != null)
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
            ObjectEvents.OnReactionPanelTriggered?.Invoke(this);
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

                Vector3 impulse = Utility.GetImpulseWithPitch(
                    -transform.forward,
                    transform.right,
                    firstPitch,
                    firstSpeed
                );

                _character.Kinematics.Rigidbody.linearVelocity = impulse;
                _character.StateMachine.SetState<FStateTrickJump>(true);
                _character.Flags.AddFlag(new Flag(FlagType.OutOfControl, false));

                yield return new WaitForEndOfFrame();
                
                InitializeQTESequences(trickTime);
            }
        }

        private void InitializeQTESequences(float _trickTime)
        {
            CreateSequence(_trickTime);

            _character.Input.OnButtonPressed += HandleButtonPressed;
        }

        private void CreateSequence(float time)
        {
            _qteSequence = new QTESequence { time = time };
            
            ButtonType buttonType = (ButtonType)Random.Range(0, 3);
            QTEButton button = new QTEButton(buttonType);

            _qteSequence.buttons.Add(button);

            _timer = _qteSequence.time;
            OnNewSequenceStarted?.Invoke(_qteSequence);
        }

        private void HandleButtonPressed(ButtonType button)
        {
            ButtonType sequenceButton = _qteSequence.buttons[0].type;
            if (sequenceButton == button)
            {
                OnCorrectButton?.Invoke();
                RuntimeManager.PlayOneShot(qteHitSound);

                float additionalScore = 1000f * _qteSequence.time * (1.0f - _timer * 0.01f);
                int score = 1000;
                Utility.AddScore(score + (int)additionalScore);

                RuntimeManager.PlayOneShot(qteSuccessSound);

                OnQTEResultReceived.Invoke(QTEResult.Success);
                RuntimeManager.PlayOneShot(qteSuccessVoiceSound);
            }
            else
            {
                OnQTEResultReceived.Invoke(QTEResult.Fail);
            }
        }

        private void HandleQTEResult(QTEResult result)
        {
            StartCoroutine(HandleQTEResultRoutine(result));
        }

        private IEnumerator HandleQTEResultRoutine(QTEResult result)
        {
            _finishingSequence = _qteSequence;
            _qteSequence = null;

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
            return _qteSequence;
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
}
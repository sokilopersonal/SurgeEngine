using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using SurgeEngine.Code.ActorHUD;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.Parameters;
using SurgeEngine.Code.Parameters.SonicSubStates;
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

        [SerializeField] private Collider box;
        [SerializeField] private Collider impulseBox;
        
        [SerializeField] private EventReference qteHitSound;
        [SerializeField] private EventReference qteSuccessSound;
        [SerializeField] private EventReference qteEndSuccessSound;
        [SerializeField] private EventReference qteFailSound;
        
        private float _targetTimeScale = 0.045f;
        private float _timeScaleDuration = 1f;
        
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
            
            _targetTimeScale = 0.045f;
            _timeScaleDuration = 1.1f;
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
                    RuntimeManager.PlayOneShot(qteFailSound);
                }
            }
        }
        
        public override void OnTriggerContact(Collider msg)
        {
            base.OnTriggerContact(msg);

            var context = ActorContext.Context;
            if (firstSpeed > 0)
            {
                box.enabled = false;
                impulseBox.enabled = false;
                
                context.stateMachine.GetSubState<FBoost>().Active = false;
                
                context.transform.position = transform.position + Vector3.up * 1.25f;
                context.transform.forward = Vector3.Cross(-startPoint.right, Vector3.up);
                context.model.transform.forward = Vector3.Cross(-startPoint.right, Vector3.up);
                
                Vector3 impulse = Common.GetImpulseWithPitch(Vector3.Cross(-startPoint.right, Vector3.up), startPoint.right, firstPitch, firstSpeed);
                Common.ApplyImpulse(impulse);
                
                var specialJump = context.stateMachine.CurrentState is FStateSpecialJump ? 
                    context.stateMachine.GetState<FStateSpecialJump>() : context.stateMachine.SetState<FStateSpecialJump>();
                specialJump.SetSpecialData(new SpecialJumpData(SpecialJumpType.TrickJumper));
                specialJump.PlaySpecialAnimation(0);
                
                context.flags.AddFlag(new Flag(FlagType.OutOfControl, null));
                context.flags.AddFlag(new Flag(FlagType.DontClampVerticalSpeed, null));
                
                StartCoroutine(ChangeTimeScaleOverTime(_targetTimeScale, _timeScaleDuration));
            }
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
                        _sequenceId++;
                        RuntimeManager.PlayOneShot(qteSuccessSound);
                        
                        if (_sequenceId >= _qteSequences.Count)
                        {
                            OnQTEResultReceived.Invoke(QTEResult.Success);
                            RuntimeManager.PlayOneShot(qteEndSuccessSound);
                        }
                        else
                        {
                            _buttonId = 0;
                            timer = _qteSequences[_sequenceId].time;
                            Destroy(_currentQTEUI.gameObject);
                            _currentQTEUI = Instantiate(ActorStageHUD.Context.GetQTEUI());
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
                    RuntimeManager.PlayOneShot(qteFailSound);
                    break;
                }
            }
        }

        private IEnumerator ChangeTimeScaleOverTime(float targetScale, float duration)
        {
            float startScale = Time.timeScale;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                Time.timeScale = Mathf.Lerp(startScale, targetScale, elapsed / duration);
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            if (Mathf.Approximately(targetScale, _targetTimeScale))
            {
                _currentQTEUI = Instantiate(ActorStageHUD.Context.GetQTEUI());
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
            
            Time.timeScale = targetScale;
        }

        private IEnumerator ChangeRigidbodyPositionOverTime(Rigidbody rigidbody, Vector3 targetPosition, float duration)
        {
            Vector3 startPosition = rigidbody.position;
            float elapsed = 0f;
            rigidbody.isKinematic = true;
            
            while (elapsed < duration)
            {
                rigidbody.position = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            rigidbody.isKinematic = false;
            Common.ResetVelocity(ResetVelocityType.Both);
        }

        private void OnResultReceived(QTEResult result)
        {
            StartCoroutine(OnResultReceivedCoroutine(result));
        }

        private IEnumerator OnResultReceivedCoroutine(QTEResult result)
        {
            _qteSequences.Clear();
            _buttonId = 0;
            _sequenceId = 0;
            if (_currentQTEUI) Destroy(_currentQTEUI.gameObject);
            
            box.enabled = true;
            impulseBox.enabled = true;
            
            var context = ActorContext.Context;
            if (result.success)
            {
                context.flags.RemoveFlag(FlagType.OutOfControl);
                context.flags.AddFlag(new Flag(FlagType.OutOfControl, null, true, secondOutOfControl));

                Common.ResetVelocity(ResetVelocityType.Both);
                Vector3 arcPeak = Common.GetArcPosition(startPoint.position,
                    Common.GetCross(transform, firstPitch, true), firstSpeed);
                context.rigidbody.position = arcPeak;
                //yield return StartCoroutine(ChangeRigidbodyPositionOverTime(context.rigidbody, arcPeak, 0.05f)); // should snap Sonic to the arc point
                context.animation.TransitionToState($"Trick {Random.Range(1, 8)}", 0f);
                
                Vector3 impulse = Common.GetImpulseWithPitch(Vector3.Cross(-startPoint.right, Vector3.up), startPoint.right, secondPitch, secondSpeed);
                Common.ApplyImpulse(impulse);
            }
            else
            {
                context.flags.RemoveFlag(FlagType.OutOfControl);
                context.flags.AddFlag(new Flag(FlagType.OutOfControl, null, true, firstOutOfControl));
                
                context.animation.TransitionToState("Air Cycle");
            }

            context.stats.movementVector = context.rigidbody.linearVelocity;
            context.stats.planarVelocity = context.rigidbody.linearVelocity;
            
            context.stateMachine.SetState<FStateAir>();
            StartCoroutine(ChangeTimeScaleOverTime(1f, 0.3f));

            context.input.OnButtonPressed -= OnButtonPressed;

            yield return null;
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
            Vector3 peakPosition = Common.GetArcPosition(startPoint.position, Common.GetCross(startPoint, firstPitch, true), firstSpeed);
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
        COUNT
    }

    public struct QTEResult
    {
        public bool success;
        
        public static QTEResult Success => new QTEResult { success = true };
        public static QTEResult Fail => new QTEResult { success = false };
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private float trickTime1 = 3f;
        [SerializeField] private float trickTime2;
        [SerializeField] private float trickTime3;
        [SerializeField] private int trickCount1 = 3;
        [SerializeField] private int trickCount2;
        [SerializeField] private int trickCount3;
        
        [SerializeField] private QuickTimeEventUI qteUI;
        
        private float _targetTimeScale = 0.045f;
        private float _timeScaleDuration = 1f;
        
        [SerializeField] private Transform startPoint;
        
        public Action<QTEResult> OnQTEResultReceived;
        public event Action OnCorrectButton; 

        private List<QTESequence> _qteSequences;
        private int _buttonId;
        private QuickTimeEventUI _currentQTEUI;
        private float timer;

        private void Awake()
        {
            _qteSequences = new List<QTESequence>();
            
            _targetTimeScale = 0.055f;
            _timeScaleDuration = 1.15f;
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
            // if (_qteActive)
            // {
            //     if (ActorContext.Context.input.JumpPressed)
            //     {
            //         OnQTEResultReceived.Invoke(QTEResult.Success);
            //     }
            //
            //     if (ActorContext.Context.input.BoostPressed)
            //     {
            //         OnQTEResultReceived.Invoke(QTEResult.Fail);
            //     }
            // }

            if (_qteSequences.Count > 0)
            {
                timer -= Time.unscaledDeltaTime;
                if (timer <= 0)
                {
                    OnQTEResultReceived?.Invoke(QTEResult.Fail);
                }
            }
        }

        public override void OnTriggerContact(Collider msg)
        {
            base.OnTriggerContact(msg);

            var context = ActorContext.Context;
            if (firstSpeed > 0)
            {
                context.stateMachine.GetSubState<FBoost>().Active = false;
                
                context.transform.position = startPoint.position;
                context.transform.forward = Vector3.Cross(-startPoint.right, Vector3.up);
                context.model.transform.forward = Vector3.Cross(-startPoint.right, Vector3.up);
                Common.ApplyImpulse(Common.GetImpulseWithPitch(transform, firstPitch, firstSpeed));
                
                var specialJump = context.stateMachine.CurrentState is FStateSpecialJump ? 
                    context.stateMachine.GetState<FStateSpecialJump>() : context.stateMachine.SetState<FStateSpecialJump>();
                specialJump.SetSpecialData(new SpecialJumpData(SpecialJumpType.TrickJumper));
                specialJump.PlaySpecialAnimation(0);
                
                context.flags.AddFlag(new Flag(FlagType.OutOfControl, null));
                //context.camera.SetRotationAxis(-transform.forward);
                
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
            Debug.Log(_qteSequences.Count);
            _currentQTEUI.CreateButtonIcon(sequence);
            timer = sequence.time;
        }

        private void OnButtonPressed(ButtonType button)
        {
            foreach (var sequence in _qteSequences)
            {
                ButtonType sequenceButton = sequence.buttons[_buttonId].type;
                if (sequenceButton == button)
                {
                    _buttonId++;
                    
                    OnCorrectButton?.Invoke();

                    if (_buttonId >= sequence.buttons.Count)
                    {
                        OnQTEResultReceived.Invoke(QTEResult.Success);
                    }
                    
                    break;
                }

                if (sequenceButton != button)
                {
                    OnQTEResultReceived.Invoke(QTEResult.Fail);
                    break;
                }
            }
        }

        public float GetTimer()
        {
            return timer;
        }

        public QTESequence GetCurrentSequence()
        {
            return _qteSequences[^1];
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
                _currentQTEUI = Instantiate(qteUI);
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
            var context = ActorContext.Context;
            if (result.success)
            {
                context.flags.RemoveFlag(FlagType.OutOfControl);
                context.flags.AddFlag(new Flag(FlagType.OutOfControl, null, true, secondOutOfControl));

                Vector3 arcPeak = Common.GetArcPosition(startPoint.position,
                    Common.GetCross(transform, firstPitch, true), firstSpeed);
                context.rigidbody.position = arcPeak;
                //yield return StartCoroutine(ChangeRigidbodyPositionOverTime(context.rigidbody, arcPeak, 0.02f));
                context.animation.TransitionToState($"Trick {Random.Range(1, 8)}", 0f);
                
                Common.ApplyImpulse(Common.GetImpulseWithPitch(transform, secondPitch, secondSpeed));
            }
            else
            {
                context.flags.RemoveFlag(FlagType.OutOfControl);
                context.flags.AddFlag(new Flag(FlagType.OutOfControl, null, true, firstOutOfControl));
                
                context.animation.TransitionToState("Air Cycle");
            }
            
            _qteSequences.Clear();
            _buttonId = 0;
            
            context.stateMachine.SetState<FStateAir>();
            StartCoroutine(ChangeTimeScaleOverTime(1f, 0.3f));

            context.input.OnButtonPressed -= OnButtonPressed;
            if (_currentQTEUI) Destroy(_currentQTEUI.gameObject);

            yield return null;
        }

        protected override void Draw()
        {
            base.Draw();

            if (startPoint == null) return;
            
            TrajectoryDrawer.DrawTrickTrajectory(startPoint.position, Common.GetCross(transform, firstPitch, true), Color.red, firstSpeed);
            Vector3 peakPosition = Common.GetArcPosition(startPoint.position, Common.GetCross(transform, firstPitch, true), firstSpeed);
            TrajectoryDrawer.DrawTrickTrajectory(peakPosition, Common.GetCross(transform, secondPitch, true), Color.green, secondSpeed);
        }
    }

    public class QTESequence
    {
        public List<QTEButton> buttons = new List<QTEButton>();
        public float time;
        private float timer;

        public void Calculate(float dt)
        {
            timer += dt;
        }
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

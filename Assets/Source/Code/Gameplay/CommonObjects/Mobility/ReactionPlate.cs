using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using FMODUnity;
using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Infrastructure.Custom;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility
{
    public enum ReactionPlateType
    {
        Spring,
        Panel,
        End
    }
    
    public class ReactionPlate : StageObject
    {
        private const float MaxFrameTime = 0.33f;
        
        [SerializeField] private float mainAcceptingTime = 0.5f;
        [SerializeField] private float preAcceptingTime = 1;
        [SerializeField] private ReactionPlate target;
        [SerializeField] private ReactionPlateType type;
        [SerializeField] private float failOutOfControlTime = 0.5f;
        [SerializeField] private float jumpMaxVelocity;
        [SerializeField] private float jumpMinVelocity;
        public ReactionPlate Target => target;
        public ReactionPlateType Type => type;

        [SerializeField] private EventReference qteHitSound;
        [SerializeField] private EventReference qteSuccessSound;
        [SerializeField] private EventReference qteSuccessVoiceSound;
        [SerializeField] private EventReference qteFailSound;
        [SerializeField] private EventReference qteFailVoiceSound;

        public Action<QTEResult> OnQTEResultReceived;
        public event Action OnCorrectButton;
        public event Action<QTESequence> OnNewSequenceStarted;
        private QTESequence _qteSequence;
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

            if (type != ReactionPlateType.End)
            {
                if (type == ReactionPlateType.Spring)
                {
                    Launch(context);
                }
            }
        }

        public void PerformTrickContact(CharacterBase context)
        {
            _character = context;
            ObjectEvents.OnReactionPanelTriggered?.Invoke(this);
            
            if (target)
            {
                context.Kinematics.ResetVelocity();
                
                InitializeQTESequences(preAcceptingTime);
            }
        }

        private void InitializeQTESequences(float trickTime)
        {
            CreateSequence(trickTime);

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

        private async void HandleQTEResult(QTEResult result)
        {
            _finishingSequence = _qteSequence;
            _qteSequence = null;

            if (result.success)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(mainAcceptingTime), DelayType.DeltaTime);
                
                Launch(_character);
            }
            else
            {
                _character.Flags.RemoveFlag(FlagType.OutOfControl);
                _character.Flags.AddFlag(new Flag(FlagType.OutOfControl, true, failOutOfControlTime));
                _character.StateMachine.SetState<FStateAir>();

                RuntimeManager.PlayOneShot(qteFailSound);
                RuntimeManager.PlayOneShot(qteFailVoiceSound);
            }

            _character.Input.OnButtonPressed -= HandleButtonPressed;
            _character = null;
        }

        private void Launch(CharacterBase context)
        {
            var st = context.StateMachine;
            var plateJump = st.GetState<FStateReactionPlateJump>();
            var info = new ReactionPlateJumpInfo(context.transform.position, target, jumpMaxVelocity, jumpMinVelocity);
            plateJump.SetInfo(info);
            st.SetState<FStateReactionPlateJump>();
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
            if (target)
            {
                
            }
        }
    }
}
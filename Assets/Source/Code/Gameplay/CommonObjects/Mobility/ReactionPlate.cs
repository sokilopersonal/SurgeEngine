using System;
using Cysharp.Threading.Tasks;
using FMODUnity;
using JetBrains.Annotations;
using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Core.StateMachine;
using SurgeEngine.Source.Code.Infrastructure.Custom;
using UnityEngine;
using Random = UnityEngine.Random;
using NaughtyAttributes;
using UnityEditor;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility
{
    public enum ReactionPlateType
    {
        Spring,
        Plate,
        End
    }

    public enum ReactionPlateButton
    {
        B = 1,
        A = 0,
        X = 2,
        Y = 3,
        Random = 4
    }
    
    [ExecuteAlways]
    public class ReactionPlate : StageObject
    {
        private const float MaxFrameTime = 0.33f;

        [Header("General Settings")]
        [SerializeField] private ReactionPlateType type;
        [SerializeField] private ReactionPlateButton buttonType;
        [SerializeField] private ReactionPlate target;

        [Header("QTE Settings")]
        [SerializeField] private float mainAcceptingTime = 0.5f;
        [SerializeField] private float preAcceptingTime = 1;
        [SerializeField] private float failOutOfControlTime = 0.5f;

        [Header("Launch Settings")]
        [SerializeField] private float jumpMaxVelocity;

        [Header("Sound References")]
        [SerializeField] private EventReference qteTouchSound;
        [SerializeField] private EventReference qteSuccessSound;
        [SerializeField] private EventReference qteFailSound;

        private bool ShowPlate() => type != ReactionPlateType.Spring;

        public ReactionPlate Target => target;
        public ReactionPlateType Type => type;
        public static float Velocity => 30f;

        public Action<QTEResult> OnQTEResultReceived;
        public event Action OnCorrectButton;
        public event Action<QTESequence> OnNewSequenceStarted;
        private QTESequence _qteSequence;
        private float _timer;
        private QTESequence _finishingSequence;
        private CharacterBase _character;
        private Material _material;
        private ReactionPlateJumpInfo _info;
        private bool _countdown;

        private void OnEnable()
        {
            OnQTEResultReceived += HandleQTEResult;
        }

        private void OnDisable()
        {
            _material = null;
            OnQTEResultReceived -= HandleQTEResult;
        }

        private void Update()
        {
            UpdateMesh();

            if (Application.isPlaying)
            {
                if (_qteSequence != null)
                {
                    float deltaTime = Time.deltaTime;
                    if (deltaTime < MaxFrameTime && _countdown)
                    {
                        _timer -= deltaTime;
                    }

                    if (_timer <= 0)
                    {
                        OnQTEResultReceived?.Invoke(QTEResult.Fail);
                    }
                }

                if (_material != null)
                    _material.SetFloat("_InputDevice", (int)CharacterContext.Context.Input.GetDevice());
            }
        }

#if UNITY_EDITOR
        Transform springRenderer => transform.Find("Spring");
        Transform plateRenderer => transform.Find("Plate");
        Collider col => GetComponent<Collider>();
        private void UpdateMeshEditor()
        {
            col.enabled = type == ReactionPlateType.Spring;

            if (springRenderer && plateRenderer)
            {
                springRenderer.gameObject.SetActive(type == ReactionPlateType.Spring && type != ReactionPlateType.End);
                plateRenderer.gameObject.SetActive(type == ReactionPlateType.Plate && type != ReactionPlateType.End);
            }

            MeshRenderer mesh = plateRenderer.GetComponentInChildren<MeshRenderer>();
            Material materialTemplate = AssetDatabase.LoadAssetAtPath<Material>("Assets/Source/Materials/HE1/CommonObjects/ReactionPanel/GlassPanel.mat");

            if (mesh == null || materialTemplate == null)
                return;

            if (mesh.sharedMaterials.Length != 2)
                return;

            if (_material == null)
                _material = new Material(materialTemplate);

            Material[] mats = mesh.sharedMaterials;
            mats[1] = _material;
            mesh.sharedMaterials = mats;
        }

        private void OnValidate()
        {
            UpdateMeshEditor();
            UpdateMesh();
        }
#endif
        private void UpdateMesh()
        {
            if (_material == null)
                return;
            
            _material.SetFloat("_ButtonFace", (int)buttonType);
            _material.SetFloat("_InputDevice", 1);
        }

        public override void OnEnter(Collider msg, CharacterBase context)
        {
            base.OnEnter(msg, context);

            if (type != ReactionPlateType.End)
            {
                if (type == ReactionPlateType.Spring)
                {
                    RuntimeManager.PlayOneShot(qteSuccessSound);
                    Launch(context);
                }
            }
        }

        public void PerformTrickContact(CharacterBase context)
        {
            if (target)
            {
                RuntimeManager.PlayOneShot(qteTouchSound);
                context.Kinematics.ResetVelocity();
            }
        }

        private async void InitializeQTESequences(float trickTime)
        {
            _countdown = false;

            float time = Vector3.Distance(_info.start, _info.target.transform.position) / Velocity;
            
            await UniTask.Delay(TimeSpan.FromSeconds(time * 0.75f), DelayType.DeltaTime);
            
            ObjectEvents.OnReactionPanelTriggered?.Invoke(this);
            CreateSequence(trickTime);
            _character.Input.OnButtonPressed += HandleButtonPressed;

            await UniTask.Delay(TimeSpan.FromSeconds(0.13f), DelayType.DeltaTime);
            
            _countdown = true;
        }

        private void CreateSequence(float time)
        {
            _qteSequence = new QTESequence { time = time };
            
            ButtonType _buttonType = buttonType.Equals(ReactionPlateButton.Random) ? (ButtonType)Random.Range(0, 3) : (ButtonType)buttonType;
            QTEButton button = new QTEButton(_buttonType);

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

                float additionalScore = 1000f * _qteSequence.time * (1.0f - _timer * 0.01f);
                int score = 1000;
                Utility.AddScore(score + (int)additionalScore);

                OnQTEResultReceived.Invoke(QTEResult.Success);
                
                RuntimeManager.PlayOneShot(qteSuccessSound);
            }
            else
            {
                OnQTEResultReceived.Invoke(QTEResult.Fail);
            }
        }

        private async void HandleQTEResult(QTEResult result)
        {
            _character.Input.OnButtonPressed -= HandleButtonPressed;
            _finishingSequence = _qteSequence;
            _qteSequence = null;

            if (result.success)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(mainAcceptingTime), DelayType.DeltaTime);
                
                Launch(_character);

                RuntimeManager.PlayOneShot(qteSuccessSound);
            }
            else
            {
                if (_timer > 0.0f)
                    await UniTask.Delay(TimeSpan.FromSeconds(mainAcceptingTime), DelayType.DeltaTime);

                _character.Flags.RemoveFlag(FlagType.OutOfControl);
                _character.Flags.AddFlag(new Flag(FlagType.OutOfControl, true, failOutOfControlTime));
                _character.StateMachine.SetState<FStateAir>();

                RuntimeManager.PlayOneShot(qteFailSound);
            }

            _character = null;
        }

        private void Launch(CharacterBase context)
        {
            target._character = context;
            _character = context;

            FStateMachine st = context.StateMachine;
            FStateReactionPlateJump plateJump = st.GetState<FStateReactionPlateJump>();
            ReactionPlateJumpInfo info = new ReactionPlateJumpInfo(context.transform.position, target);
            
            target._info = info;
            _info = info;
            
            plateJump.SetInfo(info);
            
            st.SetState<FStateReactionPlateJump>();

            if (target.type == ReactionPlateType.Plate)
                target.InitializeQTESequences(target.preAcceptingTime);
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
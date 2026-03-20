using System;
using System.Collections;
using System.Collections.Generic;
using Alchemy.Inspector;
using FMODUnity;
using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.System;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects
{
    public class HintRing : StageObject, IPointMarkerLoader
    {
        [Serializable]
        public class HintMessage
        {
            [Tooltip("Determines if the player has control while the hint is displayed")] public bool outOfControl;
            [Multiline] public string message = "Hello World!";
            [Tooltip("How long the hint will be seen on screen")] public float messageDuration = 2f;
            [Tooltip("Set to zero to disable text animation")] public float animationDuration = 0.5f;
        }

        [Title("General")]
        [SerializeField] private List<HintMessage> messages = new List<HintMessage>();
        [Tooltip("The time until the hint ring can be activated again")][SerializeField] private float cooldown = 2f;

        [FoldoutGroup("Visuals")]
        [SerializeField] private GameObject model;
        [FoldoutGroup("Visuals")]
        [SerializeField] private Animator animator;

        [FoldoutGroup("Sound")]
        [SerializeField] private EventReference hintSound;
        [FoldoutGroup("Sound")]
        [SerializeField] private EventReference appearSound;
        [FoldoutGroup("Sound")]
        [SerializeField] private EventReference disappearSound;

        private float _timer;
        private bool _activated;
        private bool _isCurrent;
        private Coroutine _messagesCoroutine;

        public HintMessage CurrentMessage { get; private set; }

        private void OnEnable()
        {
            ObjectEvents.OnHintTriggered += OnTriggered;
        }

        private void OnDisable()
        {
            ObjectEvents.OnHintTriggered -= OnTriggered;
        }

        private void Update()
        {
            if (_timer > 0f)
            {
                _timer -= Time.deltaTime;

                if (_timer <= 0f)
                    Show();
            }
        }

        public override void OnEnter(Collider msg, CharacterBase context)
        {
            base.OnEnter(msg, context);
            
            if (_activated) return;

            Hide();
            _messagesCoroutine = StartCoroutine(PlayMessages(context));
        }

        private IEnumerator PlayMessages(CharacterBase context)
        {
            for (int i = 0; i < messages.Count; i++)
            {
                CurrentMessage = messages[i];

                ObjectEvents.OnHintTriggered?.Invoke(this);

                if (CurrentMessage.outOfControl)
                {
                    context.Kinematics.ResetHorizontalVelocity();
                    context.Flags.RemoveFlag(FlagType.OutOfControl);
                    context.Flags.AddFlag(new Flag(FlagType.OutOfControl, CurrentMessage.messageDuration));

                    if (context.StateMachine.CurrentState is FStateGround)
                    {
                        context.StateMachine.SetState<FStateIdle>();
                    }
                    else if (context.Kinematics.InAir)
                    {
                        context.StateMachine.SetState<FStateAir>();
                    }
                }

                yield return new WaitForSeconds(CurrentMessage.messageDuration);

                if (!_isCurrent)
                    yield break;
            }
        }

        private void Hide()
        {
            _activated = true;

            _timer = cooldown;

            animator.Play("Touch", 0, 0);

            RuntimeManager.PlayOneShot(hintSound, transform.position);
            RuntimeManager.PlayOneShot(disappearSound, transform.position);
        }

        public void Load()
        {
            Hide();
            if (_messagesCoroutine != null)
                StopCoroutine(_messagesCoroutine);
        }

        private void Show()
        {
            _activated = false;
            animator.Play("Appear", 0, 0);
            RuntimeManager.PlayOneShot(appearSound, transform.position);
        }

        private void OnTriggered(HintRing hint)
        {
            _isCurrent = hint == this;
        }
    }
}

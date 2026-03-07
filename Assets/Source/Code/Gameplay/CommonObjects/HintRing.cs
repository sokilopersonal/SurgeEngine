using Alchemy.Inspector;
using FMODUnity;
using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.CommonObjects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace SurgeEngine
{
    public class HintRing : MonoBehaviour
    {
        [System.Serializable]
        private struct HintMessage
        {
            public bool outOfControl;
            [Multiline] public string message;
            public float messageDuration;
        }

        [Title("General")]
        [SerializeField] private List<HintMessage> messages = new List<HintMessage>();
        [SerializeField] private float cooldown = 2f;

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

        private float _timer = 0f;
        private bool _activated = false;
        private bool _isCurrent = false;
        private HintMessage currentMessage;
        
        public string GetMessage() { return currentMessage.message; }
        public float GetMessageDuration() { return currentMessage.messageDuration; }

        public void OnActivated(CharacterBase context)
        {
            if (_activated) return;

            Hide();

            StartCoroutine(PlayMessages(context));
        }

        IEnumerator PlayMessages(CharacterBase context)
        {
            for (int i = 0; i < messages.Count; i++)
            {
                currentMessage = messages[i];

                ObjectEvents.OnHintTriggered?.Invoke(this);

                if (currentMessage.outOfControl)
                {
                    context.Kinematics.ResetHorizontalVelocity();
                    context.Flags.RemoveFlag(FlagType.OutOfControl);
                    context.Flags.AddFlag(new Flag(FlagType.OutOfControl, true, currentMessage.messageDuration));

                    if (context.StateMachine.CurrentState is FStateGround)
                    {
                        context.StateMachine.SetState<FStateIdle>();
                    }
                    else if (context.StateMachine.CurrentState is FStateJump || context.StateMachine.CurrentState is FStateHoming)
                    {
                        context.StateMachine.SetState<FStateAir>();
                    }
                }

                yield return new WaitForSeconds(currentMessage.messageDuration);

                if (!_isCurrent)
                    yield break;
            }
        }

        void Hide()
        {
            _activated = true;

            _timer = cooldown;

            animator.Play("Touch", 0, 0);

            RuntimeManager.PlayOneShot(hintSound, transform.position);
            RuntimeManager.PlayOneShot(disappearSound, transform.position);
        }

        void Show()
        {
            _activated = false;
            animator.Play("Appear", 0, 0);
            RuntimeManager.PlayOneShot(appearSound, transform.position);
        }
        
        private void OnTriggered(HintRing hint)
        {
            _isCurrent = hint == this;
        }

        private void Awake()
        {
            ObjectEvents.OnHintTriggered += OnTriggered;
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
    }
}

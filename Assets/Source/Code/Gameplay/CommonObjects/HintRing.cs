using Alchemy.Inspector;
using FMODUnity;
using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.CommonObjects;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace SurgeEngine
{
    public class HintRing : MonoBehaviour
    {
        [Title("General")]
        [SerializeField] private bool outOfControl = false;
        [SerializeField] [Multiline] private string message;
        [SerializeField] private float messageDuration;
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

        public string GetMessage() { return message; }
        public float GetMessageDuration() { return messageDuration; }

        float _timer = 0f;
        bool _activated = false;

        public void OnActivated(CharacterBase context)
        {
            if (_activated) return;

            ObjectEvents.OnHintTriggered?.Invoke(this);

            Hide();

            if (outOfControl)
            {
                context.Kinematics.ResetHorizontalVelocity();
                context.Flags.RemoveFlag(FlagType.OutOfControl);
                context.Flags.AddFlag(new Flag(FlagType.OutOfControl, true, messageDuration));

                if (context.StateMachine.CurrentState is FStateGround)
                {
                    context.StateMachine.SetState<FStateIdle>();
                }
                else if (context.StateMachine.CurrentState is FStateJump || context.StateMachine.CurrentState is FStateHoming)
                {
                    context.StateMachine.SetState<FStateAir>();
                }
            }
        }

        void Hide()
        {
            _activated = true;

            _timer = messageDuration + cooldown;

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

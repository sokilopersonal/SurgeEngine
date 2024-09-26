using System;
using System.Collections.Generic;
using SurgeEngine.Code.ActorHUD;
using SurgeEngine.Code.ActorSystem;
using UnityEngine;
using UnityEngine.UI;

namespace SurgeEngine.Code.CommonObjects
{
    public class QuickTimeEventUI : MonoBehaviour
    {
        [SerializeField] private Image barFill;

        [SerializeField] private Transform buttonParent;
        
        [SerializeField] private QuickTimeEventUIButton aButton;
        [SerializeField] private QuickTimeEventUIButton xButton;
        [SerializeField] private QuickTimeEventUIButton bButton;
        [SerializeField] private QuickTimeEventUIButton yButton;
        [SerializeField] private QuickTimeEventUIButton lbButton;
        [SerializeField] private QuickTimeEventUIButton rbButton;
        [SerializeField] private QuickTimeEventUIButton aButtonKeyboard;
        [SerializeField] private QuickTimeEventUIButton xButtonKeyboard;
        [SerializeField] private QuickTimeEventUIButton bButtonKeyboard;
        [SerializeField] private QuickTimeEventUIButton yButtonKeyboard;
        [SerializeField] private QuickTimeEventUIButton lbButtonKeyboard;
        [SerializeField] private QuickTimeEventUIButton rbButtonKeyboard;

        [SerializeField] private GameObject qteClick;
        
        private List<QuickTimeEventUIButton> _buttons = new List<QuickTimeEventUIButton>();

        private TrickJumper _trickJumperObject;

        private void Update()
        {
            if (_trickJumperObject == null)
            {
                Destroy(gameObject);
            }
            else
            {
                barFill.fillAmount = _trickJumperObject.GetTimer() / _trickJumperObject.GetCurrentSequence().time;
            }
        }

        public void SetTrickJumper(TrickJumper trickJumper)
        {
            _trickJumperObject = trickJumper;
            
            trickJumper.OnCorrectButton += OnCorrectButtonPressed;
        }

        private void OnCorrectButtonPressed()
        {
            var click = Instantiate(qteClick, ActorStageHUD.Context.transform);
            click.transform.position = _buttons[0].transform.position;
            Destroy(click, 0.65f);
            _buttons[0].Destroy();
            _buttons.RemoveAt(0);
        }

        public void CreateButtonIcon(QTESequence sequence)
        {
            _buttons.Capacity = sequence.buttons.Count;
            var context = ActorContext.Context.input;
            for (int i = 0; i < sequence.buttons.Count; i++)
            {
                var buttonType = sequence.buttons[i].type;
                QuickTimeEventUIButton button = null;
                
                switch (buttonType)
                {
                    case ButtonType.A: button = context.isKeyboard ? aButtonKeyboard : aButton; break;
                    case ButtonType.B: button = context.isKeyboard ? bButtonKeyboard : bButton; break;
                    case ButtonType.X: button = context.isKeyboard ? xButtonKeyboard : xButton; break;
                    case ButtonType.Y: button = context.isKeyboard ? yButtonKeyboard : yButton; break;
                    case ButtonType.LB: button = context.isKeyboard ? lbButtonKeyboard : lbButton; break;
                    case ButtonType.RB: button = context.isKeyboard ? rbButtonKeyboard : rbButton; break;
                }
                
                if (button != null)
                {
                    _buttons.Add(Instantiate(button, buttonParent));
                }
            }
        }

        private void OnDestroy()
        {
            if (_trickJumperObject != null)
            {
                _trickJumperObject.OnCorrectButton -= OnCorrectButtonPressed;                
            }
        }
    }
}
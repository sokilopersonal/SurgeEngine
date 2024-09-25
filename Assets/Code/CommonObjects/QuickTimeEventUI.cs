using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SurgeEngine.Code.CommonObjects
{
    public class QuickTimeEventUI : MonoBehaviour
    {
        [SerializeField] private Image barFill;

        [SerializeField] private Transform buttonParent;
        [SerializeField] private Image dummyButton;
        [SerializeField] private Image aButton;
        [SerializeField] private Image xButton;
        [SerializeField] private Image bButton;
        [SerializeField] private Image yButton;
        [SerializeField] private Image lbButton;
        [SerializeField] private Image rbButton;
        
        [SerializeField] private Image aButtonKeyboard;
        [SerializeField] private Image xButtonKeyboard;
        [SerializeField] private Image bButtonKeyboard;
        [SerializeField] private Image yButtonKeyboard;
        [SerializeField] private Image lbButtonKeyboard;
        [SerializeField] private Image rbButtonKeyboard;
        
        private List<Image> _buttons = new List<Image>();

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
            Image button = _buttons[0];
            _buttons.RemoveAt(0);
            button.enabled = false;
        }

        public void CreateButtonIcon(QTESequence sequence)
        {
            for (int i = 0; i < sequence.buttons.Count; i++)
            {
                Image button = null;
                switch (sequence.buttons[i].type)
                {
                    case ButtonType.A:
                        button = Instantiate(aButton, buttonParent);
                        break;
                    case ButtonType.B:
                        button = Instantiate(bButton, buttonParent);
                        break;
                    case ButtonType.X:
                        button = Instantiate(xButton, buttonParent);
                        break;
                    case ButtonType.Y:
                        button = Instantiate(yButton, buttonParent);
                        break;
                    case ButtonType.LB:
                        button = Instantiate(lbButton, buttonParent);
                        break;
                    case ButtonType.RB:
                        button = Instantiate(rbButton, buttonParent);
                        break;
                    case ButtonType.COUNT:
                        break;
                }
                
                _buttons.Add(button);
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
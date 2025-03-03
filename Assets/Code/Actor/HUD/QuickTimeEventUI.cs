using System;
using System.Collections.Generic;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.CommonObjects;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace SurgeEngine.Code.ActorHUD
{
    public class QuickTimeEventUI : MonoBehaviour
    {
        [SerializeField] private List<QTEButtonSprites> quickTimeEventUIButtons = new List<QTEButtonSprites>();
        
        [SerializeField] private QuickTimeEventUIButton button;
        [SerializeField] private Image barFill;
        [SerializeField] private Transform buttonParent;
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
            GameObject click = Instantiate(qteClick, ActorHUDContext.Context.transform);
            click.transform.position = _buttons[0].transform.position;
            Destroy(click, 0.65f);
            _buttons[0].Destroy();
            _buttons.RemoveAt(0);
        }

        public void CreateButtonIcon(QTESequence sequence)
        {
            _buttons.Capacity = sequence.buttons.Count;
            Actor context = ActorContext.Context;
            ActorInput input = context.input;
            for (int i = 0; i < sequence.buttons.Count; i++)
            {
                ButtonType buttonType = sequence.buttons[i].type;
                InputDevice dv = input.GetDevice();
                string translatedName = input.GetTranslatedDeviceName(dv);
                QTEButtonSprites buttons = quickTimeEventUIButtons.Find(x => x.device == translatedName);
                
                QuickTimeEventUIButton tempButton = Instantiate(button, buttonParent);

                bool isBumper = buttonType is ButtonType.LB or ButtonType.RB && buttons.device != "Keyboard";
                float scale = isBumper ? 1.4f : 1f;
                tempButton.SetButtonAppearence(buttons.GetSprite(buttonType), scale);
                
                if (tempButton != null)
                {
                    _buttons.Add(tempButton);
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

    [Serializable]
    public struct QTEButtonSprites
    {
        public string device;
        
        public Sprite aButtonSprite;
        public Sprite bButtonSprite;
        public Sprite xButtonSprite;
        public Sprite yButtonSprite;
        public Sprite LBButtonSprite;
        public Sprite RBButtonSprite;

        public Sprite GetSprite(ButtonType type)
        {
            switch (type)
            {
                case ButtonType.A:
                    return aButtonSprite;
                case ButtonType.B:
                    return bButtonSprite;
                case ButtonType.X:
                    return xButtonSprite;
                case ButtonType.Y:
                    return yButtonSprite;
                case ButtonType.LB:
                    return LBButtonSprite;
                case ButtonType.RB:
                    return RBButtonSprite;
                case ButtonType.COUNT:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
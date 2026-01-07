using System;
using System.Collections.Generic;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility;
using UnityEngine;
using UnityEngine.UI;

namespace SurgeEngine.Source.Code.Core.Character.HUD
{
    public class QuickTimeEventUI : MonoBehaviour
    {
        [SerializeField] private List<QTEButtonSprites> quickTimeEventUIButtons = new List<QTEButtonSprites>();
        
        [SerializeField] private QuickTimeEventUIButton button;
        [SerializeField] private Image barFill;
        [SerializeField] private Transform buttonParent;
        [SerializeField] private GameObject qteClick;
        
        private List<QuickTimeEventUIButton> _buttons = new List<QuickTimeEventUIButton>();

        private ReactionPlate _reactionPlateObject;
        private ReactionPanelUI _reactionPanelUI;
        private TrickJumper _trickJumperObject;
        private TrickJumperUI _trickJumperUI;

        private void Update()
        {
            if (_trickJumperObject != null)
            {
                barFill.fillAmount = _trickJumperObject.GetTimer() / _trickJumperObject.GetCurrentSequence().time;
            }
            else if (_reactionPlateObject != null)
            {
                barFill.color = Color.Lerp(Color.red, Color.yellow, _reactionPlateObject.GetTimer() / _reactionPlateObject.GetCurrentSequence().time);
                barFill.fillAmount = _reactionPlateObject.GetTimer() / _reactionPlateObject.GetCurrentSequence().time;
            }
        }

        public void SetTrickJumper(TrickJumper trickJumper, TrickJumperUI ui)
        {
            _trickJumperObject = trickJumper;
            _trickJumperUI = ui;
            
            trickJumper.OnCorrectButton += OnCorrectButtonPressed;
        }

        public void SetReactionPanel(ReactionPlate reactionPlate, ReactionPanelUI ui)
        {
            _reactionPlateObject = reactionPlate;
            _reactionPanelUI = ui;

            reactionPlate.OnCorrectButton += OnCorrectButtonPressed;
        }

        private void OnCorrectButtonPressed()
        {
            GameObject click = Instantiate(qteClick, _trickJumperUI != null ? _trickJumperUI.transform : _reactionPanelUI.transform);
            click.transform.position = _buttons[0].transform.position;
            Destroy(click, 0.65f);
            _buttons[0].Destroy();
            _buttons.RemoveAt(0);
        }

        public void CreateButtonIcon(QTESequence sequence)
        {
            _buttons.Capacity = sequence.buttons.Count;
            CharacterBase context = CharacterContext.Context;
            CharacterInput input = context.Input;
            for (int i = 0; i < sequence.buttons.Count; i++)
            {
                ButtonType buttonType = sequence.buttons[i].type;
                GameDevice dv = input.GetDevice();
                QTEButtonSprites buttons = quickTimeEventUIButtons.Find(x => x.device == dv);
                
                QuickTimeEventUIButton tempButton = Instantiate(button, buttonParent);

                bool isBumper = buttonType is ButtonType.LB or ButtonType.RB && buttons.device is not GameDevice.Keyboard;
                float scale = isBumper ? 1.4f : 1f;
                tempButton.SetButtonAppearence(buttons.GetSprite(buttonType), scale);
                
                if (tempButton != null)
                {
                    _buttons.Add(tempButton);
                    tempButton.gameObject.SetActive(true);
                }
            }
        }

        private void OnDestroy()
        {
            if (_trickJumperObject != null)
            {
                _trickJumperObject.OnCorrectButton -= OnCorrectButtonPressed;                
            }

            if (_reactionPlateObject != null)
            {
                _reactionPlateObject.OnCorrectButton -= OnCorrectButtonPressed;
            }
        }
    }

    [Serializable]
    public struct QTEButtonSprites
    {
        public GameDevice device;
        
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
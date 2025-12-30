using System.Collections.Generic;
using SurgeEngine.Source.Code.Gameplay.CommonObjects;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.HUD
{
    public class ReactionPanelUI : MonoBehaviour
    {
        [SerializeField] private QuickTimeEventUI quickTimeEventUI;
        [SerializeField] private QuickTimeMessageUI quickTimeMessageUI;

        private ReactionPanel _reactionPanel;
        private QuickTimeEventUI _currentUI;
        private QuickTimeMessageUI _currentMessageUI;
        private readonly List<QuickTimeEventUI> _activeUIs = new();

        private void OnEnable()
        {
            ObjectEvents.OnReactionPanelTriggered += OnReactionPanelTriggered;
        }

        private void OnDisable()
        {
            ObjectEvents.OnReactionPanelTriggered -= OnReactionPanelTriggered;
            UnsubscribeFromReactionPanel();
        }

        private void OnReactionPanelTriggered(ReactionPanel obj)
        {
            UnsubscribeFromReactionPanel();

            _reactionPanel = obj;

            _reactionPanel.OnQTEResultReceived += OnQTEResultReceived;
            _reactionPanel.OnNewSequenceStarted += OnNewSequenceStarted;
        }

        private void OnNewSequenceStarted(QTESequence sequence)
        {
            // Cleanup previous UI if it exists
            if (_currentUI != null)
            {
                Destroy(_currentUI.gameObject);
                _activeUIs.Remove(_currentUI);
            }

            _currentUI = Instantiate(quickTimeEventUI);
            _currentUI.SetReactionPanel(_reactionPanel, this);
            _currentUI.CreateButtonIcon(sequence);
            _activeUIs.Add(_currentUI);
        }

        private void OnQTEResultReceived(QTEResult result)
        {
            CleanupUIElements();

            _currentMessageUI = Instantiate(quickTimeMessageUI, transform);
            _currentMessageUI.Play(result, _reactionPanel.GetTimer() / _reactionPanel.GetFinishingSequence().time);
            Destroy(_currentMessageUI.gameObject, 1f);
        }

        private void CleanupUIElements()
        {
            foreach (var ui in _activeUIs)
            {
                if (ui != null)
                {
                    Destroy(ui.gameObject);
                }
            }
            _activeUIs.Clear();
            _currentUI = null;
        }

        private void UnsubscribeFromReactionPanel()
        {
            if (_reactionPanel != null)
            {
                _reactionPanel.OnQTEResultReceived -= OnQTEResultReceived;
                _reactionPanel.OnNewSequenceStarted -= OnNewSequenceStarted;
            }
        }

        private void OnDestroy()
        {
            UnsubscribeFromReactionPanel();
            CleanupUIElements();
        }
    }
}
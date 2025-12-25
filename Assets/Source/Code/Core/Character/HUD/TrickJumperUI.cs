using System.Collections.Generic;
using SurgeEngine.Source.Code.Gameplay.CommonObjects;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.HUD
{
    public class TrickJumperUI : MonoBehaviour
    {
        [SerializeField] private QuickTimeEventUI quickTimeEventUI;
        [SerializeField] private QuickTimeMessageUI quickTimeMessageUI;
        
        private TrickJumper _trickJumper;
        private QuickTimeEventUI _currentUI;
        private QuickTimeMessageUI _currentMessageUI;
        private readonly List<QuickTimeEventUI> _activeUIs = new();
        
        private void OnEnable()
        {
            ObjectEvents.OnTrickJumperTriggered += OnTrickJumperTriggered;
        }
        
        private void OnDisable()
        {
            ObjectEvents.OnTrickJumperTriggered -= OnTrickJumperTriggered;
            UnsubscribeFromTrickJumper();
        }
        
        private void OnTrickJumperTriggered(TrickJumper obj)
        {
            UnsubscribeFromTrickJumper();
            
            _trickJumper = obj;
            
            _trickJumper.OnQTEResultReceived += OnQTEResultReceived;
            _trickJumper.OnNewSequenceStarted += OnNewSequenceStarted;
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
            _currentUI.SetTrickJumper(_trickJumper, this);
            _currentUI.CreateButtonIcon(sequence);
            _activeUIs.Add(_currentUI);
        }
        
        private void OnQTEResultReceived(QTEResult result)
        {
            CleanupUIElements();

            _currentMessageUI = Instantiate(quickTimeMessageUI, transform);
            _currentMessageUI.Play(result);
            Destroy(_currentMessageUI, 1f);
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

        private void UnsubscribeFromTrickJumper()
        {
            if (_trickJumper != null)
            {
                _trickJumper.OnQTEResultReceived -= OnQTEResultReceived;
                _trickJumper.OnNewSequenceStarted -= OnNewSequenceStarted;
            }
        }
        
        private void OnDestroy()
        {
            UnsubscribeFromTrickJumper();
            CleanupUIElements();
        }
    }
}
using System;
using FMODUnity;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility
{
    public class JumpSelectorSound : MonoBehaviour
    {
        [SerializeField] private EventReference selectJumpStart;
        [SerializeField] private EventReference selectJumpOK;
        [SerializeField] private EventReference selectJumpWrong;
        [SerializeField] private EventReference selectJumpFall;
        
        private JumpSelector _jumpSelector;

        private void Awake()
        {
            _jumpSelector = GetComponentInParent<JumpSelector>();
        }

        private void OnEnable()
        {
            _jumpSelector.OnJumpSelectorResult += OnResult;
        }

        private void OnDisable()
        {
            _jumpSelector.OnJumpSelectorResult -= OnResult;
        }
        
        private void OnResult(JumpSelectorResultType obj)
        {
            EventReference reference;
            switch (obj)
            {
                case JumpSelectorResultType.Start:
                    reference = selectJumpStart;
                    break;
                case JumpSelectorResultType.OK:
                    reference = selectJumpOK;
                    break;
                case JumpSelectorResultType.Wrong:
                    reference = selectJumpWrong;
                    break;
                case JumpSelectorResultType.Fall:
                    reference = selectJumpFall;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(obj), obj, null);
            }    
            
            RuntimeManager.PlayOneShot(reference, transform.position);
        }
    }
}
using UnityEngine;
using UnityEngine.Events;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.CameraObjects
{
    public class ChangePanOnContact : MonoBehaviour
    {
        private StageObject contact;
        
        [SerializeField] private UnityEvent eventOnContact;
        
        private void Awake()
        {
            contact = GetComponent<StageObject>();
        }
        
        private void OnEnable()
        {
            contact.OnContact += ChangePan;
        }
        
        private void OnDisable()
        {
            contact.OnContact -= ChangePan;
        }

        private void ChangePan(StageObject obj)
        {
            eventOnContact.Invoke();
        }
    }
}
using UnityEngine;
using UnityEngine.Events;

namespace SurgeEngine.Code.CommonObjects
{
    public class ChangePanOnContact : MonoBehaviour
    {
        private ContactBase contact;
        
        [SerializeField] private UnityEvent eventOnContact;
        
        private void Awake()
        {
            contact = GetComponent<ContactBase>();
        }
        
        private void OnEnable()
        {
            contact.OnContact += ChangePan;
        }
        
        private void OnDisable()
        {
            contact.OnContact -= ChangePan;
        }

        private void ChangePan(ContactBase obj)
        {
            eventOnContact.Invoke();
        }
    }
}
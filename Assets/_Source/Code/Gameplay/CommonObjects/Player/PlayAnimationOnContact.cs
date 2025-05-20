using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Player
{
    public class PlayAnimationOnContact : MonoBehaviour
    {
        [SerializeField] private string animationName;
        
        private ContactBase contact;
        private Animator animator;
        
        private void Awake()
        {
            contact = GetComponentInParent<ContactBase>();
            animator = GetComponent<Animator>();
        }
        
        private void OnEnable()
        {
            contact.OnContact += OnContact;
        }
        
        private void OnDisable()
        {
            contact.OnContact -= OnContact;
        }

        private void OnContact(ContactBase obj)
        {
            animator.Play(animationName);
        }
    }
}
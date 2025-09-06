using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.Player
{
    public class PlayAnimationOnContact : MonoBehaviour
    {
        [SerializeField] private string animationName;
        
        private StageObject contact;
        private Animator animator;
        
        private void Awake()
        {
            contact = GetComponentInParent<StageObject>();
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

        private void OnContact(StageObject obj)
        {
            animator.Play(animationName);
        }
    }
}
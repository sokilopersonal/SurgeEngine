using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Player
{
    public class PlayAnimationOnContact : MonoBehaviour
    {
        [SerializeField] private string animationName;
        
        private StageObject _contact;
        private Animator _animator;
        
        private void Awake()
        {
            _contact = GetComponentInParent<StageObject>();
            _animator = GetComponent<Animator>();
        }
        
        private void OnEnable()
        {
            _contact.OnContact += OnContact;
        }
        
        private void OnDisable()
        {
            _contact.OnContact -= OnContact;
        }

        private void OnContact(StageObject obj)
        {
            _animator.Play(animationName);
        }
    }
}
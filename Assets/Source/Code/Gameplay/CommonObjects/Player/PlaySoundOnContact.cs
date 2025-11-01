using FMODUnity;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Player
{
    public class PlaySoundOnContact : MonoBehaviour
    {
        [SerializeField] private EventReference soundReference;
        
        private StageObject contact;

        private void Awake()
        {
            contact = GetComponent<StageObject>();
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
            RuntimeManager.PlayOneShotAttached(soundReference, gameObject);
        }
    }
}
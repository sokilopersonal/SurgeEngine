using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

namespace SurgeEngine.Code.UI
{
    public class PlaySoundOnClick : MonoBehaviour
    {
        [SerializeField] private EventReference sound;
        
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(() => RuntimeManager.PlayOneShot(sound));
        }
    }
}
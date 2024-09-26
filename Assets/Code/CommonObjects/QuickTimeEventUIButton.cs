using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class QuickTimeEventUIButton : MonoBehaviour
    {
        [SerializeField] private CanvasGroup group;
        [SerializeField] private Animator animator;

        public void Destroy()
        {
            animator.SetBool("Fade", true);
        }
    }
}
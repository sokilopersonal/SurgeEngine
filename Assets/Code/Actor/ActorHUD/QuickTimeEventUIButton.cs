using UnityEngine;
using UnityEngine.UI;

namespace SurgeEngine.Code.CommonObjects
{
    public class QuickTimeEventUIButton : MonoBehaviour
    {
        [SerializeField] private CanvasGroup group;
        [SerializeField] private Animator animator;

        [SerializeField] private Image image;

        public void SetButtonAppearence(Sprite sprite, float scale)
        {
            image.sprite = sprite;

            if (scale > 0)
            {
                image.transform.localScale = Vector3.one * scale;
            }
        }
        
        public void Destroy()
        {
            animator.SetBool("Fade", true);
        }
    }
}
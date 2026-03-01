using SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility;
using UnityEngine;
using UnityEngine.UI;

namespace SurgeEngine
{
    public class QuickTimeMessageUI : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Image message;
        [SerializeField] private Image messageAfterImage;

        public void Play(QTEResult result, float time)
        {
            if (result.Equals(QTEResult.Success))
            {
                if (time >= 0.75)
                {
                    animator.Play("Cool", 0, 0f);
                }
                else if (time >= 0.5 && time < 0.75)
                {
                    animator.Play("Great", 0, 0f);
                }
                else
                {
                    animator.Play("Nice", 0, 0f);
                }
            }
            else
            {
                animator.Play("Fail", 0, 0f);
            }
        }
    }
}

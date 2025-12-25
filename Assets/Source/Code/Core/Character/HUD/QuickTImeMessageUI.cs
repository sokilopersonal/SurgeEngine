using SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility;
using UnityEngine;
using UnityEngine.UI;

namespace SurgeEngine
{
    public class QuickTimeMessageUI : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Sprite niceTexture;
        [SerializeField] private Sprite greatTexture;
        [SerializeField] private Sprite coolTexture;
        [SerializeField] private Sprite failTexture;
        [SerializeField] private Image message;
        [SerializeField] private Image messageAfterImage;

        public void Play(QTEResult result, float time)
        {
            if (result.Equals(QTEResult.Success))
            {
                if (time >= 0.75)
                {
                    message.sprite = coolTexture;
                }
                else if (time >= 0.5 && time < 0.75)
                {
                    message.sprite = greatTexture;
                }
                else
                {
                    message.sprite = niceTexture;
                }

                messageAfterImage.sprite = message.sprite;
                animator.Play("Success", 0, 0f);
            }
            else
            {
                message.sprite = failTexture;
                animator.Play("Fail", 0, 0f);
            }
        }
    }
}

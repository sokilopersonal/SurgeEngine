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

        public void Play(QTEResult result)
        {
            if (result.Equals(QTEResult.Success))
            {
                switch (Random.Range(0, 2))
                {
                    case 0:
                        message.sprite = niceTexture;
                        break;
                    case 1:
                        message.sprite = greatTexture;
                        break;
                    case 2:
                        message.sprite = coolTexture;
                        break;
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

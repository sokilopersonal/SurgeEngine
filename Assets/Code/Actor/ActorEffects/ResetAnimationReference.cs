using SurgeEngine.Code.ActorSystem;
using UnityEngine;

namespace SurgeEngine.Code.ActorEffects
{
    public class ResetAnimationReference : MonoBehaviour
    {
        public void ResetAction()
        {
            ActorContext.Context.animation.ResetAction();
        }
    }
}
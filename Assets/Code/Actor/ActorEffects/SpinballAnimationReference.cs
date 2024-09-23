using SurgeEngine.Code.ActorSystem;
using UnityEngine;

namespace SurgeEngine.Code.ActorEffects
{
    public class SpinballAnimationReference : MonoBehaviour
    {
        public void Enable()
        {
            ActorContext.Context.effects.EnableSpinball();   
        }
        
        public void Disable()
        {
            ActorContext.Context.effects.DisableSpinball();  
        }
    }
}
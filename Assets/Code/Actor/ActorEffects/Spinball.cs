using UnityEngine;

namespace SurgeEngine.Code.ActorEffects
{
    public class Spinball : Effect
    {
        public override void Toggle(bool value)
        {
            gameObject.SetActive(value);
        }
    }
}
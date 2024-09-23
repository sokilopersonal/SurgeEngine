using SurgeEngine.Code.ActorSystem;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class ToggleNormalCamera : ActorTrigger
    {
        [SerializeField] private bool bothSide = true;

        public override void OnTriggerContact(Collider msg)
        {
            base.OnTriggerContact(msg);
        }
    }
}
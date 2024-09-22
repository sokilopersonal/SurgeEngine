using SurgeEngine.Code.ActorSystem;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class ToggleNormalCamera : ActorTrigger
    {
        [SerializeField] private bool bothSide = true;

        protected override void OnTriggerContact(Collider msg)
        {
            base.OnTriggerContact(msg);

            float dot = Vector3.Dot(transform.forward, ActorContext.Context.transform.forward);
            Debug.Log(dot);
            
            ActorContext.Context.camera.GetNormalCamera().Toggle();
        }
    }
}
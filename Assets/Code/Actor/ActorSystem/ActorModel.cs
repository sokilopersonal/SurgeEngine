using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorModel : ActorComponent
    {
        public Transform root;

        private void Update()
        {
            root.localPosition = actor.transform.localPosition;
            root.localRotation = Quaternion.Slerp(root.localRotation,
                Quaternion.LookRotation(actor.transform.forward, actor.transform.up),
                15 * Time.deltaTime);
            
            actor.effects.spinball.transform.SetParent(root, false);
        }
    }
}
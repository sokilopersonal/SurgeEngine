using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorModel : ActorComponent
    {
        [SerializeField] private Transform model;

        private void Update()
        {
            model.localPosition = actor.transform.localPosition;
            model.localRotation = Quaternion.Slerp(model.localRotation,
                Quaternion.LookRotation(actor.transform.forward, actor.transform.up),
                8 * Time.deltaTime);
            
            actor.effects.spinball.transform.SetParent(model, false);
        }
    }
}
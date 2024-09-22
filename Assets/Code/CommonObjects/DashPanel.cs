using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Parameters;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class DashPanel : ActorTrigger, ISoundPlayer
    {
        [SerializeField] private float speed = 35f;
        
        public AudioSource source { get; set; }

        protected override void Awake()
        {
            base.Awake();

            source = GetComponent<AudioSource>();
        }

        protected override void OnTriggerContact(Collider msg)
        {
            base.OnTriggerContact(msg);
            
            var context = ActorContext.Context;
            context._rigidbody.linearVelocity = transform.forward * speed;
            context.stateMachine.SetState<FStateGround>();
        }
    }
}
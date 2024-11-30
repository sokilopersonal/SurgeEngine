using SurgeEngine.Code.ActorStates.BaseStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.GameDocuments;
using UnityEngine;

namespace SurgeEngine.Code.ActorStates
{
    public class FStateAfterHoming : FActorState
    {
        private float _timer;
        
        public FStateAfterHoming(Actor owner) : base(owner)
        {
            
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            
            var doc = SonicGameDocument.GetDocument("Sonic");
            var param = doc.GetGroup(SonicGameDocument.HomingGroup);
            
            _timer = param.GetParameter<float>(SonicGameDocumentParams.Homing_Delay);
            
            Common.ResetVelocity(ResetVelocityType.Both);
            Kinematics.Rigidbody.linearVelocity += Vector3.up * param.GetParameter<float>(SonicGameDocumentParams.Homing_AfterForce);
            
            Actor.transform.localRotation = Quaternion.Euler(0f, Actor.model.transform.localRotation.eulerAngles.y, 0f);
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            if (_timer > 0)
            {
                _timer -= dt;
            }
            else
            {
                StateMachine.SetState<FStateAir>();
            }
        }
    }
}
using SurgeEngine.Source.Code.Core.Character.System;

namespace SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic
{
    public class FStateSwingJump : FStateAir
    {
        public float failVel;
        public float successVel;
        
        public FStateSwingJump(CharacterBase owner) : base(owner)
        {
            
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (Kinematics.AirTime > 0.4f)
                StateMachine.SetState<FStateAir>();
        }

        public void Launch(bool successful)
        {
            Rigidbody.linearVelocity = (Character.transform.forward + Character.transform.up).normalized * (successful ? successVel : failVel);
        }
    }
}

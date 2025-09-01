using SurgeEngine._Source.Code.Core.Character.System;

namespace SurgeEngine._Source.Code.Core.Character.States.Characters.Sonic
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
            Rigidbody.linearVelocity = (character.transform.forward + character.transform.up).normalized * (successful ? successVel : failVel);
        }
    }
}

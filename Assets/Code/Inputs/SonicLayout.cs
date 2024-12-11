using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.ActorSystem.Actors;
using UnityEngine.InputSystem;

namespace SurgeEngine.Code.Inputs
{
    public static class SonicInputLayout
    {
        private static Sonic _sonic => (Sonic)ActorContext.Context;
        private static PlayerInput _input => _sonic.input.playerInput;
        
        public static bool DriftHeld => _sonic.input.BHeld || _input.actions["Trigger"].IsPressed();
    }
}
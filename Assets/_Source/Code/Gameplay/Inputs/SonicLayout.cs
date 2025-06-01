using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Core.Actor.System.Characters.Sonic;
using UnityEngine.InputSystem;

namespace SurgeEngine.Code.Gameplay.Inputs
{
    public static class SonicInputLayout
    {
        private static Sonic _sonic => (Sonic)ActorContext.Context;
        private static PlayerInput _input => _sonic.Input.playerInput;
    }
}
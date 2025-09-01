using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Core.Character.System.Characters.Sonic;
using UnityEngine.InputSystem;

namespace SurgeEngine._Source.Code.Gameplay.Inputs
{
    public static class SonicInputLayout
    {
        private static Sonic _sonic => (Sonic)CharacterContext.Context;
        private static PlayerInput _input => _sonic.Input.playerInput;
    }
}
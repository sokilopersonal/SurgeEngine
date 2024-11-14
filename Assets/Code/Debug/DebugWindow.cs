using System;
using System.Linq;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Parameters.SonicSubStates;
using TMPro;
using UnityEngine;

namespace SurgeEngine.Code.SurgeDebug
{
    public class DebugWindow : MonoBehaviour
    {
        [SerializeField] private TMP_Text holder;

        private void Update()
        {
            var actor = ActorContext.Context;

            string[] activeFlags = Enum.GetValues(actor.flags.flagType.GetType())
                .Cast<Enum>()
                .Where(flag => actor.flags.flagType.HasFlag(flag))
                .Select(flag => flag.ToString())
                .ToArray();
            
            string[] text = new string[]
            {
                $"Position: {actor.transform.position}",
                $"Move Dot: {actor.stats.moveDot}",
                $"Euler Angles: {actor.transform.rotation.eulerAngles}",
                $"Current Speed: {actor.stats.currentSpeed}",
                $"Current Vertical Speed: {actor.stats.currentVerticalSpeed}",
                $"Planar Velocity: {actor.stats.planarVelocity}",
                $"State: {actor.stateMachine.currentStateName}",
                $"Camera Pawn: {actor.camera.stateMachine.currentStateName}",
                $"FBoost State: {actor.stateMachine.GetSubState<FBoost>().Active}",
                $"Flags: {string.Join(", ", activeFlags)}"
            };
            
            holder.text = string.Join("\n", text);
        }
    }
}
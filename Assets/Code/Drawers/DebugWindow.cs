using SurgeEngine.Code.Actor.System;
using TMPro;
using UnityEngine;

namespace SurgeEngine.Code.CustomDebug
{
    public class DebugWindow : MonoBehaviour
    {
        [SerializeField] private TMP_Text holder;

        private void Update()
        {
            ActorBase actor = ActorContext.Context;
            string[] text = 
            {
                $"Position: {actor.transform.position}",
                $"Euler Angles: {actor.transform.rotation.eulerAngles}",
                $"Move Dot: {actor.stats.moveDot}",
                $"Current Speed: {actor.kinematics.Velocity.magnitude}",
                $"Current Vertical Speed: {actor.stats.currentVerticalSpeed}",
                $"Body Velocity: {actor.kinematics.Velocity}",
                $"Planar Velocity: {actor.kinematics.PlanarVelocity}",
                $"State: {actor.stateMachine.currentStateName}",
                $"Camera State: {actor.camera.stateMachine.currentStateName}"
            };
            
            holder.text = string.Join("\n", text);
        }
    }
}
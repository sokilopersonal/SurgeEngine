using SurgeEngine.Code.ActorSystem;
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
            string[] text = 
            {
                $"Position: {actor.transform.position}",
                $"Euler Angles: {actor.transform.rotation.eulerAngles}",
                $"Move Dot: {actor.stats.moveDot}",
                $"Current Speed: {actor.kinematics.HorizontalSpeed}",
                $"Current Vertical Speed: {actor.stats.currentVerticalSpeed}",
                $"Planar Velocity: {actor.kinematics.PlanarVelocity}",
                $"State: {actor.stateMachine.currentStateName}",
                $"Camera State: {actor.camera.stateMachine.currentStateName}",
                $"Animation State: {actor.animation.GetCurrentAnimationState()}"
            };
            
            holder.text = string.Join("\n", text);
        }
    }
}
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Parameters.SonicSubStates;
using TMPro;
using UnityEngine;

namespace SurgeEngine.Code.SurgeDebug
{
    public class DebugWindow : MonoBehaviour
    {
        [SerializeField] private TMP_Text position;
        [SerializeField] private TMP_Text rotation;
        [SerializeField] private TMP_Text eulerAngles;
        [SerializeField] private TMP_Text currentSpeed;
        [SerializeField] private TMP_Text currentVerticalSpeed;
        [SerializeField] private TMP_Text planarVelocity;
        [SerializeField] private TMP_Text state;
        [SerializeField] private TMP_Text cameraState;
        [SerializeField] private TMP_Text isBoosting;
        [SerializeField] private TMP_Text outOfControl;

        private void Update()
        {
            var actor = ActorContext.Context;

            position.text = $"Position: {actor.transform.position}";
            rotation.text = $"Move Dot: {actor.stats.moveDot}";
            eulerAngles.text = $"Euler Angles: {actor.transform.rotation.eulerAngles}";
            currentSpeed.text = $"Current Speed: {actor.stats.currentSpeed}";
            currentVerticalSpeed.text = $"Current Vertical Speed: {actor.stats.currentVerticalSpeed}";
            planarVelocity.text = $"Planar Velocity: {actor.stats.planarVelocity}";
            state.text = $"State: {actor.stateMachine.currentStateName}";
            cameraState.text = $"Camera Pawn: {actor.camera.stateMachine.currentStateName}";
            isBoosting.text = $"FBoost State: {actor.stateMachine.GetSubState<FBoost>().Active}";
            outOfControl.text = $"Out Of Control: {actor.flags.HasFlag(FlagType.OutOfControl)} (Time: {actor.flags.GetFlag(FlagType.OutOfControl)?.GetTime():0.0} )";
        }
    }
}
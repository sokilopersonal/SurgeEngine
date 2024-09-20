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
        [SerializeField] private TMP_Text isBoosting;

        private void Update()
        {
            var actor = ActorContext.Context;

            position.text = $"Position: {actor.transform.position}";
            rotation.text = $"Rotation: {actor.transform.rotation}";
            eulerAngles.text = $"Euler Angles: {actor.transform.rotation.eulerAngles}";
            currentSpeed.text = $"Current Speed: {actor.stats.currentSpeed}";
            currentVerticalSpeed.text = $"Current Vertical Speed: {actor.stats.currentVerticalSpeed}";
            planarVelocity.text = $"Planar Velocity: {actor.stats.planarVelocity}";
            state.text = $"State: {actor.stateMachine.CurrentState.ToString().Replace(".Code.Parameters", "")}";
            isBoosting.text = $"FBoost State: {actor.stateMachine.GetSubState<FBoost>().Active}";
        }
    }
}
using SurgeEngine.Code.ActorStates;
using UnityEngine;
namespace SurgeEngine.Code.ActorSystem
{
    public class ActorStartDefiner : MonoBehaviour
    {
        public StartData startData;

        private void Awake()
        {
            var type = startData.startType;
            var context = ActorContext.Context;
            switch (type)
            {
                case StartType.None:
                    break;
                case StartType.Standing:
                    context.animation.TransitionToState("StartS", 0f);
                    break;
                case StartType.Prepare:
                    context.animation.TransitionToState("StartP", 0f);
                    break;
                case StartType.Dash:
                    break;
            }
        }
    }
}
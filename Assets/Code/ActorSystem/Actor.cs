using SurgeEngine.Code.CameraSystem;
using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    public class Actor : MonoBehaviour
    {
        public ActorInput input;
        public new ActorCamera camera;

        private void Awake()
        {
            input.SetOwner(this);
            camera.SetOwner(this);
        }
    }
}
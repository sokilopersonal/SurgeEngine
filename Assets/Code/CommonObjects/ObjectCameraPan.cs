using SurgeEngine.Code.CameraSystem.Pawns;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class ObjectCameraPan : MonoBehaviour
    {
        public PanData data;

        private void Awake()
        {
            data.position = transform.position;
        }
    }
}
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class WideSpring : Spring
    {
        protected override void Awake()
        {
            direction = Vector3.up;
        }
    }
}
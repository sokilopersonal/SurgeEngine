using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorInput : ActorComponent
    {
        public Vector2 lookVector;
        
        private void Update()
        {
            lookVector = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        }
    }
}
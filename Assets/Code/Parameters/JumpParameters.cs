using System;

namespace SurgeEngine.Code.Parameters
{
    [Serializable]
    public class JumpParameters
    {
        public float jumpForce;
        public float jumpHoldForce;
        public float jumpStartTime = 0.275f;
    }
}
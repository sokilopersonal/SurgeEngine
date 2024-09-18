using System;
using UnityEngine;

namespace SurgeEngine.Code.Parameters
{
    [Serializable]
    public class CastParameters
    {
        public LayerMask collisionMask;
        public float castDistance = 1.2f;
        public float castRadius = 0.3f;
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace SurgeEngine.Code.ActorEffects
{
    public class BoostAura : Effect
    {
        private void Awake()
        {
            GetComponent<CustomPassVolume>().targetCamera = Camera.main;
        }
    }
}
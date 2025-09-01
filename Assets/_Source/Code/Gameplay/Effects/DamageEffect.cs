using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.Effects
{
    public class DamageEffect : Effect
    {
        public override void Toggle(bool value)
        {
            var instance = Instantiate(particle, transform.position, Quaternion.identity);
            Destroy(instance.gameObject, 1f);
        }
    }
}
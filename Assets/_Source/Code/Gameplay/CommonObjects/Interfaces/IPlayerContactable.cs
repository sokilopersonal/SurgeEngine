using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.Interfaces
{
    public interface IPlayerContactable
    {
        void OnContact(Collider msg);
    }
}
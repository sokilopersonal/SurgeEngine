using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Interfaces
{
    public interface IPlayerContactable
    {
        void OnContact(Collider msg);
    }
}